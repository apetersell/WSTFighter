﻿using System.Collections;
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
        FF,
        UU,
        DD,
        BB,
        ChargeB,
        ChargeF,
        ChargeD,
        ChargeU,
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
        public float chargeTime;
        float chargeTimer;
        float stickInputTimer;
        bool charged => chargeTimer >= chargeTime;
        public CommandInput command;

        void Update()
        {
            stickPos = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_X")), -Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_Y"))); 
            UpdateStickInputs();
            if (Input.GetButtonDown(InputManager.InputString(playerNum, "AButton")))
            {
                command = Command();
            }
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
            else if (stickX > .5f && stickPos.y < -0.2f)
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
            List<StickInput> latestInputs = new List<StickInput>();
            CommandInput result = CommandInput.None;
            for (int i = 0; i < 3; i++)
            {
                int searchIdx = stickInputs.Count - (3 - i);
                if (searchIdx >= 0)
                {
                    latestInputs.Add(stickInputs[searchIdx]);
                }
            }
            if (latestInputs.Count > 1)
            {
                if (latestInputs.Count == 3)
                {
                    if (latestInputs[0] == StickInput.Down &&
                   latestInputs[1] == StickInput.DownForward &&
                   latestInputs[2] == StickInput.Forward)
                    {
                        result = CommandInput.QCF;
                    }
                    else if (latestInputs[0] == StickInput.Down &&
                        latestInputs[1] == StickInput.DownBack &&
                        latestInputs[2] == StickInput.Back)
                    {
                        result = CommandInput.QCB;
                    }
                    else if (latestInputs[0] == StickInput.Forward &&
                        latestInputs[1] == StickInput.Down &&
                        latestInputs[2] == StickInput.DownForward)
                    {
                        result = CommandInput.DPF;
                    }
                    else if (latestInputs[0] == StickInput.Back &&
                        latestInputs[1] == StickInput.Down &&
                        latestInputs[2] == StickInput.DownBack)
                    {
                        result = CommandInput.DPB;
                    }
                }
               else if (latestInputs.Count > 1)
                {
                    StickInput first = latestInputs[latestInputs.Count - 2];
                    StickInput second = latestInputs[latestInputs.Count - 1];
                    if (first == second)
                    {
                        switch(first)
                        {
                            case StickInput.Up:
                                result = CommandInput.UU;
                                break;
                            case StickInput.Down:
                                result = CommandInput.DD;
                                break;
                            case StickInput.Forward:
                                result = CommandInput.FF;
                                break;
                            case StickInput.Back:
                                result = CommandInput.BB;
                                break;
                        }
                    }
                }
                else if (latestInputs.Count == 1  && charged)
                {
                    switch(latestInputs[0])
                    {
                        case StickInput.Up:
                            result = CommandInput.ChargeU;
                            break;
                        case StickInput.Down:
                            result = CommandInput.ChargeD;
                            break;
                        case StickInput.Forward:
                            result = CommandInput.ChargeF;
                            break;
                        case StickInput.Back:
                            result = CommandInput.ChargeB;
                            break;
                    }
                }
            }
            Debug.Log("COMMAND: " + result);
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
                stickInputTimer += Time.deltaTime;
                chargeTimer += Time.deltaTime;
                if (stickInputTimer >= stickInputWindow)
                {
                    if (latestStickInput == StickInput.Neutral)
                    {
                        stickInputTimer = 0;
                        stickInputs.Clear();
                    }
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
                AddStickInput();
            }
        }

        void AddStickInput()
        {
            stickInputTimer = 0;
            bool tryingToDP = latestStickInput == StickInput.Down &&
                stickInputs.Count >= 2 &&
               (stickInputs[stickInputs.Count - 2] == StickInput.Forward || stickInputs[stickInputs.Count - 2] == StickInput.Back) &&
                (stickInputs[stickInputs.Count - 1] == StickInput.DownForward || stickInputs[stickInputs.Count - 2] == StickInput.DownBack);
            if (tryingToDP)
            {
                List<StickInput> dupeList = new List<StickInput>();
                for (int i = 0; i < stickInputs.Count; i++)
                {
                    dupeList.Add(stickInputs[i]);
                }
                stickInputs.Clear();
                for (int i = 0; i < dupeList.Count; i++)
                {
                    if (i < dupeList.Count - 1)
                    {
                        stickInputs.Add(dupeList[i]);
                    }
                }
            }
            stickInputs.Add(latestStickInput);
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
