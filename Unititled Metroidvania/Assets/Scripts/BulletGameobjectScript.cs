using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGameobjectScript : MonoBehaviour
{

    
    public BulletObject thisBulletComponent;

    [SerializeField]
    int currentIndex;

    SpriteRenderer thisSpriteRenderer;

    Vector2 currentVelocity;

    Rigidbody2D thisRigidBody;

    [HideInInspector]
    public bool bulletEnabled = false;

    Vector2 position;
    Vector2 target;

    public Vector2 rotation;

    public Vector2 rotatePathAroundPoint;
    Matrix4x4 rotationMat;
    float rotationDegree;

    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        EnableBullet();   
        //SetBulletComponent(thisBulletComponent);
        DisableBullet();
    }

    private void OnEnable()
    {
        EnableBullet();
    }

    private void OnDisable()
    {
        DisableBullet();
    }

    public void EnableBullet()
    {

        thisRigidBody = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        col.enabled = true;
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        if (thisSpriteRenderer != null)
        {
            thisSpriteRenderer.enabled = true;
        }
        currentIndex = 0;
        bulletEnabled = true;
        Debug.Log("Enabled");
        if(thisBulletComponent != null)
        {
            thisRigidBody.velocity = Vector2.zero;
            position = transform.position;
            target = position + thisBulletComponent.bulletPath[currentIndex].position;
        }
    }

    

    public void DisableBullet()
    {
        thisSpriteRenderer.enabled = false;
        currentIndex = 0;
        bulletEnabled = false;
        col.enabled = false;

    }

    public void SetBulletComponent(BulletObject bullet)
    {
       
            thisBulletComponent = bullet;

        Debug.Log("Setting Component " + thisBulletComponent);
        thisSpriteRenderer.sprite = thisBulletComponent.bulletImage;

        position = transform.position;
        target = position + thisBulletComponent.bulletPath[0].position;
        rotatePathAroundPoint = position;

    }

    public void SetRotation( float angle)
    {
        rotationDegree = angle;
        Quaternion newQuaternion = Quaternion.Euler(0, 0, rotationDegree);
        rotationMat = Matrix4x4.Rotate(newQuaternion);
    }

    // Update is called once per frame
    void Update()
    {
        if(thisBulletComponent != null && bulletEnabled)
        {
            FollowPath();
        }


    }

    void FollowPath()
    {
        
        if(currentIndex >= thisBulletComponent.bulletPath.Count && thisBulletComponent.playPathOnce)
        {
        }
        else
        {
            position = transform.position;

            if (Vector2.Distance(target, position) < thisBulletComponent.bulletPath[currentIndex].targetRadius)
            {

                currentIndex++;
                if (currentIndex >= thisBulletComponent.bulletPath.Count && thisBulletComponent.playPathOnce)
                {
                    currentVelocity.Normalize();
                    currentVelocity *= thisBulletComponent.bulletPath[currentIndex - 1].speed;
                }
                else
                {
                    currentIndex %= thisBulletComponent.bulletPath.Count;
                    
                    if (currentIndex == 0)
                    {
                        rotatePathAroundPoint = position;
                        //Debug.Log("new Postigion: " + position);
                    }
                    target = (thisBulletComponent.bulletPath[currentIndex].position);

                   // target -= rotatePathAroundPoint;
                    target = rotationMat.MultiplyPoint3x4(target);
                    //target += rotatePathAroundPoint;
                    target += position;
                }
                

            }
            else if ( Vector2.Distance(position, target) < thisBulletComponent.bulletPath[currentIndex].slowRadius)
            {
                currentVelocity = (target - position).normalized;
                currentVelocity *= thisBulletComponent.bulletPath[currentIndex].slowSpeed;
                
            }
            else
            {
                currentVelocity = (target - position).normalized;                
                currentVelocity *= thisBulletComponent.bulletPath[currentIndex].speed;
            }

           
        }
        thisRigidBody.velocity = currentVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!thisBulletComponent.isPlayers && ( collision.tag == "PlayerHitBox" ||  collision.gameObject.layer == LayerMask.NameToLayer("Ground")) )
        {
            Debug.Log("collision : " + collision.gameObject);
            DisableBullet();
        }
    }


}
