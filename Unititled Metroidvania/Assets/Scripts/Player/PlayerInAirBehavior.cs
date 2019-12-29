using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirBehavior : LogicalStateMachineBehaviour
{

    [SerializeField]
    float playerAirSpeed = 0;

    [SerializeField]
    GameObject ceilingCheck;
    Collider2D ceilingColliderCheck;

    [SerializeField]
    GameObject groundCheck;
    Collider2D groundColliderCheck;

    [SerializeField]
    GameObject wallLeftCheck;
    Collider2D wallLeftColliderCheck;
    [SerializeField]
    GameObject wallRightCheck;
    Collider2D wallRightColliderCheck;

    bool leftWall = false;
    bool rightWall = false;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    LayerMask ceilingLayer;

    Rigidbody2D thisRigidBody2D;
    Vector2 airVelocity = new Vector2();

    float jumpInput = 0.0f;

    float lastJumpInput = 0.0f;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    protected override void OnStateEntered()
    {
        thisRigidBody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("GroundCheck");
        ceilingCheck = GameObject.Find("CeilingCheck");
        groundColliderCheck = groundCheck.GetComponent<Collider2D>();
        ceilingColliderCheck = ceilingCheck.GetComponent<Collider2D>();

        wallLeftCheck = GameObject.Find("WallCheckLeft");
        wallRightCheck = GameObject.Find("WallCheckRight");
        wallLeftColliderCheck = wallLeftCheck.GetComponent<Collider2D>();
        wallRightColliderCheck = wallRightCheck.GetComponent<Collider2D>();
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    void OnStateUpdate()
    {
        GroundCheck();
        CeilingCheck();
        WallCheck();

        airVelocity = thisRigidBody2D.velocity;
        airVelocity.x *= playerAirSpeed;
        thisRigidBody2D.velocity = airVelocity;


        lastJumpInput = jumpInput;
        jumpInput = Input.GetAxisRaw("Jump");

        if (jumpInput != 0.0f && lastJumpInput != 1.0f)
        {
            this.Animator.SetBool("FastFallInput", true);

        }
        else
        {
            this.Animator.SetBool("FastFallInput", false);

        }
    }

    void GroundCheck()
    {
        if (groundColliderCheck.IsTouchingLayers(groundLayer))
        {
            this.Animator.SetBool("InAir", false);
            this.Animator.SetFloat("HorizontalInput", 0.0f);
            //falling = false;
            //fastFall = false;

        }
        else
        {
            this.Animator.SetBool("InAir", true);
        }


    }

    void CeilingCheck()
    {
        if (ceilingColliderCheck.IsTouchingLayers(ceilingLayer))
        {
            this.Animator.SetBool("Falling", true);
        }
        else
        {
            this.Animator.SetBool("Falling", false);
        }
    }


    void WallCheck()
    {
        if (!GameManagerScript.Instance.CheckState("WallJump"))
            return;

        if (wallLeftColliderCheck.IsTouchingLayers(groundLayer) && this.Animator.GetFloat("HorizontalInput") < -0.1f)
        {
            this.Animator.SetBool("LeftWallTouching", true);
            
        }
        else
        {
            this.Animator.SetBool("LeftWallTouching", false);
        }

        if (wallRightColliderCheck.IsTouchingLayers(groundLayer) && this.Animator.GetFloat("HorizontalInput") > 0.1f)
        {
            this.Animator.SetBool("RightWallTouching", true);
            
        }
        else
        {
            this.Animator.SetBool("RightWallTouching", false);
            
        }
    }
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
