using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectileAbility : PlayerAbilityBase
{


    [SerializeField]
    BulletPatternScript bulletSpawner;

    protected override void Start()
    {
        base.Start();
        bulletSpawner.SpawnBullets();
    }

    public override void AbilityUpdate(float dt)
    {

        if (input != lastInput && !bulletSpawner.patternOn)
        {
            Debug.Log("Player Ability Start: " + input);
            bulletSpawner.StartPattern();

        }

        bulletSpawner.thisPattern.bulletSpawners.ForEach(s => s.currentRotation = degAngle);
        if (bulletSpawner.CheckPatternComplete())
        {
            bulletSpawner.DisablePattern();
        }

        base.AbilityUpdate(dt);
    }

}
