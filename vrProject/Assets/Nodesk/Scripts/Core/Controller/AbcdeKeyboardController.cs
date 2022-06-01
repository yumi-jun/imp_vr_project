/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace Nodesk.Scripts.Core.Controller
{
  public class AbcdeKeyboardController : AbstractKeboardController
  {

    [SerializeField] private List<GroupController> _groupControllers;
    [SerializeField] private SpriteRenderer _circle;
    [SerializeField] protected NodeskManager _nodeskManager;
    
    private Vector2 _previousTouchPos2;
    private GroupController _selectedtGroupController;
    private int _phase = 1;

    [SerializeField] private float neutralBufferTime = 0.016f;
    [SerializeField] private float neutralStickTilt = 0.1f;
    [SerializeField] private float submitStickReturnTime = 0.016f;

    private float _lastSelectTime = -1;
    private int _lastSelectIndex = -1;

    void Start()
    {
      foreach (var panelGroupView in _groupControllers)
      {
        panelGroupView.UnHover();
      }

      _circle.transform.localScale = Vector3.one * 0.6f; 
    }

    public override void ControllerInputOnTouchEvent(InputData inputData)
    {
      var action = inputData.Action;
      var position = inputData.Position;

      if (_phase == 1)
      {
        switch (action)
        {
          case InputAction.StickBeginMove:
            TouchDownPhase1(position);
            break;
          case InputAction.StickMoving:
            TouchMovePhase1(position);
            break;
          case InputAction.StickEndMove:
            TouchUpPhase1(position);
            break;
          case InputAction.Nothing:
            break;
        }
      }
    
    
      if (_phase == 2)
      {
        switch (action)
        {
          case InputAction.StickBeginMove:
            TouchDownPhase2(position);
            break;
          case InputAction.StickMoving:
            TouchMovePhase2(position);
            break;
          case InputAction.StickEndMove:
            TouchUpPhase2(position);
            break;
          case InputAction.Submit:
            SubmitPhase2();
            break;
          case InputAction.Nothing:
            break;
        }
      }
    }
  
    private void TouchDownPhase1(Vector2 position)
    {
      _circle.transform.localScale = Vector3.one * 1f;
    }

    private void TouchMovePhase1(Vector2 position)
    {
      var angle = -Vector2.SignedAngle(Vector2.up, position);
      var index = select(angle, position.magnitude);
      
      if (position.magnitude >= neutralStickTilt)
      {
        Phase1EndAnimation(index, position.magnitude, position);
        _nodeskManager.ChangeVibrate();
        _selectedtGroupController = GetGroupController(index);
        _phase = 2;
        Phase1MoveAnimation(index, position.magnitude, position);
      }
      else
      {
        Phase1MoveAnimation(index, position.magnitude, position);      
      }
    }


    private void TouchUpPhase1(Vector3 position)
    {
      foreach (var circleGroup in _groupControllers)
      {
        circleGroup.UnHover();
      }
      _circle.transform.localScale = Vector3.one * 0.6f;
    }

  
    private void TouchDownPhase2(Vector2 position)
    {
    }

    private bool _nowNeutral = false;
    private float _beginNeutralTime = 0.0f;
    
    private bool ReturnNeutral(int index)
    {
      var isNeutral = index == -1;
      if (!isNeutral)
      {
        return false;
      }

      if (!_nowNeutral)
      {
        _nowNeutral = true;
        _beginNeutralTime = Time.time;

        return false;
      }
      
      var deltaNeutralTime = Time.time - _beginNeutralTime;
      if (deltaNeutralTime > neutralBufferTime)
      {
        _nowNeutral = false;
        return true;
      }

      return false;
    }

    private int _prevIndex = -1;
    private void TouchMovePhase2(Vector2 position)
    {
      _previousTouchPos2 = position;
      var rotate = Quaternion.Euler(0, 0, (45 * (_selectedtGroupController.Index-1))) * position;
      var angle = -Vector2.SignedAngle(Vector2.up, rotate);

      var index = _selectedtGroupController.Index % 2 == 1
        ? select1(angle, position.magnitude, _selectedtGroupController.Index)
        : select2(angle, position.magnitude, _selectedtGroupController.Index);

      if (index != -1)
      {
        _lastSelectIndex = index;
      }
      
      if (!ReturnNeutral(index))
      {
        Phase2MoveAnimation(index, position.magnitude, position);
        _lastSelectTime = Time.time;
      }
      else
      {
        foreach (var circleGroup in _groupControllers)
        {
          circleGroup.UnHoverElements();
          circleGroup.UnHover();
          circleGroup.Activate();
        }
        _nowNeutral = false;
        _phase = 1;
      }
    }

    private void TouchUpPhase2(Vector2 position)
    {
      var diffTime = Time.time - _lastSelectTime;
      if (_lastSelectTime > 0 && diffTime < submitStickReturnTime)
      {
        SubmitPhase2();
      }
      
      foreach (var circleGroup in _groupControllers)
      {
        circleGroup.UnHoverElements();
        circleGroup.UnHover();
        circleGroup.Activate();
      }

      _phase = 1;
      _circle.transform.localScale = Vector3.one * 0.6f;
      _nowNeutral = false;
    }

    public override void Reset()
    {
      foreach (var circleGroup in _groupControllers)
      {
        circleGroup.UnHoverElements();
        circleGroup.UnHover();
        circleGroup.Activate();
      }

      _phase = 1;
      _circle.transform.localPosition = Vector3.zero;
      _circle.transform.localScale = Vector3.one * 0.6f;
      _nowNeutral = false;
    }

    private void SubmitPhase2()
    {
      _nodeskManager.SubmitVibrate();
      _circle.transform.localPosition = Vector3.zero;
      
      var index = _lastSelectIndex;
      string key = _selectedtGroupController.GetKey(index);
      _nodeskManager.FireEvent(new NodeskTypeEventInfo()
      {
        kind = TypeEventKind.Type,
        character = key,
      });
    }

    private void Phase1MoveAnimation(int groupIndex, float distanceFromCenter, Vector2 touchPosition)
    {
      var degree = correction(touchPosition);
      var pos = new Vector2(Mathf.Sin(degree * Mathf.Deg2Rad), Mathf.Cos(degree * Mathf.Deg2Rad));
      _circle.transform.localPosition = new Vector3(
                                          pos.x,
                                          pos.y,
                                          0
                                        ).normalized * (distanceFromCenter * 5.0f);
    }

    private float correction(Vector2 touchPosition)
    {
      var angle = -Vector2.SignedAngle(Vector2.up, touchPosition);
      
      if (-22.5f <= angle  && angle <= 22.5f)
      {
        return 0;
      }
      if (22.5f <= angle  && angle <= 67.5f)
      {
        return 45;
      }
      if (67.5f <= angle  && angle <= 112.5f)
      {
        return 90;
      }
      if (112.5f <= angle  && angle <= 157.5f)
      {
        return 135;
      }
      if (157.5f <= angle  && angle <= 180f)
      {
        return 180;
      }

      if (-22.5f >= angle && angle >= -67.5)
      {
        return -45;
      }
      if (-67.5f >= angle && angle >= -112.5)
      {
        return -90;
      }
      if (-112.5f >= angle && angle >= -157.5)
      {
        return -135;
      }

      if (-157.5f >= angle && angle >= -180)
      {
        return -180;
      }
      
      return 1;
    }
  
  
    private void Phase1EndAnimation(int groupIndex, float distanceFromCenter, Vector2 touchPosition)
    {
      _circle.transform.localScale = Vector3.one * 0.6f;
      foreach (var groupController in _groupControllers)
      {
        if(groupController.Index == groupIndex)
        {
          groupController.transform.localScale = Vector3.one * 1.2f;
          groupController.Activate();
        }
        else
        {
          groupController.transform.localScale = Vector3.one;
          groupController.Deactivate();
        }
      }

    }
  
    private void Phase2MoveAnimation(int elementIndex, float distanceFromCenter, Vector2 touchPosition)
    {
      var elements = _selectedtGroupController.GetElements();
      _selectedtGroupController.transform.localPosition = _selectedtGroupController.InitialPosition +
                                                        (new Vector3(touchPosition.x, touchPosition.y, -0f).normalized *
                                                         (distanceFromCenter / 2.0f));
      _selectedtGroupController.transform.localPosition += new Vector3(0,0,-0.5f);

    
      if (_prevIndex != elementIndex)
      {
        _nodeskManager.ChangeVibrate();
        _prevIndex = elementIndex;
      } 
    
      foreach (var elementController in elements)
      {
        if(elementController.Index == elementIndex)
        {
          if (elementController.IsCenter)
          {
            elementController.transform.localScale = Vector3.one * 1.2f;  
          }
          else
          {
            elementController.transform.localScale = Vector3.one * 1.0f;
          }
      
        }
        else
        {
          if (elementController.IsCenter)
          {
            elementController.transform.localScale = Vector3.one * 1f;  
          }
          else
          {
            elementController.transform.localScale = Vector3.one * 0.5f;
          }
        }
      }
    
    }

    private GroupController GetGroupController(int index)
    {
      foreach (var circleGroup in _groupControllers)
      {
        if(circleGroup.Index == index)
        {
          return circleGroup;
        }
      }

      return null;
    }
  
    int select1(double angle, double distance, int groupIndex)
    {
      if (distance <= neutralStickTilt)
      {
        return -1;
      }

      if (-36f <= angle  && angle <= 36f)
      {
        return 1;
      }
      if (36f <= angle  && angle <= 108f)
      {
        return 3;
      }
      if (108f <= angle  && angle <= 180f)
      {
        return 4;
      }
      
      if (-36f >= angle && angle >= -108)
      {
        return 2;
      }
      
      if (-108f >= angle && angle >= -180)
      {
        return 5;
      }
      
      return 1;
    }

  
    int select2(double angle, double distance, int groupIndex)
    {
      if (distance <= neutralStickTilt)
      {
        return -1;
      }

      if (-45f <= angle  && angle <= 45f)
      {
        return 1;
      }
      if (45f <= angle  && angle <= 135f)
      {
        return 2;
      }
      if (135f <= angle  && angle <= 180f)
      {
        return 3;
      }
      
      if (-45f >= angle && angle >= -135f)
      {
        return 4;
      }
      
      if (-135f >= angle && angle >= -180)
      {
        return 3;
      }
      
      return 1;
    }


  
    int select(double angle, double distance)
    {
    
      if (-22.5f <= angle  && angle <= 22.5f)
      {
        return 1;
      }
      if (22.5f < angle && angle <= 67.5f)
      {
        return 2;
      }
      if (67.5f < angle && angle <= 112.5f)
      {
        return 3;
      }
      if (112.5f < angle && angle <= 157.5f)
      {
        return 4;
      }
    
      if (157.5f <= angle || angle <= -157.5f)
      {
        return 5;
      }
    
      if (-157.5f < angle && angle <= -112.5f)
      {
        return 6;
      }
    
      if (-112.5f < angle && angle <= -67.5f)
      {
        return 7;
      }

      if (-67.5f < angle && angle <= -22.5f)
      {
        return 8;
      }

      return -1;
    }
  }
}
