using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : EnemyBase
{
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerAttack")
        {
            TakeDamage(10);
        }
    }
}
