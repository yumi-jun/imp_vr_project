/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using TMPro;
using UnityEngine;

namespace Nodesk.Scripts.Core.Controller
{
    public class ElementController : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private bool isCenter;
    
        [SerializeField] private string key;
        [SerializeField] private string printing;
        [SerializeField] private float size;
    
        [SerializeField] private TMP_Text text;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color activeTextColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private Color inactiveTextColor;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Vector3 initialPosition;
    
        public int Index => index;
        public bool IsCenter => isCenter;
    
        private string _currentKey;
        public Vector3 InitialPosition => initialPosition;

        private void Awake()
        {
            if (text != null)
            {
                var parentIsGroup = transform.parent.gameObject.GetComponent<GroupController>() != null;
                if (parentIsGroup)
                {
                    text.transform.localRotation = Quaternion.Inverse(transform.parent.localRotation);
                    icon.transform.localRotation = Quaternion.Inverse(transform.parent.localRotation);
                }

                if (string.IsNullOrEmpty(printing))
                {
                    if(icon != null) icon.gameObject.SetActive(true);
                    
                    text.gameObject.SetActive(false);                    
                }
                else
                {
                    if(icon != null) icon.gameObject.SetActive(false);
                    
                    text.gameObject.SetActive(true);
                    text.text = printing;
                }
                
                text.fontSize = size;
                _currentKey = key;
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = activeColor;
            text.color = activeTextColor;

            initialPosition = transform.localPosition;
        }

        [ContextMenu("Hover")]
        public void Hover(bool highlight = false)
        {
            if (isCenter)
            {
                transform.localScale = Vector3.one * 1.5f;
            }
            else
            {
                transform.localScale = Vector3.one * 0.75f;
            }
        
        }
    
        [ContextMenu("UnHover")]
        public void UnHover(bool highlight = false)
        {
            if (isCenter)
            {
                transform.localScale = Vector3.one * 1f;    
            }
            else
            {
                transform.localScale = Vector3.one * 0.5f;
            }
        }
    
    
        [ContextMenu("Activate")]
        public void Activate()
        {
            spriteRenderer.color = activeColor;
            text.color = activeTextColor;
        }
    
    
        [ContextMenu("Deactivate")]
        public void Deactivate()
        {
            spriteRenderer.color = inactiveColor;
            text.color = inactiveTextColor;
        }

        public string GetKey()
        {
            return _currentKey;
        }

        public void SwitchToAlphabet()
        {
            text.text = string.IsNullOrEmpty(printing) ? key : printing;
            text.fontSize = size;
            _currentKey = key;
        }

        public void ResetPosition()
        {
            transform.localPosition = InitialPosition;
        }
    }
}
