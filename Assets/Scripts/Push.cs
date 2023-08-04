using System;
using System.Collections;
using Infrastructure;
using UnityEngine;
using Services;
using Services.Input;

public class Push : MonoBehaviour
{
    [SerializeField] private float maximumForce;
    [SerializeField] private float maximumForceTime;
    [SerializeField] private float pushHeight = 20;
    [SerializeField] private GameObject particlesGO;
    
    private IInputService _inputService;
    
    private float _mouseButtonDownTimer;
    private Camera _camera;


    private void Awake()
    {
        _camera = Camera.main;
        _inputService = AllServices.Container.Single<IInputService>();
    }

    private void Update()
    {
        if(_inputService.IsMouseButtonDown())
        {
            _mouseButtonDownTimer = Time.time;
        }

        if (_inputService.IsMouseButtonUp())
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Stickman stickman = hitInfo.collider.GetComponent<Stickman>();
                Instantiate(particlesGO, hitInfo.point, Quaternion.identity);
                if (stickman != null)
                {
                    float mouseButtonDownDuration = Time.time - _mouseButtonDownTimer;
                    float forcePercentage = mouseButtonDownDuration / maximumForceTime;
                    float forceMagnitude = Mathf.Lerp(1, maximumForce, forcePercentage);
                    
                    Vector3 forceDirection = stickman.transform.position - _camera.transform.position;

                    forceDirection.y = pushHeight;
                    forceDirection.Normalize();
                    
                    Vector3 force = forceMagnitude * forceDirection;
                    

                    stickman.TriggerRagdoll(force, hitInfo.point);
                }
            }
        }
    }
}
