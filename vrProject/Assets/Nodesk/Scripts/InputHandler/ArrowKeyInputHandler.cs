/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System;
using Nodesk.Scripts.Core;
using UnityEngine;

namespace Nodesk.Scripts.InputHandler
{
    public class ArrowKeyInputHandler : AbstractInputHandler
    {
        [SerializeField] public KeyCode UpKey = KeyCode.UpArrow;
        [SerializeField] public KeyCode DownKey = KeyCode.DownArrow;
        [SerializeField] public KeyCode LeftKey = KeyCode.LeftArrow;
        [SerializeField] public KeyCode RightKey = KeyCode.RightArrow;
        
        [SerializeField] public KeyCode BackSpaceKey = KeyCode.Backspace;
        [SerializeField] public KeyCode SwitchKeysKey = KeyCode.LeftShift;
        [SerializeField] public KeyCode AutocompleteSelectKey = KeyCode.LeftControl;
        
        

        private InputState _inputState = InputState.Up;
        private Vector2 _previousPosition = Vector2.zero;
        private bool _downAnyKey = false;
        private static IDisposable _vibrateCoroutine;

        void Update()
        {
            ProcessInput();
        }

        private readonly float _delayTime = 0.05f;

        private float _lastPressUpArrowTime;
        private float _lastPressDownArrowTime;
        private float _lastPressLeftArrowTime;
        private float _lastPressRightArrowTime;
        private float _beginKeyDownTime;
        private void ProcessInput()
        {
            // check Switch Key down
            if (Input.GetKeyDown(SwitchKeysKey))
            {
                InvokeTouchEvent(new InputData()
                {
                    Action = InputAction.SwitchKeys
                });
            }
            
            
            // check Switch Key down
            if (Input.GetKeyUp(AutocompleteSelectKey))
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
            
            // Stick Input
            var selectInputData = ProcessSelectStickInput();
            if (selectInputData != null)
            {
                InvokeTouchEvent(selectInputData);
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
            if (Input.GetKeyDown(BackSpaceKey))
            {
                _deleteKeyDownTime = Time.unscaledTime;
                keyDownInThisFrame = true;
                _deleteKeyIsDown = true;
            }
            
            // check button up
            if (Input.GetKeyUp(BackSpaceKey))
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
            
            bool enableSelect = Input.GetKey(AutocompleteSelectKey);
            if (enableSelect)
            {
                return null;
            }
            
            // check Arrow Keys down
            if (Input.GetKey(UpKey))
            {   
                _lastPressUpArrowTime = Time.time;
            }
            if (Input.GetKey(DownKey))
            {
                _lastPressDownArrowTime = Time.time;
            }
            if (Input.GetKey(LeftKey))
            {
                _lastPressLeftArrowTime = Time.time;
            }
            if (Input.GetKey(RightKey))
            {
                _lastPressRightArrowTime = Time.time;
            }
            
            // process delay
            var downAnyKeyInFrame = Input.GetKey(UpKey) ||
                                    Input.GetKey(DownKey) ||
                                    Input.GetKey(LeftKey) ||
                                    Input.GetKey(RightKey);
            
            if (!_downAnyKey && downAnyKeyInFrame)
            {
                _beginKeyDownTime = Time.time;
                _downAnyKey = true;
            }

            if (_downAnyKey && !downAnyKeyInFrame)
            {
                _downAnyKey = false;
            }
            _downAnyKey = downAnyKeyInFrame;
            bool buttonDown = (Time.time - _beginKeyDownTime) > _delayTime;
            if (buttonDown)
            {
                if (!downAnyKeyInFrame)
                {
                    buttonDown = false;
                }
            }
            
            // create vector by uptime
            var vector = Vector2.zero;
            if (Time.time - _lastPressUpArrowTime < _delayTime)
            {
                vector += Vector2.up;
            }
            if (Time.time - _lastPressDownArrowTime < _delayTime)
            {
                vector += Vector2.down;
            }
            if (Time.time - _lastPressLeftArrowTime < _delayTime)
            {
                vector += Vector2.left;
            }
            if (Time.time - _lastPressRightArrowTime < _delayTime)
            {
                vector += Vector2.right;
            }

            // create Input Event
            InputAction action = InputAction.Nothing ;
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
                _previousPosition = position;

                return new InputData()
                {
                    Action = action, Position = position
                };
            }

            return null;
        }
        
        private float _selectUpDownTime;
        private bool _selectUpIsDown;
        private float _selectDownDownTime;
        private bool _selectDownIsDown;
        
        private InputData ProcessSelectStickInput()
        {   
            bool enableSelect = Input.GetKey(AutocompleteSelectKey);
            if (!enableSelect)
            {
                return null;
            }

            // Process Select Down
            var stickDownInFrame = Input.GetKeyDown(DownKey);
            if (stickDownInFrame)
            {
                _selectDownDownTime = Time.unscaledTime;
                _selectDownIsDown = true;
            }
            if (Input.GetKeyUp(DownKey))
            {
                _selectDownDownTime = 0;
                _selectDownIsDown = false;
            }
            if (stickDownInFrame)
            {
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
                    _selectDownDownTime = Time.unscaledTime;
                    return new InputData()
                    {
                        Action = InputAction.SelectDown, Position = Vector3.zero
                    };    
                }
            }
            
            // Process Select Up
            var stickUpInFrame = Input.GetKeyDown(UpKey);
            if (stickUpInFrame)
            {
                _selectUpDownTime = Time.unscaledTime;
                _selectUpIsDown = true;
            }
            if (Input.GetKeyUp(UpKey))
            {
                _selectUpDownTime = 0;
                _selectUpIsDown = false;
            }
            if (stickUpInFrame)
            {
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
                    _selectUpDownTime = Time.unscaledTime;
                    return new InputData()
                    {
                        Action = InputAction.SelectUp, Position = Vector3.zero
                    };    
                }
            }
            
            return null;
        }



    }
}