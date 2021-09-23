using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectileAbility : PlayerAbilityBase
{

    [SerializeField]
    BulletPatternScript bulletSpawner;

    public void AbilityUpdate(float dt)
    {


        if (input != lastInput)
        { 
                
        }

        base.AbilityUpdate(dt);
    }

}
