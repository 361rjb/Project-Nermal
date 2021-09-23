using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    [SerializeField]
    PlayerControllerScript playerController;



    [Header("Misc Values"), SerializeField]
    float distanceFromPlayer;
    //Player's Current ability container
    //
    public PlayerAbilityBase ability;

    //BulletPatternScript abilityScript;

    Vector2 newPosition = new Vector2(0, 0);

    //Update called from player:
    public void AbilityUpdate(float dt, float lastInput, float input, float xAlt, float yAlt)
    {
        float angle = Mathf.Atan2(yAlt, xAlt);
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);
        newPosition.x = x;
        newPosition.y = y;
        transform.localPosition = newPosition*distanceFromPlayer;
        ability.SetBaseValues(lastInput, input, x, y, angle);

    }
}
