/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/


using UnityEngine;

namespace Nodesk.Scripts.Core.Controller
{
    public class AbstractKeboardController : MonoBehaviour, IKeyboardController
    {
        public virtual void Reset()
        {
            
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public virtual void ControllerInputOnTouchEvent(InputData inputData)
        {
            throw new System.NotImplementedException();
        }
    }
}