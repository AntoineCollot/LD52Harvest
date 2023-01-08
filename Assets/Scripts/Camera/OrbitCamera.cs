using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{

    public bool auto;
    [Header("Auto")]
    public float autoRotateSpeed;
    public float noCameraLockRadius = 5;
    Transform player;
    Vector3 currentCameraDirection;
    Vector3 smoothedCamDir;
    Vector3 targetDirection;
    Vector3 refDirection;
    [Range(0,1)] public float autoRotateSmooth = 0.1f;

    [Header("Manual")]
    public float rotateSpeed;
    [Range(0,1)] public float rotateSmooth = 0.1f;
    float refRotate;
    float currentRotationSpeed;
    protected InputMap inputs;

    void Awake()
    {
        inputs = new InputMap();
    }
    private void Start()
    {
        player = PlayerState.Instance.transform;

        currentCameraDirection = 2*transform.position -player.position;
        smoothedCamDir = currentCameraDirection;
    }

    void OnEnable()
    {
        inputs.Enable();
        inputs.Gameplay.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if(auto)
        {
            UpdateAutoCamera();
        }
        else
        {
            UpdateManualCamera();
        }
        
    }

    void UpdateManualCamera()
    {
        float rotateInput = inputs.Gameplay.RotateCamera.ReadValue<float>();
        currentRotationSpeed = Mathf.SmoothDamp(currentRotationSpeed, rotateInput * rotateSpeed, ref refRotate, rotateSmooth);

        transform.Rotate(Vector3.down * currentRotationSpeed * Time.deltaTime);
    }

    void UpdateAutoCamera()
    {
        float distToPlayer = Vector3.Distance(player.position, transform.position);
        if (distToPlayer > noCameraLockRadius)
        {
            targetDirection = 2 * transform.position - player.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
        }

        float rotationStep = Mathf.Deg2Rad * autoRotateSpeed * Time.deltaTime;
        currentCameraDirection = Vector3.RotateTowards(currentCameraDirection, targetDirection, rotationStep, 0);
        currentCameraDirection.y = 0;

        smoothedCamDir = Vector3.SmoothDamp(smoothedCamDir, currentCameraDirection, ref refDirection, autoRotateSmooth);

        transform.LookAt(transform.position + smoothedCamDir);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (auto)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, noCameraLockRadius);
        }
    }
#endif
}
