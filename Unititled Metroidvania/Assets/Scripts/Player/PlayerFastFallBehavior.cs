using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFastFallBehavior : LogicalStateMachineBehaviour
{
    Rigidbody2D thisRigidBody2D;

    Vector2 airVelocity = new Vector2();

    [SerializeField]
    float fastFallSpeed;
    [SerializeField]
    float fastFallMax;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    protected override void OnStateEntered()
    {
        thisRigidBody2D = this.Animator.gameObject.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
     void OnStateUpdate()
     {

        airVelocity = thisRigidBody2D.velocity;
        if (airVelocity.y > fastFallMax)
        {
            airVelocity.x = Mathf.Clamp(airVelocity.x, -5f, 5f);
            airVelocity.y += fastFallSpeed;

        }
        thisRigidBody2D.velocity = airVelocity;
        
        
        Debug.Log("Fast Falling");
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
