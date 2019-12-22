using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int baseHealth;
    public int health;

    public bool alive;

   public void TakeDamage(int damage)
    {
        health -= damage;
    }


    void EnableThisEnemy()
    {
        health = baseHealth;
        alive = true;
    }

    void DisableThisEnemy()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //Remove when enemyloader Enabled
        EnableThisEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDeath();
    }

    void CheckDeath()
    {
        if(health <= 0)
        {
            alive = false;
            DisableThisEnemy();
            
        }
    }
}
