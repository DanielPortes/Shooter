using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static src_Models;
using UnityEngine.Serialization;

public class src_CharacterController : MonoBehaviour
{
    private CharacterController _characterController;
    private DefaultInput _defaultInput;
    private Vector2 _inputMovement;
    private Vector2 _inputView;

    private Vector3 _newCameraRotation;
    private Vector3 _newCharacterRotation;

    [Header("References")] 
    public Transform cameraHolder;
    public Transform feetTransform;

    [Header("Settings")] 
    public PlayerSettingsModel playerSettingsModel;
    public float viewClamYMin = -70f;
    public float viewClamYMax = 80f;
    public LayerMask playerMask;

    [Header("Gravity")] public float gravityAmount;
    public float gravityMin;
    private float _playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")] public PlayerStance PlayerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    private float _stanceCheckErrorMargin = 0.05f;
    
    private float _cameraHeigh;
    private float _cameraHeighVelocity;

    private Vector3 _stanceCapsuleCenterVelocity;
    private float _stanceCapsuleHeightVelocity;
    private bool _isSprinting;

    private Vector3 _newMovementSpeed;
    private Vector3 _newMovementSpeedVelocity;
    
    private void Awake()
    {
        _defaultInput = new DefaultInput();

        _defaultInput.Charachter.Movement.performed += e => _inputMovement = e.ReadValue<Vector2>();
        _defaultInput.Charachter.View.performed += e => _inputView = e.ReadValue<Vector2>();
        _defaultInput.Charachter.Jump.performed += e => Jump();
        _defaultInput.Charachter.Crouch.performed += e => Crouch();
        _defaultInput.Charachter.Prone.performed += e => Prone();
        _defaultInput.Charachter.Sprint.performed += e => ToggleSprint();
        _defaultInput.Charachter.SprintReleased.performed += e => StopSprint();

        _defaultInput.Enable();
        _newCameraRotation = cameraHolder.localRotation.eulerAngles;
        _newCharacterRotation = transform.localRotation.eulerAngles;

        _characterController = GetComponent<CharacterController>();

        _cameraHeigh = cameraHolder.localPosition.y;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
    }
    private void CalculateView()
    {
        _newCharacterRotation.y += playerSettingsModel.ViewXSensitivity *
                                   (playerSettingsModel.ViewYInverted ? -_inputView.x : _inputView.x) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_newCharacterRotation);

        _newCameraRotation.x += playerSettingsModel.ViewYSensitivity *
                                (playerSettingsModel.ViewYInverted ? _inputView.y : -_inputView.y) * Time.deltaTime;

        _newCameraRotation.x = Mathf.Clamp(_newCameraRotation.x, viewClamYMin, viewClamYMax);

        cameraHolder.localRotation = Quaternion.Euler(_newCameraRotation);
    }

    private void CalculateMovement()
    {
        if (_inputMovement.y <= 0.2f)
        {
            _isSprinting = false;
        }
        
        var verticalSpeed = playerSettingsModel.walkingForwardSpeed ;
        var horizontalSpeed = playerSettingsModel.walkingStrafeSpeed;

        if (_isSprinting)
        {
            verticalSpeed = playerSettingsModel.RunningForwardSpeed ;
            horizontalSpeed = playerSettingsModel.RunningStrafeSpeed;
        }

        if (!_characterController.isGrounded)
        {
            playerSettingsModel.SpeedEffector = playerSettingsModel.FallingSpeedEffector;
        }
        else if (PlayerStance == PlayerStance.Crouch)
        {
            playerSettingsModel.SpeedEffector = playerSettingsModel.CrouchSspeedEffector;
        }
        else if (PlayerStance == PlayerStance.Prone)
        {
            playerSettingsModel.SpeedEffector = playerSettingsModel.ProneSpeedEffector;
        }
        else
        {
            playerSettingsModel.SpeedEffector = 1;
        }
        verticalSpeed *= playerSettingsModel.SpeedEffector;
        horizontalSpeed *= playerSettingsModel.SpeedEffector;
        
        _newMovementSpeed = Vector3.SmoothDamp(_newMovementSpeed, new Vector3(
                horizontalSpeed * _inputMovement.x * Time.deltaTime, 0,
                verticalSpeed * _inputMovement.y * Time.deltaTime), ref _newMovementSpeedVelocity,
            _characterController.isGrounded ? playerSettingsModel.MovementSmoothing : playerSettingsModel.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(_newMovementSpeed);

        if (_playerGravity > gravityMin)
        {
            _playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (_playerGravity < -0.1f && _characterController.isGrounded)
        {
            _playerGravity = -1f;
        }

        movementSpeed.y += _playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;

        _characterController.Move(movementSpeed);
    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity,
            playerSettingsModel.jumpingFalloff);
    }

    private void CalculateStance()
    {
        var currentStance = playerStandStance;

        if (PlayerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (PlayerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }

        _cameraHeigh = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.cameraHeight,
            ref _cameraHeighVelocity, playerStanceSmoothing);
        cameraHolder.localPosition =
            new Vector3(cameraHolder.localPosition.x, _cameraHeigh, cameraHolder.localPosition.z);

        _characterController.height = Mathf.SmoothDamp(_characterController.height, currentStance.stanceCollider.height,
            ref _stanceCapsuleHeightVelocity, playerStanceSmoothing);
        _characterController.center = Vector3.SmoothDamp(_characterController.center,
            currentStance.stanceCollider.center,
            ref _stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }

    private void Jump()
    {
        if (!_characterController.isGrounded || PlayerStance == PlayerStance.Prone)
        {
            return;
        }        
        if (PlayerStance == PlayerStance.Crouch)
        {           
            if (StanceCheck(playerStandStance.stanceCollider.height))
            {
                return;
            }
            PlayerStance = PlayerStance.Stand;
            return;
        }

        jumpingForce = Vector3.up * playerSettingsModel.jumpingHeight;
        _playerGravity = 0;
    }
    
    private void Crouch()
    {
        if (PlayerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.stanceCollider.height))
            {
                return;
            }
            PlayerStance = PlayerStance.Stand;
            return;
        }
        if (StanceCheck(playerCrouchStance.stanceCollider.height))
        {
            return;
        }
        PlayerStance = PlayerStance.Crouch;
        
    }
    private void Prone()
    {
        PlayerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        Vector3 start = new Vector3(feetTransform.position.x,
            feetTransform.position.y + _characterController.radius + _stanceCheckErrorMargin, feetTransform.position.z);

        Vector3 end = new Vector3(feetTransform.position.x,
            feetTransform.position.y - _characterController.radius - _stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);
        

        return Physics.CheckCapsule(start, end, _characterController.radius, playerMask);
        
    }

    private void ToggleSprint()
    {
        if (_inputMovement.y <= 0.2f)
        {
            _isSprinting = false;
            return;
        }

        _isSprinting = !_isSprinting;
        
    }
    private void StopSprint()
    {
        if (playerSettingsModel.sprintingHold)
        {
            _isSprinting = false;
        }
    }
}