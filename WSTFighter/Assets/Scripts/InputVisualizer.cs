using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WST;

public class InputVisualizer : MonoBehaviour
{
    public int playerNum;
    public Image AButton;
    public Image BButton;
    public Image XButton;
    public Image YButton;
    public Image BackButton;
    public Image StartButton;
    public Image RightBumper;
    public Image LeftBumper;
    public Image DPad_Up;
    public Image DPad_Down;
    public Image DPad_Right;
    public Image DPad_Left;
    public Image RightTrigger;
    public Image LeftTrigger;
    public Image RightJoystick;
    public Image LeftJoystick;
    public Text commandText;
    Vector2 leftStickStart;
    Vector2 rightStickStart;
    public float stickDistance;
    Vector2 leftStickPos;
    Vector2 rightStickPos;

    public Color inactiveColor;

    private void Awake()
    {
        rightStickStart = RightJoystick.transform.localPosition;
        leftStickStart = LeftJoystick.transform.localPosition;
    }

    void Update()
    {
        AButton.color = Input.GetButton(InputManager.InputString(playerNum, "AButton")) ? Color.red : inactiveColor;
        BButton.color = Input.GetButton(InputManager.InputString(playerNum, "BButton")) ? Color.red : inactiveColor;
        XButton.color = Input.GetButton(InputManager.InputString(playerNum, "XButton")) ? Color.red : inactiveColor;
        YButton.color = Input.GetButton(InputManager.InputString(playerNum, "YButton")) ? Color.red : inactiveColor;
        BackButton.color = Input.GetButton(InputManager.InputString(playerNum, "BackButton")) ? Color.red : inactiveColor;
        StartButton.color = Input.GetButton(InputManager.InputString(playerNum, "StartButton")) ? Color.red : inactiveColor;
        LeftBumper.color = Input.GetButton(InputManager.InputString(playerNum, "LeftBumper")) ? Color.red : inactiveColor;
        RightBumper.color = Input.GetButton(InputManager.InputString(playerNum, "RightBumper")) ? Color.red : inactiveColor;
        LeftJoystick.color = Input.GetButton(InputManager.InputString(playerNum, "LeftStickClick")) ? Color.red : inactiveColor;
        RightJoystick.color = Input.GetButton(InputManager.InputString(playerNum, "RightStickClick")) ? Color.red : inactiveColor;
        LeftTrigger.color = Color.Lerp(inactiveColor, Color.red, Input.GetAxis(InputManager.InputString(playerNum, "LeftTrigger")));
        RightTrigger.color = Color.Lerp(inactiveColor, Color.red, Input.GetAxis(InputManager.InputString(playerNum, "RightTrigger")));
        rightStickPos = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "RightStick_X")), -Input.GetAxis(InputManager.InputString(playerNum, "RightStick_Y")));
        leftStickPos = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_X")), -Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_Y")));
        LeftJoystick.transform.localPosition = leftStickStart + (leftStickPos * stickDistance);
        RightJoystick.transform.localPosition = rightStickStart + (rightStickPos * stickDistance);
        Vector2 dPadInput = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "DPad_X")), Input.GetAxis(InputManager.InputString(playerNum, "DPad_Y")));
        DPad_Up.color = dPadInput.y > .5f ? Color.red : inactiveColor;
        DPad_Down.color = dPadInput.y < -.5f ? Color.red : inactiveColor;
        DPad_Right.color = dPadInput.x > .5f ? Color.red : inactiveColor;
        DPad_Left.color = dPadInput.x < -.5f ? Color.red : inactiveColor;
    }

    public void DoInput(FGInput input)
    {
        string direction = "";
        string button = input.button.ToString();
        if (input.commandInput != CommandInput.None)
        {
            direction = input.commandInput.ToString();
        }
        else
        {
            if (input.stick != StickInput.Neutral)
            {
                direction = input.stick.ToString();
            }
        }
        commandText.text = string.Format("{0} {1}", direction, button);
        
    }
}
