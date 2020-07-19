using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    //References
    public GameObject playerPrefab;
    public Player[] players;
    public PlayerUI[] playerUIs;

    //IG Stats
    public static float maxHP = 100;
    public static float maxTime = 100;
    public float maxPlayerDistance;

    public Vector3[] startingPositions;

    float comboCounter = 1;
    float timer;
    bool inCombo;

    public LayerMask floorMask;
    public CameraControl cam;

    private void Awake()
    {
        timer = comboCounter + 1;
       if (instance == null)
        {
            instance = this;
        }
        players = new Player[2];
        for (int i = 0; i < playerUIs.Length; i++)
        {
            playerUIs[i].InitUI();
            GameObject player = Instantiate(playerPrefab) as GameObject;
            Player playerComponent = player.GetComponent<Player>();
            playerComponent.playerNum = i + 1;
            players[i] = playerComponent;
            if (i > 0)
            {
                SpriteRenderer sprite = playerComponent.visual.gameObject.GetComponent<SpriteRenderer>();
                sprite.color = Color.red;
            }
        }
        for (int i = 0; i < players.Length; i++)
        {
            players[i].InitPlayer();
        }
        cam.updating = true;
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
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
