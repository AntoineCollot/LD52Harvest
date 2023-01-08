using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    public float rotateSpeed;
    [Range(0,1)] public float rotateSmooth = 0.1f;
    float refRotate;
    float currentRotationSpeed;

    [Header("Inputs")]
    protected InputMap inputs;

    void Awake()
    {
        inputs = new InputMap();
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
        float rotateInput = inputs.Gameplay.RotateCamera.ReadValue<float>();
        currentRotationSpeed = Mathf.SmoothDamp(currentRotationSpeed, rotateInput * rotateSpeed, ref refRotate, rotateSmooth);

        transform.Rotate(Vector3.down * currentRotationSpeed * Time.deltaTime);
    }
}
