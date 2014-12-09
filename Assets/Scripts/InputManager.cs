
using System;
using System.Collections;
using UnityEngine;

//This will need to be first in script execution order so other scripts can get inputs on the same frame
namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Abstracts inputs behind a singleton object and 
    /// allows other scripts to enable or disable different input groups, 
    /// which cause those inputs to always return as their default value
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        private float _horitzontalAxis;
        private float _verticalAxis;
        private float _rawVerticalAxis;
        private float _rawHoritzontalAxis;
        private bool _cameraAction;
        private bool _interactAction;

        private bool _playerInputEnabled = true;
        private bool _cameraInputEnabled = true;
        private bool _playerMovementEnabled = true;

        public bool DebugWalkRight = false;

        public float HoritzontalAxis
        {
            get
            {
                if (DebugWalkRight) return 1;

                return _playerMovementEnabled && PlayerInputEnabled ? _horitzontalAxis : 0;
            }
            private set
            {
                _horitzontalAxis = value; 
                
            }
        }

        public float VerticalAxis
        {
            get
            {
                return _playerMovementEnabled && PlayerInputEnabled ? _verticalAxis : 0;
            }
            private set { _verticalAxis = value; }
        }

        public float RawHoritzontalAxis
        {
            get
            {
                if (DebugWalkRight) return 1;
                
                return _playerMovementEnabled && PlayerInputEnabled ? _rawHoritzontalAxis : 0;
            }
            private set { _rawHoritzontalAxis = value; }
        }

        public float RawVerticalAxis
        {
            get
            {
                return _playerMovementEnabled && PlayerInputEnabled ? _rawVerticalAxis : 0;
            }
            private set { _rawVerticalAxis = value; }
        }

        public bool CameraAction
        {
            get
            {
                return _cameraAction
                    && PlayerInputEnabled 
                    && _cameraInputEnabled;
            }
            private set { _cameraAction = value; }
        }

        public bool InteractAction
        {
            get
            {
                return _interactAction
                    && PlayerInputEnabled;
            }
            private set { _interactAction = value; }
        }


        private bool _climbButton = false;
        public bool ClimbButton
        {
            get
            {
                return _climbButton
                    && PlayerInputEnabled;
            }
            private set { _climbButton = value; }
        }

        void Update()
        {
            //Get all the inputs for da frame
            VerticalAxis = Input.GetAxis("Vertical");
            RawVerticalAxis = Input.GetAxisRaw("Vertical");

            HoritzontalAxis = Input.GetAxis("Horizontal");
            RawHoritzontalAxis = Input.GetAxisRaw("Horizontal");
            
            InteractAction = Input.GetKeyDown(KeyCode.E);
            CameraAction = Input.GetKeyDown(KeyCode.Q);
            ClimbButton = Input.GetKeyDown(KeyCode.Space);
        }

        public bool PlayerEnteringComment;

        public bool PlayerInputEnabled
        {
            get { return _playerInputEnabled && !PlayerEnteringComment; }
            set{ _playerInputEnabled = value;}
        }

        public bool PlayerMovementEnabled
        {
            get { return _playerMovementEnabled; }
            set { _playerMovementEnabled = value; }
        }

        public bool CameraControlEnabled
        {
            private get { return _cameraInputEnabled; }
            set { _cameraInputEnabled = value; }
        }
    }
}

