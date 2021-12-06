using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityBase : MonoBehaviour
{

    public string title = "TITLE IS NULL";
    public string description = "DESCRIPTION IS NULL";

    protected Vector2 direction = new Vector2();
    protected float lastInput, input;
    protected float radAngle, degAngle;

    public EventScriptableObject eventUnlocksFrom;
    public Sprite abilityIcon;

    protected virtual void Start()
    {

    }

    public virtual void AbilityUpdate(float dt)
    {
    }

    public void SetBaseValues(float lastInput_in, float input_in, float xAlt, float yAlt, float angle_in)
    {
        direction.x = xAlt;
        direction.y = yAlt;
        lastInput = lastInput_in;
        input = input_in;
        radAngle = angle_in;
        degAngle = radAngle * (180f / 3.14f);
        
    }

}
