/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using UnityEngine;

namespace Nodesk.Scripts.Core
{
    public abstract class AbstractAdapter : MonoBehaviour
    {
        public abstract void OnNodeskTyping(NodeskTypeEventInfo typeEventInfo);

        public virtual bool IsEnableAutocomplete()
        {
            return true;
        }
        
        public virtual Vector3 GetPredictionCanvasWorldPosition()
        {
            return Vector3.zero;
        }
        
        public virtual Quaternion GetPredictionCanvasWorldRotation()
        {
            return Quaternion.identity;
        }
        
        public virtual void ProcessPredictionSelect(int replaceStartIndex, int replaceEndIndex, string select)
        {
            return;
        }

        public virtual string GetText()
        {
            return "";
        }

    }
}