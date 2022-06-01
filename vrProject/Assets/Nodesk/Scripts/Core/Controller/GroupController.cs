/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace Nodesk.Scripts.Core.Controller
{
    public class GroupController : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private List<ElementController> elementControllers;
        private Vector3 initialPosition;
    
        public int Index => index;
        public Vector3 InitialPosition => initialPosition;

        private void Awake()
        {
            initialPosition = transform.localPosition;
        }

        [ContextMenu("Hover")]
        public void Hover()
        {
            transform.localScale = Vector3.one * 1.5f;
            transform.position = new Vector3(initialPosition.x,initialPosition.y,-0.5f);
        }
    
        [ContextMenu("UnHover")]
        public void UnHover()
        {
            transform.localScale = Vector3.one;
            transform.localPosition = initialPosition;
        }
    
    
        public void HoverElement(int index)
        {
            if (elementControllers.Count == 1)
            {
                elementControllers[0].Hover();
            }
            else
            {
                foreach (var circleElement in elementControllers)
                {
                    if (circleElement.Index == index)
                    {
                        circleElement.Hover();
                    }
                    else
                    {
                        circleElement.UnHover();
                    }
                }
            }
        }

        public ElementController GetElement(int elementIndex)
        {

            if (elementControllers.Count == 1)
            {
                return elementControllers[0];
            }
            else
            {
                foreach (var circleElement in elementControllers)
                {
                    if (circleElement.Index == elementIndex)
                    {
                        return circleElement;
                    }
                }

            }

            return null;
        }
    
    
        public List<ElementController> GetElements()
        {
            return elementControllers;
        }

        public void UnHoverElements()
        {
            if (elementControllers.Count == 1)
            {
                return;
            }
            else
            {
                foreach (var circleElement in elementControllers)
                {
                    circleElement.UnHover();
                }
            }
        }

        [ContextMenu("Deactivate")]
        public void Deactivate()
        {
            foreach (var circleElement in elementControllers)
            {
                circleElement.Deactivate();
            }
        }
    
    
    
        [ContextMenu("Activate")]
        public void Activate()
        {
            foreach (var circleElement in elementControllers)
            {
                circleElement.Activate();
            }
        }

        public string GetKey(int elementIndex)
        {
            if (elementControllers.Count == 1)
            {
                return elementControllers[0].GetKey();
            }
            else
            {
                foreach (var circleElement in elementControllers)
                {
                    if (circleElement.Index == elementIndex)
                    {
                        return circleElement.GetKey();
                    }
                }
            }
            return "";
        }
    
    
        public void SwitchToAlphabet()
        {
            foreach (var circleElement in elementControllers)
            {
                circleElement.SwitchToAlphabet();
            }
        }
    }
}
