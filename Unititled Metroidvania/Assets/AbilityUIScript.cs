using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityUIScript : MonoBehaviour
{
    [SerializeField]
    List<Selectable> abilitySlot;
    [SerializeField]
    Text abilityTitle;
    [SerializeField]
    Text abilityDescription;

    [SerializeField]
    List<PlayerAbilityBase> playerAbilities = new List<PlayerAbilityBase>();
    
    List<PlayerAbilityBase> unlockedPlayerAbilities = new List<PlayerAbilityBase>();

    private void Start()
    {
        
    }

}
