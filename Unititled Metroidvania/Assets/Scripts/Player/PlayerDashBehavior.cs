using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashBehavior : LogicalStateMachineBehaviour
{

    [SerializeField]
    float dashTimer;

    float dashTimerCount;

    Rigidbody2D thisRigidBody2D;

    SpriteRenderer thisSpriteRenderer;

    [SerializeField]
    float dashSpeed;
    Vector2 dashVelocity = Vector2.zero;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    protected override void OnStateEntered()
    {
        dashTimerCount = 0f;
        thisSpriteRenderer = this.Animator.GetComponent<SpriteRenderer>();
        thisRigidBody2D = this.Animator.GetComponent<Rigidbody2D>();
    }

    void OnStateUpdate()
    {
        if (dashTimerCount >= dashTimer)
        {
            dashTimerCount = 0;
            this.Animator.SetBool("Dash", false);
           
        }
        if(thisSpriteRenderer.flipX)
        {
            dashVelocity.x = -dashSpeed;
        }
        else
        {
            dashVelocity.x = dashSpeed;
        }
        dashTimerCount += Time.deltaTime;
        thisRigidBody2D.velocity = dashVelocity;
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
