using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    Transform thisTransform;

    GameObject thisGameObject;

    Rigidbody2D thisRigidBody2D;

    //Input Variables
    float xInput = 0.0f;
    float yInput = 0.0f;
    float jumpInput = 0.0f;
    float lastJumpInput = 0.0f;
    float slowWalkInput = 0.0f;
    

    //Game Checks
    bool isGrounded = false;
    bool falling = false;
    bool fastFall = false;
    bool touchedCeil = false;

    //Movement Vars
    float jumpedY;

    //Editor Variables
    [Header ("Movement")]
    [SerializeField]
    float playerWalkSpeed = 0;

    [SerializeField]
    float playerAirSpeed = 0;

    [SerializeField]
    float playerSlowWalkSpeed = 0;

    [SerializeField]
    float jumpHeightLimit = 5f;

    [SerializeField]
    float jumpInitialVelocity;

    [SerializeField]
    float jumpHoldVelocity;

    [SerializeField]
    int shortHopFrameLength;
    int shortHopCounter;

    [SerializeField]
    float fastFallSpeed;

    [SerializeField]
    int fastFallFrameLength;

    [SerializeField]
    GameObject groundCheck;
    Collider2D groundColliderCheck;
    [SerializeField]
    GameObject ceilingCheck;
    Collider2D ceilingColliderCheck;

    [SerializeField]
    LayerMask groundLayer;

    [HideInInspector]
   public LevelManagerScript currentLevelScript;

    LevelTransition levelTransScript;

    [HideInInspector]
    public bool canControl = true;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = transform;
        thisGameObject = gameObject;
        thisRigidBody2D = thisGameObject.GetComponent<Rigidbody2D>();
        groundColliderCheck = groundCheck.GetComponent<Collider2D>();
        ceilingColliderCheck = ceilingCheck.GetComponent<Collider2D>();
        levelTransScript = GameObject.Find("LevelTransition").GetComponent<LevelTransition>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
        {
            Transition();
        }
        else
        {
            GetInput();
            GroundCheck();
            CeilingCheck();
            FastFall();
            Move();
            Jump();
        }
    }

    void Transition()
    {
        thisRigidBody2D.velocity = new Vector2(0.0f, thisRigidBody2D.velocity.y);
    }

    void GroundCheck()
    {
        if(groundColliderCheck.IsTouchingLayers(groundLayer))
        {
            isGrounded = true;
            falling = false;
            fastFall = false;

        }
        else
        {
            isGrounded = false;
        }


    }

    void CeilingCheck()
    {
        if(ceilingColliderCheck.IsTouchingLayers(groundLayer))
        {
            touchedCeil = true;
        }
        else
        {
            touchedCeil = false;    
        }
    }

    void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        slowWalkInput = Input.GetAxisRaw("SlowWalk");
        lastJumpInput = jumpInput;
        jumpInput = Input.GetAxisRaw("Jump");
    }

    void Move()
    {
        if (isGrounded)
        {
            if (slowWalkInput != 1.0f)
            {
                if (fastFall)
                {
                    thisRigidBody2D.velocity = new Vector2(xInput * playerWalkSpeed, thisRigidBody2D.velocity.y + fastFallSpeed);
                }
                else
                {
                    thisRigidBody2D.velocity = new Vector2(xInput * playerWalkSpeed, thisRigidBody2D.velocity.y);
                }
            }
            else
            {
                thisRigidBody2D.velocity = new Vector2(xInput * playerWalkSpeed * playerSlowWalkSpeed, thisRigidBody2D.velocity.y);
            }
        }
        else
        {
            if (slowWalkInput != 1.0f)
            {
                if (fastFall)
                {
                    thisRigidBody2D.velocity = new Vector2(xInput * playerAirSpeed, thisRigidBody2D.velocity.y + fastFallSpeed);
                }
                else
                {
                    thisRigidBody2D.velocity = new Vector2(xInput * playerAirSpeed, thisRigidBody2D.velocity.y);
                }
            }
            else
            {
                thisRigidBody2D.velocity = new Vector2(xInput * playerAirSpeed * playerSlowWalkSpeed, thisRigidBody2D.velocity.y);
            }
        }
    }


    void Jump()
    {

        if (lastJumpInput != 1.0)
        {
            falling = true;
        }

        if (isGrounded && jumpInput != 0.0f && !falling)
        {
            shortHopCounter = 0;
            jumpedY = thisTransform.position.y;
            thisRigidBody2D.velocity = new Vector2(thisRigidBody2D.velocity.x, jumpInitialVelocity);
            falling = false;
            shortHopCounter++;
        }
        else if (!isGrounded && lastJumpInput != 0.0f && !falling)
        {
            if (thisTransform.position.y - jumpedY >= jumpHeightLimit || touchedCeil)
            {
                falling = true;
                
            }
            else
            {
                shortHopCounter++;
                if(shortHopCounter > shortHopFrameLength)
                { 
                    thisRigidBody2D.velocity = new Vector2(thisRigidBody2D.velocity.x, jumpHoldVelocity);
                }
                falling = false;
            }
            
        }

    }

    void FastFall()
    {

        if(yInput < -0.1f && !isGrounded && jumpInput != 0.0f && lastJumpInput != 1.0f)
        {
            fastFall = true;
        }
        else
        {

        }
    }

   public void SetLevel(LevelManagerScript lv)
    {
        currentLevelScript = lv;
    }

    public void LevelTransition()
    {
        levelTransScript.Transition();
    }

}
