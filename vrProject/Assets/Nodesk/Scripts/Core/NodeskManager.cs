/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Nodesk.Scripts.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Nodesk.Scripts.Core
{
  #region Enums
  public enum GeneralEventKind
  {
    Delete,Enter,Tab,Open,Close,Submit
  }
  public enum TypeEventKind
  {
    Type,Delete,Enter
  }
  #endregion
    
  #region Event Defines
  public class NodeskTypeEventInfo
  {
    public TypeEventKind kind;
    public string character;

    public override string ToString()
    {
      return kind + "," + character;
    }
  }
  
  [Serializable] public class GeneralEvent : UnityEvent<GeneralEventKind> {}
  
  #endregion

  public class NodeskManager : MonoBehaviour
  { 
    [SerializeField, HeaderAttribute ("User Settings")] 
    public AbstractAdapter adapter;
    [SerializeField] 
    private AbstractInputHandler inputHandler;
    [SerializeField] 
    private bool useBelowControllerInEditor = true;
    [SerializeField]
    private AbstractInputHandler inputHandlerInUnityEditor;
    [SerializeField]
    public GeneralEvent generalEvent = new GeneralEvent();

    [SerializeField, HeaderAttribute("System Settings")]
    private List<AbstractKeboardController> keyboardControllers = new List<AbstractKeboardController>();

    [SerializeField] 
    private GameObject uiView;

    [SerializeField] private PredictionController predictionController;
    
    private IKeyboardController _currentKeyboardController;
    private int _activeKeyboardIndex = 0;

    #region Static

    private static NodeskManager _instance;
    public static NodeskManager Instance()
    {
      return _instance;
    }
    
    public static void SetAdapter(AbstractAdapter abstractAdapter)
    {
      Instance().adapter = abstractAdapter;
      if (abstractAdapter != null && abstractAdapter.IsEnableAutocomplete())
      {
        Instance().predictionController.Show();
        Instance().predictionController.transform.position = abstractAdapter.GetPredictionCanvasWorldPosition();
        Instance().predictionController.transform.rotation = abstractAdapter.GetPredictionCanvasWorldRotation();
      }
      else
      {
        Instance().predictionController.Hide();
      }
    }

    public static PredictionController GetPredictionController()
    {
      return Instance().predictionController;
    }

    public static Transform GetUiTransform()
    {
      return Instance().uiView.transform;
    }


    #endregion

    private void Awake()
    {
      _instance = this;
    }

    void Start()
    {
#if UNITY_EDITOR
      if (useBelowControllerInEditor)
      {
        inputHandler = inputHandlerInUnityEditor;
      }
#endif
      inputHandler.TouchEvent += InputHandlerOnTouchEvent;
      SwitchKeyboard(_activeKeyboardIndex);
      Instance().predictionController.Hide();
    }
  
    private void InputHandlerOnTouchEvent(InputData inputData)
    {
      switch (inputData.Action)
      {
        case InputAction.Activate:
          uiView.gameObject.SetActive(true);
          generalEvent.Invoke(GeneralEventKind.Open);
          break;
        case InputAction.Deactivate:
          uiView.gameObject.SetActive(false);
          generalEvent.Invoke(GeneralEventKind.Close);
          break;
        case InputAction.Delete:
          FireEvent(new NodeskTypeEventInfo()
          {
            kind = TypeEventKind.Delete
          });
          generalEvent.Invoke(GeneralEventKind.Delete);
          break;
        case InputAction.SwitchKeys:
          if (_activeKeyboardIndex + 1 >= keyboardControllers.Count)
          {
            _activeKeyboardIndex = 0;
          } else
          {
            _activeKeyboardIndex++;
          }
          SwitchKeyboard(_activeKeyboardIndex);
          break;
        case InputAction.SelectDown:
        case InputAction.SelectUp:
        case InputAction.OppositeStickEndMove:
        case InputAction.Select:
          if (_currentKeyboardController != null)
          {
            _currentKeyboardController.Reset();
          }

          if (predictionController.IsShow())
          {
            predictionController.ControllerInputOnTouchEvent(inputData);  
          }
          
          break;  
        default:
          if (_currentKeyboardController != null)
          {
            _currentKeyboardController.ControllerInputOnTouchEvent(inputData);
          }
          break;
      }
    }

    private void SwitchKeyboard(int index)
    {
      for (int i = 0; i < keyboardControllers.Count; i++)
      {
        if (index == i)
        {
          keyboardControllers[i].Activate();
          _currentKeyboardController = keyboardControllers[i];
        }
        else
        {
          keyboardControllers[i].Deactivate();
        }
      }
    }
    
    public void FireEvent(NodeskTypeEventInfo nodeskTypeEventInfo)
    {
      if (adapter == null) return;
      
      adapter.OnNodeskTyping(nodeskTypeEventInfo);
      if (nodeskTypeEventInfo.character == "Enter")
      {
        generalEvent.Invoke(GeneralEventKind.Enter);
      }
      if (nodeskTypeEventInfo.character == "BkSp")
      {
        generalEvent.Invoke(GeneralEventKind.Delete);
      }
      if (nodeskTypeEventInfo.character == "Tab")
      {
        generalEvent.Invoke(GeneralEventKind.Tab);
      }
      if (nodeskTypeEventInfo.character == "Submit")
      {
        generalEvent.Invoke(GeneralEventKind.Submit);
      }
    }

    public void ChangeVibrate()
    {
      inputHandler.ChangeVibrate();
    }

    public void SubmitVibrate()
    {
      inputHandler.SubmitVibrate();
    }
  }
}
