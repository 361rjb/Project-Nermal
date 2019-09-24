using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxScript : MonoBehaviour
{
    [SerializeField]
    float regularSize = 0.4f;
    [SerializeField]
    float slowSpeedSize = 0.2f;

    SpriteRenderer hitBoxSprite;

    [SerializeField]
    Animator animator;

    CircleCollider2D circleCollider;

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        hitBoxSprite = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        hitBoxSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("SlowWalk"))
        {
            hitBoxSprite.enabled = true;
            circleCollider.radius = slowSpeedSize;

        }
        else
        {

            hitBoxSprite.enabled = false;
            circleCollider.radius = regularSize;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Enemy" || other.tag == "Bullet")
        {
            animator.SetBool("HitStun", true);
            playerRB.velocity = Vector2.zero;
            playerRB.AddForce((playerTransform.position - other.transform.position).normalized * 10f);
            circleCollider.enabled = false;

        }
    }
}
