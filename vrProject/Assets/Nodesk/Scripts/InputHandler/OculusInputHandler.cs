/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using Nodesk.Scripts.Core;
using UnityEngine;

namespace Nodesk.Scripts.InputHandler
{
    public class OculusInputHandler : AbstractInputHandler
    {
        public enum NodeskHandType
        {
            Left,
            Right
        };
        
        [SerializeField] public NodeskHandType controllerHandType = NodeskHandType.Right;
        [SerializeField] public bool uiFollowController;

        private OVRInput.Controller _vibrationController = OVRInput.Controller.LTouch;
        private OVRInput.RawAxis2D _axisType = OVRInput.RawAxis2D.LThumbstick;
        private OVRInput.Button _deleteButtonType = OVRInput.Button.Two;
        private OVRInput.RawButton _switchKeysButtonType = OVRInput.RawButton.LThumbstick;
        private OVRInput.Button _autocompleteSelectButtonType = OVRInput.Button.PrimaryIndexTrigger;

        private InputState _inputState = InputState.Up;
        private Vector2 _previousPosition = Vector2.zero;
        private readonly Queue<bool> _buttonBuffer = new Queue<bool>();

        
        private static Coroutine _vibrateCoroutine;
        


        public override void ChangeVibrate()
        {
            _vibrateCoroutine = StartCoroutine(nameof(VibrateOculus));
        }

        private IEnumerator VibrateOculus()
        {
            OVRInput.SetControllerVibration(1,0.4f, OVRInput.Controller.LTouch);
            yield return new WaitForSeconds(0.016f);
            OVRInput.SetControllerVibration(0,0);
        }
        
        
        public override void SubmitVibrate()
        {
            if (_vibrateCoroutine != null)
            {
                StopCoroutine(_vibrateCoroutine);
                OVRInput.SetControllerVibration(0,0);
                _vibrateCoroutine = null;
            }
            StartCoroutine(nameof(ClickVibrateOculus));
        }

        private IEnumerator ClickVibrateOculus()
        {
            OVRInput.SetControllerVibration(1,1, _vibrationController); // amplitudeが強度
            yield return new WaitForSeconds(0.05f); // 上記振動の持続時間
            OVRInput.SetControllerVibration(1,0.3f, _vibrationController); // amplitudeが強度
            yield return new WaitForSeconds(0.05f); // 上記振動の持続時間
            OVRInput.SetControllerVibration(0,0);
        }
        
        void Update()
        {
            if (controllerHandType == NodeskHandType.Left)
            {
                _vibrationController = OVRInput.Controller.LTouch;
                _axisType = OVRInput.RawAxis2D.LThumbstick;
                _deleteButtonType = OVRInput.Button.Two;
                _switchKeysButtonType = OVRInput.RawButton.LThumbstick;
                _autocompleteSelectButtonType = OVRInput.Button.PrimaryIndexTrigger;
            }
            else
            {
                _vibrationController = OVRInput.Controller.RTouch;
                _axisType = OVRInput.RawAxis2D.RThumbstick;
                _deleteButtonType = OVRInput.Button.Two;
                _switchKeysButtonType = OVRInput.RawButton.RThumbstick;
                _autocompleteSelectButtonType = OVRInput.Button.SecondaryIndexTrigger;
            }

            if (uiFollowController)
            {
                var position = OVRInput.GetLocalControllerPosition(_vibrationController);
                var rotation = OVRInput.GetLocalControllerRotation(_vibrationController);
                NodeskManager.GetUiTransform().position = position + ((rotation * Vector3.up) * 0.1f);
                NodeskManager.GetUiTransform().rotation = rotation;
            }
            
            
            ProcessInput();
        }


        private void ProcessInput()
        {
            // Switch Keys
            if (OVRInput.GetDown(_switchKeysButtonType))
            {
                InvokeTouchEvent(new InputData()
                {
                    Action = InputAction.SwitchKeys
                });
            }

            // Select
            if (OVRInput.GetUp(_autocompleteSelectButtonType))
            {
                InvokeTouchEvent(new InputData()
                {
                    Action = InputAction.Select
                });
            }
            
            // Stick Input
            var inputData = ProcessStickInput();
            if (inputData != null)
            {
                InvokeTouchEvent(inputData);
            }
            
            // Opposite Stick Input
            var oppositeInputData = ProcessOppositeStickInput();
            if (oppositeInputData != null)
            {
                InvokeTouchEvent(oppositeInputData);
            }
            
            // Delete Key
            var deleteInputData = ProcessDeleteKey();
            if (deleteInputData != null)
            {    
                InvokeTouchEvent(deleteInputData);
            }
        }
        
        
        private float _deleteKeyDownTime;
        private bool _deleteKeyIsDown;
        private InputData ProcessDeleteKey()
        {
            var keyDownInThisFrame = false;
            // check button down
            if (OVRInput.GetDown(_deleteButtonType))
            {
                _deleteKeyDownTime = Time.unscaledTime;
                keyDownInThisFrame = true;
                _deleteKeyIsDown = true;
            }
            
            // check button up
            if (OVRInput.GetUp(_deleteButtonType))
            {
                _deleteKeyDownTime = 0.0f;
                _deleteKeyIsDown = false;
            }

            if (keyDownInThisFrame)
            {
                return new InputData()
                {
                    Action = InputAction.Delete
                };    
            }

            if (_deleteKeyIsDown)
            {   
                var deltaTime = Time.unscaledTime - _deleteKeyDownTime;
                if (deltaTime > 0.2f)
                {
                    _deleteKeyDownTime = Time.unscaledTime;
                    return new InputData()
                    {
                        Action = InputAction.Delete
                    };    
                }
            }

            return null;
        }

