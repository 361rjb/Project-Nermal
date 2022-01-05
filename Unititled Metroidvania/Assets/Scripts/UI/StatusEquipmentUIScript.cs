﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEquipmentUIScript : MonoBehaviour
{
    [SerializeField]
    Text timeDisplay;

    [SerializeField]
    Text moneyDisplay;

    [SerializeField]
    Text collectedHealthDisplay;

    [SerializeField]
    Sprite healthIcon;

    [SerializeField]
    Transform healthBackdrop;
    [SerializeField]
    GameObject healthPrefab;
    List<Image> healthIcons = new List<Image>();

    Vector2 spawnPos = new Vector2(0, 0);

    [SerializeField]
    List<ItemUIScript> itemsList = new List<ItemUIScript>();

    Dictionary<string, ItemUIScript> equipmentDictionary = new Dictionary<string, ItemUIScript>();

    GameObject player;
    PlayerControllerScript playerController;

    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControllerScript>();
        foreach (ItemUIScript item in itemsList)
        {
            equipmentDictionary.Add(item.thisItemEvent.eventName, item);
        }
        LoadHealth();
    }


    public void UnlockItem(string eventTitle)
    {
        if(equipmentDictionary.ContainsKey(eventTitle))
        {
            equipmentDictionary[eventTitle].CheckUnlock();
        }
    }

    //instantiate max health orbs; 
    //only show each orb current max health 
    //


    public void LoadHealth()
    {
        spawnPos = new Vector2(0, 0);
        bool overHalf = playerController.currentMaxHealth > (playerController.totalMaxHealth / 2);
        float total = overHalf ? 210.0f : 105.0f;
        float xIncrease = total / (float)(playerController.currentMaxHealth / 2);
        Vector3 newScale = new Vector3((xIncrease / 35f), (xIncrease / 35f ), 1.0f);

        for (int i = 0; i < ((float)playerController.currentMaxHealth) / 2; i++)
        {
            GameObject healthIcon = (GameObject)Instantiate(healthPrefab, healthBackdrop.transform);
            spawnPos.x = xIncrease * ((i + 1) % (playerController.totalMaxHealth / 4)) ;
            spawnPos.y = (playerController.totalMaxHealth / 4) - i > 0 ? 0 : -xIncrease;
            spawnPos.x -= 70f;
            
            healthIcon.transform.localPosition = spawnPos;

            healthIcon.transform.localScale = newScale;
            healthIcons.Add(healthIcon.GetComponent<Image>());

        }
    }
}
