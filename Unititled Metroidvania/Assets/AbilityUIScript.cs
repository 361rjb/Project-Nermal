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
}