        private InputData ProcessStickInput()
        {   
            bool enableSelect = OVRInput.Get(_autocompleteSelectButtonType);
            if (enableSelect)
            {
                return null;
            }

            var vector = OVRInput.Get(_axisType);
            InputAction action = InputAction.Nothing ;
            
            bool buttonDownInFrame = vector.magnitude > 0.001f;
            _buttonBuffer.Enqueue(buttonDownInFrame);
            if (_buttonBuffer.Count > 5)
            {
                _buttonBuffer.Dequeue();
            }

            var buttonDown = false; 
            foreach(var down in _buttonBuffer)
            {
                if (down)
                {
                    buttonDown = true;
                }
            }
            
            if (buttonDown)
            {
                if (_inputState == InputState.Up)
                {
                    action = InputAction.StickBeginMove;
                    _inputState = InputState.Down;
                } else if (_inputState == InputState.Down)
                {
                    action = InputAction.StickMoving;
                }
            }
            else
            {
                if (_inputState == InputState.Down)
                {
                    action = InputAction.StickEndMove;
                    _inputState = InputState.Up;
                } else if (_inputState == InputState.Up)
                {
                    action = InputAction.StickEndMove;
                }
            }

            if (action != InputAction.Nothing)
            {
                var position = vector;
            
                if (position == Vector2.zero)
                {
                    position = _previousPosition;
                }

                var lastPosition = _previousPosition;
                _previousPosition = position;
                return new InputData()
                {
                    Action = action, Position = position, LastPosition = lastPosition
                };
                
            }

            return null;
        }

        private Vector2 _lastVector2 = Vector2.zero;
        private float _selectUpDownTime;
        private bool _selectUpIsDown;
        private float _selectDownDownTime;
        private bool _selectDownIsDown;
        private InputData ProcessOppositeStickInput()
        {   
            bool enableSelect = OVRInput.Get(_autocompleteSelectButtonType);
            if (!enableSelect)
            {
                return null;
            }
            
            var vector = OVRInput.Get(_axisType);
            InputAction action = InputAction.Nothing ;
            
            // Process Select Down
            var stickDownInFrame = vector.y < -0.7f && _lastVector2.y > -0.7f;
            if (stickDownInFrame)
            {
                _selectDownDownTime = Time.unscaledTime;
                _selectDownIsDown = true;
            }
            if (vector.y > -0.7f)
            {
                _selectDownDownTime = 0;
                _selectDownIsDown = false;
            }
            if (stickDownInFrame)
            {
                _lastVector2 = vector;
                return new InputData()
                {
                    Action = InputAction.SelectDown, Position = Vector3.zero
                };
            }
            if (_selectDownIsDown)
            {   
                var deltaTime = Time.unscaledTime - _selectDownDownTime;
                if (deltaTime > 0.2f)
                {
                    _lastVector2 = vector;
                    _selectDownDownTime = Time.unscaledTime;
                    return new InputData()
                    {
                        Action = InputAction.SelectDown, Position = Vector3.zero
                    };    
                }
            }
            
            // Process Select Up
            var stickUpInFrame = vector.y > 0.7f && _lastVector2.y < 0.7f;
            if (stickUpInFrame)
            {
                _selectUpDownTime = Time.unscaledTime;
                _selectUpIsDown = true;
            }
            if (vector.y < 0.7f)
            {
                _selectUpDownTime = 0;
                _selectUpIsDown = false;
            }
            if (stickUpInFrame)
            {
                _lastVector2 = vector;
                return new InputData()
                {
                    Action = InputAction.SelectUp, Position = Vector3.zero
                };
            }
            if (_selectUpIsDown)
            {   
                var deltaTime = Time.unscaledTime - _selectUpDownTime;
                if (deltaTime > 0.2f)
                {
                    _lastVector2 = vector;
                    _selectUpDownTime = Time.unscaledTime;
                    return new InputData()
                    {
                        Action = InputAction.SelectUp, Position = Vector3.zero
                    };    
                }
            }

            _lastVector2 = vector;
            return null;
        }
    }
}