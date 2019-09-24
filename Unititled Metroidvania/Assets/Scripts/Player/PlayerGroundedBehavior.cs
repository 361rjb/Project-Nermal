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
        groundedVelocity.x *= playerWalkSpeed;
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
