using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInputScript : MonoBehaviour
{
    GameObject player;
    PlayerControllerScript playerController;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject savingImage;


    float pauseInput;
    float lastPauseInput;

    bool inPause= false;

    public bool showSave = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControllerScript>();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if(pauseInput == 1.0f && lastPauseInput != 1.0f && !showSave)
        {
            inPause = !inPause;
        }

        if (inPause || showSave)
        {
            Time.timeScale = 0.0f;
            if (showSave)
            {
                savingImage.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
            playerController.pausedGame = true;
        }
        else
        {
            Time.timeScale = 1.0f; 
            
                savingImage.SetActive(false);
            
                pauseMenu.SetActive(false);
            
            playerController.pausedGame = false;
        }
        
    }

    void GetInput()
    {
        lastPauseInput = pauseInput;
        pauseInput = Input.GetAxisRaw("Pause");
    }


}
