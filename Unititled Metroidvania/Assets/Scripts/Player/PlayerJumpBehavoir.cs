using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpBehavoir : LogicalStateMachineBehaviour
{
    Rigidbody2D thisRigidBody2D;


    [SerializeField]
    int shortHopFrameLength;
    int shortHopCounter;

    [SerializeField]
    float jumpHoldVelocity;

    protected override void OnStateEntered()
    {
        thisRigidBody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
        shortHopCounter = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
   void OnStateUpdate()
    {
        thisRigidBody2D.velocity = new Vector2(thisRigidBody2D.velocity.x, jumpHoldVelocity);
        if (this.Animator.GetBool("HoldJump") && !this.Animator.GetBool("Falling"))
        {
            if (shortHopCounter > shortHopFrameLength)
            {
                thisRigidBody2D.velocity = new Vector2(thisRigidBody2D.velocity.x, jumpHoldVelocity);
            }

            this.Animator.SetBool("Falling", false);
           // this.Animator.SetBool("Jump", false);
        }
        else
        {
            this.Animator.SetBool("Falling", true);
            this.Animator.SetBool("HoldJump", false);
            this.Animator.SetBool("Jump", false);
        }
        shortHopCounter++;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
