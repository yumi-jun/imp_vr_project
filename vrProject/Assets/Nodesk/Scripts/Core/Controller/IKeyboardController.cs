/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

namespace Nodesk.Scripts.Core.Controller
{
    public interface IKeyboardController
    {
        void Reset();
        
        void Activate();

        void Deactivate();

        void ControllerInputOnTouchEvent(InputData inputData);
    }
}