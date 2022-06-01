/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using UnityEngine;

namespace Nodesk.Scripts.Core
{
    public delegate void TouchEvent(InputData inputData);

    public class InputData
    {
        public InputAction Action;
        public Vector2 Position;
        public Vector2 LastPosition;

        public override string ToString()
        {
            return Action + "," + Position + "," + LastPosition;
        }
    }

    public enum InputAction
    {
        Nothing, StickBeginMove, StickMoving, StickEndMove, Delete, SwitchKeys, Activate, Deactivate , Submit, 
        SelectDown, SelectUp, OppositeStickEndMove, Select  
    }

    public enum InputState
    {
        Up, Down
    }
    
    public abstract  class AbstractInputHandler : MonoBehaviour
    {
        public event TouchEvent TouchEvent;

        protected void InvokeTouchEvent(InputData inputData) 
        {
            TouchEvent?.Invoke(inputData);
        }

        public virtual void ChangeVibrate()
        {
            
        }
        public virtual void SubmitVibrate()
        {
            
        }
    }
}