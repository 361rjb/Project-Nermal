using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogEventScript : MonoBehaviour
{
    [SerializeField]
    Text logText;
    
    public Button buttonScript;

    Text textObj;

    string title;
    string textbox;

    public void SetTitle(string titleIn)
    {
        title = titleIn;
        logText.text = title;
    }

    public void SetTextbox(string textIn, Text textObjIn)
    {
        textbox = textIn;
        textObj = textObjIn;
    }

    public void OnClick()
    {
        textObj.text = textbox;
    }

    public void SetUp(Selectable selectable)
    {
        Navigation newNav = new Navigation ();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnUp= selectable;
        newNav.selectOnDown = buttonScript.navigation.selectOnDown;
        buttonScript.navigation = newNav;
    }
    public void SetDown(Selectable selectable)
    {
        Navigation newNav = new Navigation ();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnDown= selectable;
        newNav.selectOnUp = buttonScript.navigation.selectOnUp;
        buttonScript.navigation = newNav;
    }
}
