using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHorizontalInputBehavior : LogicalStateMachineBehaviour
{


    float xInput = 0.0f;
    float yInput = 0.0f;
    float lastSlowWalkInput = 0.0f;
    float slowWalkInput = 0.0f;
    float jumpInput = 0.0f;

    float lastJumpInput = 0.0f;

    float attackInput = 0.0f;
    float lastAttackInput = 0.0f;

    float dashInput = 0.0f;
    float lastDashInput = 0.0f;


    [SerializeField]
    float attackCooldown;
    float attackCooldownCounter = 0;

    [SerializeField]
    float dashCooldown;
    float dashCooldownCounter = 0;

    bool attackCoolingDown;
    bool dashCoolingDown;


    Rigidbody2D thisRigidBody2D;
    Transform thisTransform;

    [Header("Movement")]

    [SerializeField]
    float playerAirSpeed = 0;

    [SerializeField]
    float playerSlowWalkSpeed = 0;


    [SerializeField]
    float jumpInitialVelocity;

    [SerializeField]
    float dashInitialVelocity;

    [SerializeField]
    float jumpHeightLimit;

    [SerializeField]
    float wallHorizontalVelocity = 3;

    float jumpedY = 0;

    float downVelocityInit;

    SpriteRenderer thisSpriteRender;

    PlayerControllerScript controllerScript;
    [SerializeField]
    PhysicsMaterial2D fullFriction;
    [SerializeField]
    PhysicsMaterial2D noFriction;


    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    protected override void OnStateEntered()
    {
        thisSpriteRender = this.Animator.GetComponent<SpriteRenderer>();
        thisRigidBody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
        thisTransform = this.Animator.transform;
        controllerScript = this.Animator.gameObject.GetComponent<PlayerControllerScript>();
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    void OnStateUpdate()
    {
        if (controllerScript.pausedGame)
        {
            this.Animator.SetFloat("HorizontalInput", 0);
            this.Animator.SetFloat("VerticalInput", 0);
            return;
        }
        if (this.Animator.GetBool("Dash"))
        {
            this.Animator.SetBool("Jump", false);
            this.Animator.SetBool("HoldJump", false);
            this.Animator.SetBool("Falling", true);
            lastDashInput = 0.0f;
            
            return;
        }
        if(this.Animator.GetBool("Attacking"))
        {

            this.Animator.SetBool("Jump", false);
            this.Animator.SetBool("HoldJump", false);
            this.Animator.SetBool("Falling", true);
            lastJumpInput = 0.0f;
          //  thisRigidBody2D.velocity = new Vector2(thisRigidBody2D.velocity.x, thisRigidBody2D.velocity.y);
            


            return;
        }

        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        thisRigidBody2D.sharedMaterial = (xInput == 0.0f && this.Animator.GetBool("OnSlope")) ?
                fullFriction : noFriction;


        lastAttackInput = attackInput;
        attackInput = Input.GetAxisRaw("Attack");

        lastDashInput = dashInput;
        dashInput = Input.GetAxisRaw("Dash");

        lastJumpInput = jumpInput;
        jumpInput = Input.GetAxisRaw("Jump");

        if (controllerScript.pausedGame)
        {
            this.Animator.SetFloat("HorizontalInput", 0);
            this.Animator.SetFloat("VerticalInput", 0); 
            return; }
        this.Animator.SetFloat("HorizontalInput", xInput);
        this.Animator.SetFloat("VerticalInput", yInput);

        if (GameManagerScript.Instance.CheckState("Attack") && attackInput != 0.0f && lastAttackInput != 1.0f && !attackCoolingDown)
        {
            this.Animator.SetBool("Attacking", true);
            attackCoolingDown = true;
        }

        if(GameManagerScript.Instance.CheckState("Dash") && dashInput != 0.0f && lastDashInput != 1.0f && !dashCoolingDown)
        {
            this.Animator.SetBool("Dash", true);
            dashCoolingDown= true;
           
        }

        

        if(attackCoolingDown)
        {
            attackCooldownCounter += 0.02f;
            if(attackCooldownCounter >= attackCooldown)
            {
                attackCoolingDown = false;
                attackCooldownCounter = 0;
                
            }
        }

        if(dashCoolingDown)
        {
            dashCooldownCounter += 0.02f;
            if(dashCooldownCounter >= dashCooldown)
            {
                dashCoolingDown = false;
                dashCooldownCounter = 0;
            }
        }

        if(xInput > 0)
        {
            thisSpriteRender.flipX = false;
        }
        else if(xInput < 0)
        {
            thisSpriteRender.flipX = true;
        }
        lastSlowWalkInput = slowWalkInput;
        slowWalkInput = Input.GetAxisRaw("SlowWalk");

        if (slowWalkInput == 1.0f && lastSlowWalkInput != 1.0f)
        {
            this.Animator.SetBool("SlowWalk", !this.Animator.GetBool("SlowWalk"));
        }
                

        if(!this.Animator.GetBool("SlowWalk"))
        {
            thisRigidBody2D.velocity = new Vector2(xInput, thisRigidBody2D.velocity.y);
        }
        else
        {
            thisRigidBody2D.velocity = new Vector2(xInput* playerSlowWalkSpeed, thisRigidBody2D.velocity.y);
        }
       
        if(lastJumpInput != 1.0f)
        {
            this.Animator.SetBool("Falling", true);

        }

        if (jumpInput != 0.0f && lastJumpInput != 1.0f && (!this.Animator.GetBool("InAir") || this.Animator.GetBool("RightWallTouching") || this.Animator.GetBool("LeftWallTouching")))
        {
            this.Animator.SetBool("Jump", true);
            float jumpDirection = 0;
            if (GameManagerScript.Instance.CheckState("WallJump"))
            {
                if (this.Animator.GetBool("RightWallTouching"))
                {
                    jumpDirection = -wallHorizontalVelocity;
                }
                else if (this.Animator.GetBool("LeftWallTouching"))
                {

                    jumpDirection = wallHorizontalVelocity;
                }
            }


            thisRigidBody2D.velocity = new Vector2(/* thisRigidBody2D.velocity.x+*/ jumpDirection, jumpInitialVelocity);
            jumpedY = thisTransform.position.y;
            this.Animator.SetBool("Falling", false);

            this.Animator.SetBool("HoldJump", false);


        }
        else if (!this.Animator.GetBool("Falling") && /*lastJumpInput != 0.0f &&*/ this.Animator.GetBool("InAir")  && lastJumpInput == jumpInput && this.Animator.GetBool("Jump"))
        {
            
            if (thisTransform.position.y - jumpedY >= jumpHeightLimit)
            {
                this.Animator.SetBool("Falling", true);
                this.Animator.SetBool("HoldJump", false);
            }
            else
            {
                this.Animator.SetBool("HoldJump", true);

            }
        }
        else
        {
           // this.Animator.SetBool("Jump", false);

            this.Animator.SetBool("HoldJump", false);   

        }

        IEnumerator RemoveInput()
        {
            controllerScript.pausedGame = true;
            yield return new WaitForSeconds(0.2f);
            controllerScript.pausedGame = false;
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
