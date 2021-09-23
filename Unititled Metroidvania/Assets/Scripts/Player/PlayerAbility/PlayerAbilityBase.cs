using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityBase : MonoBehaviour
{

    protected Vector2 direction = new Vector2();
    protected float lastInput, input;
    protected float angle;

    public void AbilityUpdate(float dt)
    {
    }

    public void SetBaseValues(float lastInput_in, float input_in, float xAlt, float yAlt, float angle)
    {
        direction.x = xAlt;
        direction.y = yAlt;
        lastInput = lastInput_in;
        input = input_in;
        
    }

}
