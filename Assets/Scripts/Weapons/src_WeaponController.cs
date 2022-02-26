using System;
using UnityEngine;
using static src_Models;

public class src_WeaponController : MonoBehaviour
{

    private src_CharacterController _characterController;

    [Header("Settings")] public WeaponSettingsModel settingsModel;

    private bool _isInitialised;
    Vector3 _newWeaponRotation;
    Vector3 _newWeaponRotationVelocity;    
    Vector3 _targetWeaponRotation;
    Vector3 _targetWeaponRotationVelocity;

    private void Start()
    {
        _newWeaponRotation = transform.localRotation.eulerAngles;
    }

    private void Awake()
    {
    }

    public void initialise(src_CharacterController characterController)
    {
        _characterController = characterController;
        _isInitialised = true;
    }

    private void Update()
    {
        if (!_isInitialised)
        {
            return;
        }

        _targetWeaponRotation.y += settingsModel.swayAmount *
                                   (settingsModel.swayXInverted ? -_characterController._inputView.x : _characterController._inputView.x) * Time.deltaTime;
        _targetWeaponRotation.x += settingsModel.swayAmount *
                                   (settingsModel.swayYInverted ? _characterController._inputView.y : -_characterController._inputView.y) * Time.deltaTime;
        _targetWeaponRotation.x = Mathf.Clamp(_targetWeaponRotation.x, settingsModel.swayClampX, settingsModel.swayClampX);
        _targetWeaponRotation.y = Mathf.Clamp(_targetWeaponRotation.x, settingsModel.swayClampY, settingsModel.swayClampY);

        _targetWeaponRotation = Vector3.SmoothDamp(_targetWeaponRotation, Vector3.zero,
            ref _targetWeaponRotationVelocity, settingsModel.swayResetSmoothing);
        _newWeaponRotation = Vector3.SmoothDamp(_newWeaponRotation, _targetWeaponRotation,
            ref _newWeaponRotationVelocity, settingsModel.swaySmoothing);
        
        transform.localRotation = Quaternion.Euler(_newWeaponRotation);
    }
    

}