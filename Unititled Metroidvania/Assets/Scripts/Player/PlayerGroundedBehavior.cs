using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedBehavior : LogicalStateMachineBehaviour
{
    [SerializeField]
    float playerWalkSpeed = 0;

    [SerializeField]
    GameObject groundCheck;
    Collider2D groundColliderCheck;
    [SerializeField]
    GameObject ceilingCheck;
    Collider2D ceilingColliderCheck;

    [SerializeField]
    LayerMask groundLayer;

    Rigidbody2D thisRigidBody2D;
    Vector2 groundedVelocity = new Vector2();

    

    float slopeDownAngle;
    float slopeDownAngleOld;
    Vector2 slopeNormalPerp;
    bool onSlope = false;
    

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    protected override void OnStateEntered()
    {
        thisRigidBody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("GroundCheck");
        ceilingCheck = GameObject.Find("CeilingCheck");
        groundColliderCheck = groundCheck.GetComponent<Collider2D>();
        ceilingColliderCheck = ceilingCheck.GetComponent<Collider2D>();
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    void OnStateUpdate()
    {
        GroundCheck();

        groundedVelocity = thisRigidBody2D.velocity;
        float xSpeed = groundedVelocity.x;
        if(onSlope && !this.Animator.GetBool("Jump"))
        {
            groundedVelocity.x = playerWalkSpeed * slopeNormalPerp.x * -xSpeed;
            groundedVelocity.y = playerWalkSpeed * slopeNormalPerp.y * -xSpeed;            

        }
        else
        {
            groundedVelocity.x *= playerWalkSpeed;
        }
        thisRigidBody2D.velocity = groundedVelocity;
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    void GroundCheck()
    {
        if (groundColliderCheck.IsTouchingLayers(groundLayer))
        {
            this.Animator.SetBool("InAir", false);
            //falling = false;
            //fastFall = false;

           RaycastHit2D frontHit = Physics2D.Raycast(groundCheck.transform.position, groundCheck.transform.right, 1.5f, groundLayer);
           RaycastHit2D backHit = Physics2D.Raycast(groundCheck.transform.position, -groundCheck.transform.right, 1.5f, groundLayer);
           if (frontHit)
           {
               onSlope = true;
          
           }
           else if (backHit)
           {
               onSlope = true;
          
           }
           else
           {
               onSlope = false;
               //slopeSideAngle = 0;
           }
           RaycastHit2D hit = Physics2D.Raycast(groundCheck.transform.position, Vector2.down, 1.5f, groundLayer);
           if(hit)
           {
               slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
               slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
          
               if( slopeDownAngle != slopeDownAngleOld)
                {
                    onSlope = true;
                }
          
               slopeDownAngleOld = slopeDownAngle;
                Debug.DrawRay(hit.point, hit.normal, Color.black);
           }
            this.Animator.SetBool("OnSlope", onSlope);

        }
        else
        {
            this.Animator.SetBool("InAir", true);
        }


    }



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
