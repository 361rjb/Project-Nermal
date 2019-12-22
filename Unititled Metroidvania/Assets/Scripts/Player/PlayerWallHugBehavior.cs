using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallHugBehavior : LogicalStateMachineBehaviour
{
    Rigidbody2D thisRigidbody2D;

    Vector2 wallVelocity;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    protected override void OnStateEntered()
    {
        thisRigidbody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    void OnStateUpdate()
    {
        this.Animator.SetBool("Dash", false);
        wallVelocity.x = thisRigidbody2D.velocity.x;
        wallVelocity.y =-1;
        thisRigidbody2D.velocity = wallVelocity;
    }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
       protected override  void OnStateExited()
        {
            this.Animator.SetBool("RightWallTouching", false);
            this.Animator.SetBool("LeftWallTouching", false);
        }

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
