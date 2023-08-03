using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stickman : MonoBehaviour
{
    [SerializeField] private Transform butt;
    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private string standUpFromBackAnimationName;
    [SerializeField] private string standUpFromBellyAnimationName;
    
    [SerializeField] private float timeToResetBones;
    
    private Animator _animator;
    private CharacterController _characterController;
    private Rigidbody[] _ragdollRigidbodies;
    private Transform _hipsBone;
    
    private BoneTransform[] _standUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;

    private StickmanState _currentState = StickmanState.Idle;
    
    private float _randomIdleIndex;
    private float _timeToStandUp;
    private float _elapsedResetBonesTime;

    private bool _isButtOnTheGround;
    
    private static readonly int IdleIndex = Animator.StringToHash("idleIndex");

    private void Awake()
    {   
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _randomIdleIndex = Random.Range(0, 4);
        
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _bones  = _hipsBone.GetComponentsInChildren<Transform>();
        _standUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];
        
        for(int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _standUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        DisableRagdoll();
    }

    private void FixedUpdate()
    {
        _isButtOnTheGround = CheckButtTouchesGround();
        Debug.Log(_isButtOnTheGround);
    }

    private void Update()
    {
          switch (_currentState)
        {
            case StickmanState.Idle:
                IdleBehaviour();
                break;
            case StickmanState.Ragdoll:
                RagdollBehaviour();
                break;
            case StickmanState.StandingUp:
                StandingUpBehaviour();
                break;  
            
            case StickmanState.ResetingBones:
                ResettingBonesBehaviour();
                break;
        }
    }
    
    public void TriggerRagdoll(Vector3 force, Vector3 hitInfo)
    {
        EnableRagdoll();
        Rigidbody hitRigidbody = FindHitRigidbody(hitInfo);
        
        hitRigidbody.AddForceAtPosition(force, hitInfo,ForceMode.Impulse);
        
        _currentState = StickmanState.Ragdoll;
        _timeToStandUp = Random.Range(5, 10);
    }
    
    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);

            if (closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }

        return closestRigidbody;
    }
    
    private void IdleBehaviour()
    {
        PlayRandomIdleAnimation();
    }

    private void RagdollBehaviour()
    {
        _animator.enabled = false;
        _timeToStandUp -= Time.deltaTime;

        if (_timeToStandUp <= 0)
        {
            Debug.Log("Butt on the ground "+_isButtOnTheGround);

            AlignPositionToHips();
            PopulateAnimationStartBoneTransforms(_isButtOnTheGround ? standUpFromBackAnimationName : standUpFromBellyAnimationName , _standUpBoneTransforms);
            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = StickmanState.ResetingBones;
            _elapsedResetBonesTime = 0;
        }
    }

    private void StandingUpBehaviour()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(standUpFromBellyAnimationName) == false && 
            _animator.GetCurrentAnimatorStateInfo(0).IsName(standUpFromBackAnimationName) == false)
        {
            _currentState = StickmanState.Idle;
        }
    }
    private void ResettingBonesBehaviour()
    {
       
        
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / timeToResetBones;

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex ++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[boneIndex].Position,
                _standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransforms[boneIndex].Rotation,
                _standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            _currentState = StickmanState.StandingUp;
            DisableRagdoll();
           
            _animator.Play(_isButtOnTheGround ? standUpFromBackAnimationName : standUpFromBellyAnimationName);
        }
    }
    

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        _animator.enabled = true;
        _characterController.enabled = true;
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }

    private void PlayRandomIdleAnimation()
    {
        _animator.SetFloat(IdleIndex, _randomIdleIndex);
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;
        
        bool onGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1f);

        if (onGround) transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        
        _hipsBone.position = originalHipsPosition;
    }
    
        private void AlignRotationToHips()
        {
            Vector3 originalHipsPosition = _hipsBone.position;
            Quaternion originalHipsRotation = _hipsBone.rotation;
    
            Vector3 desiredDirection = _hipsBone.up * -1;
            desiredDirection.y = 0;
            desiredDirection.Normalize();
    
            Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
            transform.rotation *= fromToRotation;
    
            _hipsBone.position = originalHipsPosition;
            _hipsBone.rotation = originalHipsRotation;
        }

    private void PopulateBoneTransforms(BoneTransform[] boneTransform)
    {
        for(int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransform[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransform[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }
    
    private void PopulateAnimationStartBoneTransforms(string animationName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
            Quaternion rotationBeforeSampling = transform.rotation;

            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationName)
                {
                    clip.SampleAnimation(gameObject, 0);
                    PopulateBoneTransforms(_standUpBoneTransforms);
                    break;
                }
            }

            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        
    }

    private bool CheckButtTouchesGround()
    {
        return Physics.CheckSphere(butt.position, 1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(butt.position, 1f);
    }
}
