using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stickman : MonoBehaviour
{
    [SerializeField] private string faceUpStandUpAnimation;
    [SerializeField] private string faceDowmStandUpAnimation;
    
    [SerializeField] private float timeToResetBones;
    
    private Animator _animator;
    private CharacterController _characterController;
    private Rigidbody[] _ragdollRigidbodies;
    private Transform _hipsBone;
    
    private BoneTransform[] _faceUpStandUpBoneTransforms;
    private BoneTransform[] _faceDownStandUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;

    private StickmanState _currentState = StickmanState.Idle;
    
    private float _randomIdleIndex;
    private float _timeToStandUp;
    private float _elapsedResetBonesTime;

    private bool _isFacingUp;
    
    private static readonly int IdleIndex = Animator.StringToHash("idleIndex");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _randomIdleIndex = Random.Range(0, 4);

        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _bones = _hipsBone.GetComponentsInChildren<Transform>();
        _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms(faceDowmStandUpAnimation, _faceDownStandUpBoneTransforms);
        PopulateAnimationStartBoneTransforms(faceUpStandUpAnimation, _faceUpStandUpBoneTransforms);

        DisableRagdoll();
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

    private string GetStandingUpAnimationName =>
        _isFacingUp ? faceUpStandUpAnimation : faceDowmStandUpAnimation;
    
    private BoneTransform[] GetStandingUpBoneTransforms =>
        _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    
    
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
    
    
    private void IdleBehaviour()
    {
        PlayRandomIdleAnimation();
    }

    private void RagdollBehaviour()
    {
        _timeToStandUp -= Time.deltaTime;

        if (_timeToStandUp <= 0)
        {
            _isFacingUp = _hipsBone.forward.y > 0;

            AlignRotationToHips();
            AlignPositionToHips();
            
            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = StickmanState.ResetingBones;
            _elapsedResetBonesTime = 0;
        }
    }

    private void StandingUpBehaviour()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(GetStandingUpAnimationName) == false)
        {
            _currentState = StickmanState.Idle;
        }
    }
    
    private void ResettingBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / timeToResetBones;
        
        BoneTransform[] standingUpBoneTransforms = GetStandingUpBoneTransforms;

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex ++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[boneIndex].Position,
                standingUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransforms[boneIndex].Rotation,
                standingUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            _currentState = StickmanState.StandingUp;
            DisableRagdoll();
           
            _animator.Play(GetStandingUpAnimationName, 0 , 0);
        }
    }
    
    private void PlayRandomIdleAnimation()
    {
        _animator.SetFloat(IdleIndex, _randomIdleIndex);
    }


    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.up;

        if (_isFacingUp)
        {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPosition;
        _hipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position; 
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandingUpBoneTransforms[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }
        
        _hipsBone.position = originalHipsPosition;
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
                    PopulateBoneTransforms(boneTransforms);
                    break;
                }
            }

            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        
    }
}
