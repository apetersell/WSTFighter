using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    //References
    public Player[] players;
    public PlayerUI[] playerUIs;

    //IG Stats
    public static float maxHP = 100;
    public static float maxTime = 100;

    float comboCounter = 1;
    float timer;
    bool inCombo;

    private void Awake()
    {
        timer = comboCounter + 1;
       if (instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < playerUIs.Length; i++)
        {
            playerUIs[i].InitUI();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1, 4, 1);
            timer = 0;
            inCombo = true;
            
        }
        if (inCombo)
        {
            timer += Time.deltaTime;
            if (timer >= comboCounter)
            {
                playerUIs[0].ResetCombo();
                inCombo = false;
            }
        }
    }

    public Player OtherPlayer(int playerNum)
    {
        return playerNum == 1 ? players[1] : players[0];
    }

    public void TakeDamage(int playerNum, float damage, float scaling)
    {
        int idx = playerNum - 1;
        playerUIs[idx].TakeDamage(damage, scaling);
    }
}
