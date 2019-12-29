using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    [SerializeField]
    RectTransform pauseCanvas;

    RectTransform thisTransform;
    Image thisImage;

    Color clear;
    Color black;

    PlayerControllerScript playerScript;



    [SerializeField]
    Animator anim;

    void Start()
    {
        playerScript = GameObject.Find("player").GetComponent<PlayerControllerScript>();
        clear = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        black = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        thisImage = GetComponent<Image>();
        thisTransform = GetComponent<RectTransform>();
        thisTransform.sizeDelta = pauseCanvas.sizeDelta;
        thisImage.color = clear;
        anim = GetComponent<Animator>();
    }
    
    public void Transition()
    {
        StartCoroutine(changeColor());
        Fade();
    }

    IEnumerator changeColor()
    {

        playerScript.canControl = false;
        yield return new WaitUntil(() => playerScript.currentLevelScript.loadedLevel);
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Black");

        anim.SetTrigger("Alpha");
        
    }
    
    void Fade()
    {
        anim.SetTrigger("Black");
    }

    public void FadeDone()
    {


        anim.ResetTrigger("Alpha");
        playerScript.canControl = true;
    }
}
