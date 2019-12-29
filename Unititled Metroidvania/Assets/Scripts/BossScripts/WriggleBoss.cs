using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriggleBoss : Boss
{
    [SerializeField]
    float attackCooldown;
    float cooldownCount = 0;

    bool cooldown;

    bool attacking;

    int currentAttackIndex;

    protected override void Start()
    {
        base.Start();
        foreach(BulletPatternScript bps in bossPatterns)
        {
            bps.SpawnBullets();
        }
        currentAttackIndex = -1;
        cooldownCount = 0;
        cooldown = true;
        attacking = false;
        
    }

    Vector2 storedVelocity;
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (!active)
            return;
        if (!attacking && avoidingWall && !cooldown && currentAttackIndex != 1)
        {
            stopMoving = true;
            currentAttackIndex = 0;
            bossPatterns[currentAttackIndex].StartPattern();
            if (diff.normalized.x < 0)
            {
                bossPatterns[currentAttackIndex].thisPattern.bulletSpawners[0].angle = 180;
            }
            else
            {
                bossPatterns[currentAttackIndex].thisPattern.bulletSpawners[0].angle = 0;
            }
            attacking = true;
            storedVelocity = thisRB.velocity;
        }

        if(jumping && thisRB.velocity.y <= -2 && !cooldown && currentAttackIndex == 0)
        {

            bossPatterns[0].DisablePattern();
            stopMoving = true;
            currentAttackIndex = 1;
            bossPatterns[currentAttackIndex].StartPattern();
            attacking = true;
            storedVelocity = thisRB.velocity;
            thisRB.gravityScale = 0;
            thisRB.velocity = Vector2.zero;
        }

        if(stopMoving)
        {

            thisRB.velocity = Vector2.zero;
            if (diff.normalized.x > 0 && currentAttackIndex == 0)
            {
                bossPatterns[currentAttackIndex].thisPattern.bulletSpawners[0].currentRotation = 180;
            }
            else if(currentAttackIndex == 0)
            {
                bossPatterns[currentAttackIndex].thisPattern.bulletSpawners[0].currentRotation = 0;
            }


            if (bossPatterns[currentAttackIndex].CheckPatternComplete())
            {

                attacking = false;
                stopMoving = false;
                cooldown = true;
                thisRB.gravityScale = 1;
                thisRB.velocity = storedVelocity;
                foreach(BulletPatternScript bps in bossPatterns)
                {
                    bps.DisablePattern();
                }
            }
        }

        if(cooldown)
        {
            cooldownCount += Time.deltaTime;
            if(cooldownCount >= attackCooldown)
            {
                cooldownCount = 0;
                cooldown = false;
                currentAttackIndex = -1;
            }
        }
        base.FixedUpdate();
    }
}
