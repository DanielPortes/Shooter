using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class src_Models 
{
    #region  - Player -
    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }
    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted = true;
        public bool ViewYInverted = false;

        [Header("Movement - Running")] 
        public float RunningForwardSpeed;
        public float RunningStrafeSpeed;
        
        [Header("Movement - Walking")]
        public float walkingForwardSpeed;
        public float walkingBackwardSpeed;
        public float walkingStrafeSpeed;

        [Header("Jumping")]
        public float jumpingHeight;
        public float jumpingFalloff;
    }

    [Serializable]
    public class CharacterStance
    {
        public float cameraHeight;
        public CapsuleCollider stanceCollider;
    }
    

    

    #endregion
}
