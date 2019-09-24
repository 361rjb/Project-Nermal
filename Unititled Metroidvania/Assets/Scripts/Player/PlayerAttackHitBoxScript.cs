using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitBoxScript : MonoBehaviour
{
    [SerializeField]
    Animator playerAnimator;
    SpriteRenderer thisSprite;
    
    Color hitEnemy = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        thisSprite = GetComponent<SpriteRenderer>();
        thisSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            playerAnimator.SetBool("HitLag", true);
            Debug.Log("AHHH");
            thisSprite.color = hitEnemy;
        }
    }

}
