using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour
{
    [SerializeField] private float maximumForce;
    [SerializeField] private float maximumForceTime;
    [SerializeField] private float pushHeight = 20;
    
    private float _mouseButtonDownTimer;
    private Camera _camera;


    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _mouseButtonDownTimer = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Stickman stickman = hitInfo.collider.GetComponent<Stickman>();
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
