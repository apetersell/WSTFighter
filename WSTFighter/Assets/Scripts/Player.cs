using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WST;

[System.Serializable]
public struct AcceptedInputs
{
    public bool canJump;
    public bool canMove;
    public bool canAttack;
}

public class Player : MonoBehaviour
{
    
    [Header("Basics")]
    public int playerNum;
    public Transform visual;
    Rigidbody2D rb;
    public Transform footOrigin;
    public float footRaycastDistance;
    public int facingMod;
    int idx => playerNum - 1;
    Player otherPlayer;
    public Vector2 leftStickPos;
    float xScale;
    bool needsToReturnToNeutral;
    public AcceptedInputs acceptedInputs;

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float airDashDistance;
    public float groundDashDistance;
    public float airDashTime;
    public float groundDashTime;
    Vector2 dash_start;
    Vector2 dash_end;
    TweenID dashTween;
    bool running;
    bool dashing => dashTween.isTweening;

    [Header("Jump")]
    public float jumpForce;
    public float airJumpForce;
    public float runJumpForce;
    public int maxAirJumps;
    int airJumps;
    Vector2 jumpAngle;


    public void InitPlayer()
    {
        ToggleInputs(true, true, true);
        transform.position = MatchManager.instance.startingPositions[idx];
        otherPlayer = MatchManager.instance.OtherPlayer(playerNum);
        xScale = visual.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        TurnAround();
    }

    // Update is called once per frame
    void Update()
    {
        DebugStuff();
        UpdateStickPosition();
        DetectJump();
        if (Grounded())
        {
            if (acceptedInputs.canMove)
            {
                Movement();
                TurnAround();
            }
            airJumps = 0;
        }
       
    }

    void Movement()
    {
        if (Mathf.Abs(leftStickPos.x) > 0.4f)
        {
            Vector3 pos = transform.position;
            float moveMod = running ? (runSpeed * facingMod) * Time.deltaTime : (leftStickPos.x * walkSpeed) * Time.deltaTime;
            pos.x += moveMod;
            if (WithinDistance(pos))
            {
                transform.position = pos;
            }
        }
        else
        {
            running = false;
        }
    }

    void TurnAround()
    {
        if (!running)
        {
            Vector3 direction = otherPlayer.transform.position - transform.position;
            facingMod = direction.x >= 0 ? 1 : -1;
            Vector3 scale = visual.localScale;
            scale.x = xScale * facingMod;
            visual.localScale = scale;
        }
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
        if (JumpInput() != StickInput.Neutral)
        {
            FGInput input = new FGInput();
            input.button = Button.None;
            input.stick = JumpInput();
            input.commandInput = CommandInput.Jump;
            DoInput(input);
        }
    }

    void Jump()
    {
        if(Grounded() && !needsToReturnToNeutral)
        {
            rb.velocity = jumpAngle * jumpForce;
            needsToReturnToNeutral = true;
        }
        if (!Grounded() && airJumps < maxAirJumps && !needsToReturnToNeutral)
        {
            rb.velocity = jumpAngle * airJumpForce;
            needsToReturnToNeutral = true;
            airJumps++;
        }
        running = false;
    }

    public virtual void DoInput(FGInput input)
    {
       if (acceptedInputs.canJump)
        {
            if (input.commandInput == CommandInput.Jump)
            {
                Jump();
            }
        }
      if (acceptedInputs.canMove)
        {
            if (input.commandInput == CommandInput.FF)
            {
                if (Grounded())
                {
                    running = true;
                }
                else
                {
                    Dash(true);
                }
            }
            if (input.commandInput == CommandInput.BB)
            {
                Dash(false);
            }
        }
      if (acceptedInputs.canAttack)
        {

        }
    }

    public StickInput JumpInput()
    {
        StickInput result;
        float stickX = leftStickPos.x * facingMod;
        if (stickX > -.4f && stickX < .4f && leftStickPos.y > 0.4f)
        {
            result = StickInput.Up;
            jumpAngle = new Vector2(0, 1);
        }
        else if (stickX > .4f && leftStickPos.y >= 0.4f)
        {
            result = StickInput.UpForward;
            float force = running ? .5f : .25f; 
            jumpAngle = new Vector2(facingMod * force, 1);
        }
        else if (stickX < -.4f && leftStickPos.y >= 0.4f)
        {
            result = StickInput.UpBack;
            jumpAngle = new Vector2(facingMod * -.25f, 1);
        }
        else
        {
            needsToReturnToNeutral = false;
            result = StickInput.Neutral;
        }
        return result;
    }

    void ToggleInputs(bool jump, bool move, bool attack)
    {
        acceptedInputs.canJump = jump;
        acceptedInputs.canMove = move;
        acceptedInputs.canAttack = attack;
    }

    void Dash(bool forward)
    {
        ToggleInputs(false, false, false);
        float time = Grounded() ? groundDashTime : airDashTime;
        float distance = Grounded() ? groundDashDistance : airDashDistance;
        float mod = forward ? facingMod : -facingMod;
        dash_start = transform.position;
        dash_end = dash_start;
        dash_end.x = Mathf.Clamp(dash_start.x + (distance * mod), MatchManager.instance.xBounds.x, MatchManager.instance.xBounds.y);
        while(!WithinDistance(dash_end))
        {
            float adjustMod = forward ? -.01f : .01f;
            dash_end.x += adjustMod;
        }
        LeanTween.value(0, 1, time)
            .setOnUpdate(DashUpdate)
            .setTweenId(ref dashTween)
            .setOnComplete(DashComplete);
    }

    void DashUpdate(float t)
    {
        Vector2 pos = dash_start;
        pos.x = Mathf.Lerp(dash_start.x, dash_end.x, t);
        transform.position = pos;
    }

    void DashComplete()
    {
        ToggleInputs(true, true, true);
    }
}
