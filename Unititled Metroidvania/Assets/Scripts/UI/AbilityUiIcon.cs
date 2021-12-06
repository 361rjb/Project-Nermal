using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityUiIcon : MonoBehaviour
{

    public Text title;

    public PlayerAbilityBase abilityScript;
    public GameObject abilityGO;

    AbilityUIScript abilityUI;

    public Button buttonScript;

    public Image sprite;

    // Start is called before the first frame update
    void Start()
    {
        abilityUI = GameObject.FindObjectOfType<AbilityUIScript>();
    }


    public void SetAbility()
    {
        abilityUI.SelectAbility(this);
    }

    public void SetUp(Selectable selectable)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnUp = selectable;
        newNav.selectOnDown = buttonScript.navigation.selectOnDown;
        buttonScript.navigation = newNav;
    }
    public void SetDown(Selectable selectable)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnDown = selectable;
        newNav.selectOnUp = buttonScript.navigation.selectOnUp;
        buttonScript.navigation = newNav;
    }
   
}
