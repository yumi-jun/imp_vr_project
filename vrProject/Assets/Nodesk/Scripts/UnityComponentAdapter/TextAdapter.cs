/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using Nodesk.Scripts.Core;
using Nodesk.Scripts.Core.Converter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nodesk.Scripts.UnityComponentAdapter
{
    [RequireComponent(typeof(Text))]
    public class TextAdapter : AbstractAdapter, IPointerClickHandler
    {
        [SerializeField] private bool isEnableAutocomplete = true;
        private Text _target;
        void Start()
        {
            _target = this.GetComponent<Text>();
        }

        public override void OnNodeskTyping(NodeskTypeEventInfo typeEventInfo)
        {
            if (typeEventInfo.kind == TypeEventKind.Type)
            {
                string text;
                if (NodeskKeyToCharacter.TryConvertKey(typeEventInfo.character, out text))
                {
                    if (text == "BackSpace")
                    {
                        BackSpace();
                    }
                    else
                    {
                        _target.text += text;    
                    }
                    RunPredictionIfNeeded();
                }
            
            }
            else if(typeEventInfo.kind == TypeEventKind.Delete)
            {
                BackSpace();
                RunPredictionIfNeeded();
            }
            else if(typeEventInfo.kind == TypeEventKind.Enter)
            {
                _target.text += "\n";
                RunPredictionIfNeeded();
            }
        }

        private void BackSpace() 
        {
            if (!string.IsNullOrEmpty(_target.text))
            {
                _target.text = _target.text.Remove(_target.text.Length - 1);
            }   
        }
        
        private void RunPredictionIfNeeded()
        {
            if (isEnableAutocomplete)
            {
                NodeskManager.GetPredictionController().RunPrediction(this);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            NodeskManager.SetAdapter(this);
        }
        
        
        public override Vector3 GetPredictionCanvasWorldPosition()
        {
            var rect = GetComponent<RectTransform>();
            return rect.position - 
                   new Vector3( -rect.rect.xMin * rect.lossyScale.x,
                       (rect.rect.yMax * rect.lossyScale.y),
                       0f);
        }

        public override Quaternion GetPredictionCanvasWorldRotation()
        {
            return transform.rotation;
        }

        public override void ProcessPredictionSelect(int replaceStartIndex, int replaceEndIndex, string select)
        {
            if (replaceStartIndex == 0)
            {
                _target.text = select + " ";
            }
            else
            {
                _target.text = _target.text.Substring(0, replaceStartIndex) + " "+ select + " ";    
            }
            
            NodeskManager.GetPredictionController().RunPrediction(this);
        }

        public override string GetText()
        {
            return _target.text;
        }
        
    }
    
}
