using System;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Net;
using UnityEngine;
using VContainer.Unity;

public class PlayerController : IInitializable, ITickable
{
    private Dictionary<Direction, bool> buttons = new Dictionary<Direction, bool>();

    private Vector2f curDir = Vector2f.Zero;

    private UIGame uiGame;
    private UnitSystem unitSystem;

    public PlayerController(UIGame uiGame, UnitSystem unitSystem)
    {
        this.uiGame = uiGame;
        this.unitSystem = unitSystem;
    }

    public void Initialize()
    {
        buttons.Add(Direction.up, false);
        buttons.Add(Direction.down, false);
        buttons.Add(Direction.left, false);
        buttons.Add(Direction.right, false);
    }

    public void Tick()
    {
        if (unitSystem.PlayerTransform == null)
            return;


        if (!uiGame.IsFocus())
        {
            UpdateControl(KeyCode.W, KeyCode.UpArrow, Direction.up);
            UpdateControl(KeyCode.S, KeyCode.DownArrow, Direction.down);
            UpdateControl(KeyCode.A, KeyCode.LeftArrow, Direction.left);
            UpdateControl(KeyCode.D, KeyCode.RightArrow, Direction.right);
        }
        else
        {
            ResetControl(Direction.up);
            ResetControl(Direction.down);
            ResetControl(Direction.left);
            ResetControl(Direction.right);
        }

        CalcDirection();
    }

    private void CalcDirection()
    {
        Vector2f dir = Vector2f.Zero;

        foreach (var b in buttons)
        {
            if (b.Value)
            {
                switch (b.Key)
                {
                    case Direction.up:
                        dir += Vector2f.Up;
                        break;
                    case Direction.down:
                        dir += Vector2f.Down;
                        break;
                    case Direction.right:
                        dir += Vector2f.Right;
                        break;
                    case Direction.left:
                        dir += Vector2f.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        var camAngle = Camera.main.transform.eulerAngles.y;
        dir.Rotate(-camAngle);

        if ((curDir - dir).sqrMagnitude > 0)
        {
            curDir = dir;
            Data.Instance.SendMsg(new MsgClient(MsgClintType.inputKey, new MsgClientInputKey(curDir)));
        }
    }


    private void ResetControl(Direction key)
    {
        if (buttons[key])
        {
            buttons[key] = false;
        }
    }

    private void UpdateControl(KeyCode keyCode1, KeyCode keyCode2, Direction key)
    {
        bool isOn = Input.GetKey(keyCode1) || Input.GetKey(keyCode2);
        if (isOn != buttons[key])
        {
            buttons[key] = isOn;
        }
    }

    
}
