using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stickman : MonoBehaviour
{
 
    private Animator _animator;
    private CharacterController _characterController;
    
    private Rigidbody[] _ragdollRigidbodies;
    private StickmanState _currentState = StickmanState.Idle;
    private float _randomIdleIndex;
    
    
    private static readonly int IdleIndex = Animator.StringToHash("idleIndex");

    private void Awake()
    {   
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _randomIdleIndex = Random.Range(0, 4);

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
        }
    }
    
    private void IdleBehaviour()
    {
        _currentState = StickmanState.Idle;
        PlayRandomIdleAnimation();
    }

    private void RagdollBehaviour()
    {
        _currentState = StickmanState.Ragdoll;
        _animator.enabled = false;
    }
  

    public void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    
        _animator.enabled = true;
        _characterController.enabled = true;
    }
    
    public void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitInfo)
    {
        EnableRagdoll();
        Rigidbody hitRigidbody = _ragdollRigidbodies.OrderBy(_ragdollRigidbodies => Vector3.Distance(_ragdollRigidbodies.transform.position, hitInfo)).First();
        
        hitRigidbody.AddForce(force, ForceMode.Impulse);
    }
    
    private void PlayRandomIdleAnimation()
    {
        _animator.SetFloat(IdleIndex, _randomIdleIndex);
    }
}
