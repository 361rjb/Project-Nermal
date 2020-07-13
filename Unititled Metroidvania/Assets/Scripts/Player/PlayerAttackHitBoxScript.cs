using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttackHitBoxScript : MonoBehaviour
{
    [SerializeField]
    Animator playerAnimator;
    SpriteRenderer thisSprite;
    [SerializeField]
    ParticleSystem thisSystem;

    [SerializeField]
    Transform particleTransform;
    
    Color hitEnemy = Color.red;

    ParticleSystem.MainModule main;
    // Start is called before the first frame update
    void Start()
    {
        thisSprite = GetComponent<SpriteRenderer>();

        thisSprite.enabled = false;

        particleTransform.parent = null;
        particleTransform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        particleTransform.position = currentPos;
    }


    Vector3 currentPos = Vector2.zero;
    Vector3 right = new Vector3(-15, 90, 0);
    Vector3 left = new Vector3(-15, 270, 0);

    Vector3 spriteRight = new Vector3(0, 0, 0);
    Vector3 spriteLeft = new Vector3(1, 0, 0);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            playerAnimator.SetBool("HitLag", true);
            Debug.Log("AHHH");
            thisSprite.color = hitEnemy;
            if(thisSprite.flipX)
            {
                particleTransform.eulerAngles = left;

            }
            else
            {
                particleTransform.eulerAngles = right;

            }
            currentPos = other.transform.position;
            thisSystem.Play(true);
        }
    }
    

}
