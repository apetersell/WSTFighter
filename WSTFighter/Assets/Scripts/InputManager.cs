using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WST
{
    public class InputManager : MonoBehaviour
    {
        public static string InputString(int playerNum, string input)
        {
            return string.Format("P{0}_{1}", playerNum, input);
        }
    }
}
