using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WST
{
    public enum CommandInput
    {
        None,
        QCF,
        QCB,
        DPF,
        DPB,
    }

    public enum Button
    {
        A,
        B,
        X,
        Y,
    }

    public enum StickInput
    {
        Neutral,
        Up,
        Down,
        Forward,
        Back,
        DownForward,
        DownBack,
        UpForward,
        UpBack,

    }

    public struct FGInput
    {
        public StickInput stick;
        public CommandInput commandInput;
        public Button button;

    }

    public class InputHandler : MonoBehaviour
    {
        public int playerNum;
        public int directionMod = 1;
        Vector2 stickPos;
        public List<StickInput> stickInputs = new List<StickInput>();
        StickInput latestStickInput;
        StickInput previousStickInput;
        public float stickInputWindow;
        float stickInputTimer;

        void Update()
        {
            stickPos = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_X")), -Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_Y"))); 
            UpdateStickInputs();
        }

        public StickInput Stick ()
        {
            float stickX = stickPos.x * directionMod;
            StickInput result = StickInput.Neutral;
            if (stickX > .5f && stickPos.y < 0.5f && stickPos.y > -0.5f)
            {
                result = StickInput.Forward;
            }
            else if (stickX > .5f && stickPos.y > 0.5f)
            {
                result = StickInput.UpForward;
            }
            else if (stickX > .5f && stickPos.y < -0.5f)
            {
                result = StickInput.DownForward;
            }
            else if (stickX < -.5f && stickPos.y < 0.5f && stickPos.y > - 0.5f)
            {
                result = StickInput.Back;
            }
            else if (stickX < -.5f && stickPos.y > 0.5f)
            {
                result = StickInput.UpBack;
            }
            else if (stickX < -.5f && stickPos.y < - 0.5f)
            {
                result = StickInput.DownBack;
            }
            else if (stickX > -.5f && stickX < .5f && stickPos.y > 0.5f)
            {
                result = StickInput.Up;
            }
            else if (stickX > -.5f && stickX < .5f && stickPos.y < -.5f)
            {
                result = StickInput.Down;
            }
            else
            {
                result = StickInput.Neutral;
            }
            return result;
        }

        public CommandInput Command ()
        {
            CommandInput result = CommandInput.None;
            if (stickInputs.Count == 3)
            {
                if (stickInputs[0] == StickInput.Down && stickInputs[1] == StickInput.DownForward && stickInputs[2] == StickInput.Forward)
                {
                    result = CommandInput.QCF;
                }
                else if (stickInputs[0] == StickInput.Down && stickInputs[1] == StickInput.DownBack && stickInputs[2] == StickInput.Back)
                {
                    result = CommandInput.QCB;
                }
            }
            return result;
        }

        void UpdateStickInputs()
        {
            if (previousStickInput != Stick())
            {
                previousStickInput = latestStickInput;
            }
            latestStickInput = Stick();
            if (latestStickInput == previousStickInput)
            {
                if (latestStickInput == StickInput.Neutral)
                {
                    stickInputTimer = 0;
                    stickInputs.Clear();
                }
                stickInputTimer += Time.deltaTime;
                if (stickInputTimer >= stickInputWindow)
                {
                    RemoveOldestInput();
                }
            }
            if (stickInputs.Count > 0)
            {
                StickInput lastInput = stickInputs[stickInputs.Count - 1];
                if (lastInput == latestStickInput)
                {
                    if (previousStickInput != StickInput.Neutral)
                    {
                        return;
                    }
                }
            }
            if (latestStickInput != StickInput.Neutral)
            {
                stickInputTimer = 0;
                stickInputs.Add(latestStickInput);
            }
        }

        void RemoveOldestInput()
        {
            stickInputTimer = 0;
            if (stickInputs.Count > 1)
            {
                List<StickInput> dupeList = new List<StickInput>();
                for (int i = 0; i < stickInputs.Count; i++)
                {
                    dupeList.Add(stickInputs[i]);
                }
                stickInputs.Clear();
                for (int i = 0; i < dupeList.Count; i++)
                {
                    if (i > 0)
                    {
                        stickInputs.Add(dupeList[i]);
                    }
                }
            }
            else
            {
                stickInputs.Clear();
            }
        }
    }
}
