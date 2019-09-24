﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitStunBehavior : LogicalStateMachineBehaviour
{
    [SerializeField]
    float stunTimer;
    float stunTimerCount = 0;

    Collider2D playerHitBox;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    protected override void OnStateEntered()
    {
        playerHitBox = GameObject.Find("PlayerHealthBox").GetComponent<Collider2D>();
        
        stunTimerCount = 0;  
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    void OnStateUpdate()
    {
        if(stunTimerCount >= stunTimer)
        {
            this.Animator.SetBool("HitStun", false);
            stunTimerCount = 0;
            playerHitBox.enabled = true;
        }
        stunTimerCount += Time.deltaTime;
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
