using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    List<PlayerSave> playerSaves = new List<PlayerSave>();
    [SerializeField]
    List<Button> gameButtons;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    Button newGameButton;
    [SerializeField]
    Button backButton;
    [SerializeField]
    Button exitButton;

    [SerializeField]
    GameObject popUp;
    [SerializeField]
    Text popUpText;
    [SerializeField]
    Button confirmButton;
    [SerializeField]
    Button declineButton;
    [SerializeField]
    EventSystem system;

    // Start is called before the first frame update
    void Start()
    {        
        foreach(Button b in gameButtons)
        {
            PlayerSave ps = SaveLoadHandler.LoadGame(gameButtons.IndexOf(b));
            if (ps != null)
            {

                b.interactable = true;
                b.GetComponentInChildren<Text>().text = "Start Game " + ps.saveIndex;

                playerSaves.Add(ps);
                //get button from serialized list
                //hide button
                //Add to list
                //Set text to be save data ex: % time and last save local
            }
            else
            {
                b.GetComponentInChildren<Text>().text = "New Game";
                
            }
                b.gameObject.SetActive(false);

        }

        if(playerSaves.Count <= 0)
        {
            system.firstSelectedGameObject =newGameButton.gameObject;
            continueButton.gameObject.SetActive(false);
            newGameButton.gameObject.SetActive(true);
        }
        else
        {
            system.firstSelectedGameObject =continueButton.gameObject;
            continueButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(false);
        }
        backButton.gameObject.SetActive(false);

        //if() button list isbigger than max save dont show new game button
        //also for continue, dont show for continue if more than one game
        popUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Buttons

    int currentIndex;
    public void StartGame(int index)
    {
        currentIndex = index;
        popUp.SetActive(true);

        system.SetSelectedGameObject(confirmButton.gameObject);
        if (index < playerSaves.Count && playerSaves[index] != null)
        {
            popUpText.text = "Continue Game?";

        }
        else
        {
            popUpText.text = "Create New Save?";
        }

        backButton.gameObject.SetActive(false);
        foreach (Button b in gameButtons)
        {
            b.gameObject.SetActive(false);
        }

    }

    public void Confirm()
    {
        GameManagerScript.Instance.Load(currentIndex);

    }

    public void Decline()
    {
        popUp.SetActive(false);
        backButton.gameObject.SetActive(true);
        foreach (Button b in gameButtons)
        {
            b.gameObject.SetActive(true);
        }

        system.SetSelectedGameObject(gameButtons[0].gameObject);
    }

    //Show optional save slots for player to pick
    public void NewGame()
    {
        foreach(Button b in gameButtons)
        {
            b.gameObject.SetActive(true);
        }
        exitButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(false);
        system.SetSelectedGameObject(gameButtons[0].gameObject);
        
    }

    //Show current saves and allow for new games in any empty slots
    public void Continue()
    {
        foreach(Button b in gameButtons)
        {
            b.gameObject.SetActive(true);
        }

        exitButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        system.SetSelectedGameObject(gameButtons[0].gameObject);
    }

    //go back to main menu ie hide all 
    public void Back()
    {
        foreach (Button b in gameButtons)
        {
            b.gameObject.SetActive(false);
        }

        exitButton.gameObject.SetActive(true);
        if (playerSaves.Count <= 0)
        {
            newGameButton.gameObject.SetActive(true);
            system.SetSelectedGameObject( newGameButton.gameObject);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            system.SetSelectedGameObject(continueButton.gameObject);
        }
        backButton.gameObject.SetActive(false);
    }
    
    public void CloseApplication()
    {
        Application.Quit();
    }
}
