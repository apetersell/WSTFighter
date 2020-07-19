using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WST;

public class Player : MonoBehaviour
{
    public int playerNum;
    public Transform visual;
    Rigidbody2D rb;
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public Transform footOrigin;
    public float footRaycastDistance;
    public int facingMod;
    int idx => playerNum - 1;
    Player otherPlayer;
    public Vector2 leftStickPos;
    float xScale;
    Vector2 jumpAngle;
    bool hasJumped;

    public void InitPlayer()
    {
        transform.position = MatchManager.instance.startingPositions[idx];
        otherPlayer = MatchManager.instance.OtherPlayer(playerNum);
        xScale = visual.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        TurnAround();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugStuff();
        UpdateStickPosition();
        if (Grounded())
        {
            Movement();
            TurnAround();
        }
       
    }

    void Movement()
    {
        if (Mathf.Abs(leftStickPos.x) > 0.4f)
        {
            Vector3 pos = transform.position;
            pos.x += leftStickPos.x * walkSpeed;
            if (WithinDistance(pos))
            {
                transform.position = pos;
            }
        }
    }

    void TurnAround()
    {
        Vector3 direction = otherPlayer.transform.position - transform.position;
        facingMod = direction.x >= 0 ? 1 : -1;
        Vector3 scale = visual.localScale;
        scale.x = xScale * facingMod;
        visual.localScale = scale;
    }

    bool Grounded()
    {
        RaycastHit2D below = Physics2D.Raycast(footOrigin.transform.position, Vector2.down, footRaycastDistance, MatchManager.instance.floorMask);
        if (below.collider != null)
        {
            return below.transform.gameObject.tag == "Floor";
        }
        else
        {
            return false;
        }
    }

    void UpdateStickPosition()
    {
        leftStickPos = new Vector2(Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_X")), -Input.GetAxis(InputManager.InputString(playerNum, "LeftStick_Y")));
    }

    void DebugStuff()
    {
        Vector3 footEndPoint = new Vector3(footOrigin.position.x, footOrigin.position.y - footRaycastDistance, footOrigin.position.z);
        Debug.DrawLine(footOrigin.transform.position, footEndPoint, Color.green);
    }

    bool WithinDistance (Vector3 pos)
    {
        return Mathf.Abs(Vector3.Distance(pos, otherPlayer.transform.position)) < MatchManager.instance.maxPlayerDistance; 
    }

    void DetectJump()
    {
        if (Grounded() && !hasJumped)
        {
            if (JumpInput() != StickInput.Neutral)
            {
                FGInput input = new FGInput();
                input.button = Button.None;
                input.stick = JumpInput();
                input.commandInput = CommandInput.Jump;
                DoInput(input);
            }
        }
        if (Grounded() && hasJumped)
        {
            if (JumpInput() == StickInput.Neutral)
            {
                hasJumped = false;
            }
        }
    }

    void Jump()
    {
        if(Grounded() && !hasJumped)
        {
            rb.velocity = jumpAngle * jumpForce;
            hasJumped = true;
        }
    }

    public virtual void DoInput(FGInput input)
    {
       if (input.commandInput == CommandInput.Jump)
        {
            Jump();
        }
    }

    public StickInput JumpInput()
    {
        StickInput result;
        if (leftStickPos.x > -.5f && leftStickPos.x < .5f && leftStickPos.y > 0.05f)
        {
            result = StickInput.Up;
            jumpAngle = new Vector2(0, 1);
        }
        else if (leftStickPos.x > .5f && leftStickPos.y >= 0.05f)
        {
            result = StickInput.UpForward;
            jumpAngle = new Vector2(facingMod * .5f, 1);
        }
        else if (leftStickPos.x < -.5f && leftStickPos.y >= 0.05f)
        {
            result = StickInput.UpBack;
            jumpAngle = new Vector2((facingMod * .5f) * -1, 1);
        }
        else
        {
            result = StickInput.Neutral;
        }
        return result;
    }
}
