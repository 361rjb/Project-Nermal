using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBehavior : LogicalStateMachineBehaviour
{
    [SerializeField]
    float attackTimer;

    float attackTimerCount;

    SpriteRenderer thisSpriteRenderer;

    Transform hitboxTransform;
    SpriteRenderer hitBoxSpriteRender;

    Rigidbody2D thisRigidBody2D;

    Vector2 hitLagVelocity = Vector2.zero;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    protected override void OnStateEntered()
    {
        attackTimerCount = 0;
        hitboxTransform = GameObject.Find("AttackHitBox").transform;
        hitBoxSpriteRender = hitboxTransform.GetComponent<SpriteRenderer>();
        hitBoxSpriteRender.enabled = true;
        hitBoxSpriteRender.color = Color.blue;
        hitboxTransform.gameObject.GetComponent<Collider2D>().enabled = true;
        thisSpriteRenderer = this.Animator.gameObject.GetComponent<SpriteRenderer>();
        thisRigidBody2D = this.Animator.GetComponent<Rigidbody2D>();

        if (thisSpriteRenderer.flipX)
        {
            hitboxTransform.localPosition = new Vector2(-0.7f, hitboxTransform.localPosition.y);
        }
        else
        {
            hitboxTransform.localPosition = new Vector2(0.7f, hitboxTransform.localPosition.y);
        }
    }



    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    void OnStateUpdate()
    {
        if (this.Animator.GetBool("HitLag"))
        {
            if (thisSpriteRenderer.flipX)
            {

                thisRigidBody2D.velocity = new Vector2(5, thisRigidBody2D.velocity.y);
            }
            else
            {
                thisRigidBody2D.velocity = new Vector2(-5, thisRigidBody2D.velocity.y);
            }
        }

        if (attackTimerCount >= attackTimer)
        {
            attackTimerCount = 0;
            this.Animator.SetBool("Attacking", false);
            this.Animator.SetBool("HitLag", false);
            hitBoxSpriteRender.enabled = false;
            hitboxTransform.gameObject.GetComponent<Collider2D>().enabled = false;
        }
        attackTimerCount += Time.deltaTime;
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    void OnStateExit()
    {
        attackTimerCount = 0;
        this.Animator.SetBool("Attacking", false);
        this.Animator.SetBool("HitLag", false);
        hitBoxSpriteRender.enabled = false;

        hitboxTransform.gameObject.GetComponent<Collider2D>().enabled = false;
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
