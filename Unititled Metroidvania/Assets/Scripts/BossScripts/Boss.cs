using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(BulletPatternScript))]
public class Boss : MonoBehaviour
{

    public bool active = false;

    [SerializeField]
   public List< BulletPatternScript> bossPatterns;


    public int maxHealth;
   public int currentHealth;

   protected Transform player;
    PlayerControllerScript playerScript;
    protected Rigidbody2D thisRB;
    [SerializeField]
    float xSpeed;

    protected Vector2 currentVelocity;
    protected bool stopMoving;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent< PlayerControllerScript >();
        thisRB = GetComponent<Rigidbody2D>();
        active = false;

    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (!active)
            return;


        if (!jumping)
        {
            AvoidPlayer();
            AvoidWall();

            if (avoidingPlayer && avoidingWall && !stopMoving)
            {
                Jump();
            }


            if (stopMoving)
            {
                currentVelocity.x = 0;
            }
            
                currentVelocity.y = thisRB.velocity.y;
                thisRB.velocity = Vector2.Lerp(thisRB.velocity, currentVelocity, Time.deltaTime * 5f);
        }
        else
        {
            //currentVelocity.x = thisRB.velocity.x;
            GroundCheck();
        }
    }

    float jumpTimeBuffer = 0.3f;
    float jumpBufferCurrent = 0f;

    protected bool justLanded = false;

    void GroundCheck()
    {
        jumpBufferCurrent += Time.deltaTime;
        if (jumpBufferCurrent <= jumpTimeBuffer)
            return;
        if(Physics2D.Linecast(transform.position,transform.position - (Vector3.up), wallAvoidLayer))
        {
            justLanded = true;
            jumping = false;
        }
    }

    public virtual void EnableBoss()
    {

        currentHealth = maxHealth;
    }

    [SerializeField]
    LayerMask wallAvoidLayer;


    protected bool avoidingWall;
    protected bool avoidingPlayer;
    float jumpDirection;

    void AvoidWall()
    {
        if(Physics2D.Linecast(transform.position, transform.position + Vector3.right* 3f, wallAvoidLayer))
        {
            
            currentVelocity.x = -xSpeed;
            avoidingWall = true;
            jumpDirection = -jumpDistance;
        }
        else if(Physics2D.Linecast(transform.position, transform.position + Vector3.left * 3f, wallAvoidLayer))
        {

            currentVelocity.x = xSpeed;
            avoidingWall = true;
            jumpDirection = jumpDistance;
        }
        else
        { 
            avoidingWall = false;

        }
    }

    [SerializeField]
    float jumpHeight;
    [SerializeField]
    float jumpDistance;
    protected bool jumping;

    Vector2 jumpForce;
    void Jump()
    {
        jumpForce.x = jumpDirection;
        jumpForce.y = jumpHeight;
        jumping = true;
        thisRB.AddForce(jumpForce, ForceMode2D.Force);
        jumpBufferCurrent = 0f;
        justLanded = false;
    }


    protected Vector2 diff;
    [SerializeField]
    protected float avoidPlayerDistance;
    [SerializeField]
    protected bool avoidPlayer;
    void AvoidPlayer()
    {
        if (!avoidPlayer)
        {
            avoidingPlayer = false;
            return;
        }
        diff = transform.position - player.position;
        if ((diff).magnitude  < avoidPlayerDistance)
        {
            currentVelocity.x = diff.normalized.x > 0 ? xSpeed : -xSpeed;

            avoidingPlayer = true;
        }else
        {
            avoidingPlayer = false; 

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerAttack")
        {
            currentHealth -= playerScript.currentAttackDamage;
        }
        
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
