using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimber : EnemyBase
{
    [SerializeField]
    float speed;
    Rigidbody2D thisRB;
    [SerializeField]
    Transform frontCheck;
    [SerializeField]
    LayerMask surfaceLayer;
    Vector2 currentVelocity;
    Vector3 currentRotation;
    bool movingRight = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        thisRB = GetComponent<Rigidbody2D>();
        currentRotation = transform.eulerAngles;
        if(currentRotation.y == 180)
        {
            movingRight = false;
        }

        currentVelocity = transform.right * speed;
        currentVelocity.y = thisRB.velocity.y;
            thisRB.velocity = currentVelocity;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(!(Physics2D.Raycast(frontCheck.position, -Vector2.up, 1, surfaceLayer)) || Physics2D.Raycast(frontCheck.position, transform.right, 0.5f, surfaceLayer))
        {
            if (movingRight)
            {
                currentRotation.y = 180;
                movingRight = false;
                
            }
            else
            {

                currentRotation.y = 0;
                movingRight = true;
            }
            transform.eulerAngles = currentRotation;
            currentVelocity = transform.right * speed;
            currentVelocity.y = thisRB.velocity.y;
            thisRB.velocity = currentVelocity;
        }
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            TakeDamage(10);
        }
    }
}
