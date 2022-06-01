/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using Nodesk.Scripts.Core;
using Nodesk.Scripts.Core.Converter;
using OVR.OpenVR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nodesk.Scripts.UnityComponentAdapter
{
    [RequireComponent(typeof(InputField))]
    public class InputFieldAdapter : AbstractAdapter, IPointerClickHandler
    {
        [SerializeField] private bool isEnableAutocomplete = true;
        private InputField _target;
        void Start()
        {
            _target = this.GetComponent<InputField>();
        }
        
        public override bool IsEnableAutocomplete()
        {
            return isEnableAutocomplete;
        }
        
        public override void OnNodeskTyping(NodeskTypeEventInfo typeEventInfo)
        {
            _target.MoveTextEnd(false);
            if (typeEventInfo.kind == TypeEventKind.Type)
            {
                string text;
                if (NodeskKeyToInputFieldKey.TryConvertKey(typeEventInfo.character, out text))
                {
                    if (text == "return")
                    {
                        Event ev = Event.KeyboardEvent(text);
                        _target.ProcessEvent(ev);
                    } else if (text == "tab")
                    {
                        Event ev = Event.KeyboardEvent(text);
                        _target.ProcessEvent(ev);
                        
                    } else if (text == "[esc]")
                    {
                        Event ev = Event.KeyboardEvent(text);
                        _target.ProcessEvent(ev);
                    } else if (text == "backspace")
                    {
                        Event ev = Event.KeyboardEvent(text);
                        _target.ProcessEvent(ev);
                    }
                    else
                    {
                        foreach(char c in text)
                        {
                            Event ev = Event.KeyboardEvent("t");
                            ev.character = c;
                            _target.ProcessEvent(ev);
                        }
                    }
                    
                    _target.ForceLabelUpdate();

                    RunPredictionIfNeeded();

                }
            
            }
            else if(typeEventInfo.kind == TypeEventKind.Delete)
            {
                if (!string.IsNullOrEmpty(_target.text))
                {
                    _target.text = _target.text.Remove(_target.text.Length - 1);
                }

                RunPredictionIfNeeded();
            }
            else if(typeEventInfo.kind == TypeEventKind.Enter)
            {
                _target.text += "\n";
                RunPredictionIfNeeded();
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
            
            _target.MoveTextEnd(false);
            
            NodeskManager.GetPredictionController().RunPrediction(this);
        }

        public override string GetText()
        {
            return _target.text;
        }
    }
}