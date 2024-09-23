using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls _controls;

    public Vector2 moveInput;
    public Vector2 aimInput;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
        
        _controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
        
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
