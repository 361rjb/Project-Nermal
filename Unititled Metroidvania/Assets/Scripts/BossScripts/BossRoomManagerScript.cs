using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossRoomManagerScript : MonoBehaviour
{
    public static BossRoomManagerScript Instance;

    [SerializeField]
    GameObject bossWalls;

    Image healthBar;

    [SerializeField]
    Boss thisRoomsBoss;
    Rigidbody2D bossRB;

    bool bossEnabled;
    [SerializeField]
    EventScriptableObject defeatEvent;

    [SerializeField]
    GameObject defeatedWalls;

    // Start is called before the first frame update
    void Start()
    {
        if(!Instance)
        {
            Instance = this;
        }
        Debug.Log("New Boss");
        if (bossWalls)
        {
            bossWalls.SetActive(false);
        }
        healthBar = GameObject.Find("BossHealthBar").GetComponent<Image>();
        bossEnabled = false;
        maxHealth = thisRoomsBoss.maxHealth;
        bossRB = thisRoomsBoss.GetComponent<Rigidbody2D>();
        bossRB.bodyType = RigidbodyType2D.Static;
        if (defeatEvent)
        {
            
            if (GameManagerScript.Instance.occuredEvents.Contains(defeatEvent.eventName))
            {
            if(defeatedWalls)
                defeatedWalls.SetActive(true);
            thisRoomsBoss.gameObject.SetActive(false);
            }
            else
            {
                if (defeatedWalls)
                    defeatedWalls.SetActive(false);
            thisRoomsBoss.gameObject.SetActive(true);
                    
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;   
    }


    float currentHealth;
    float maxHealth;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!bossEnabled)
        {
            return;
        }

        currentHealth = thisRoomsBoss.currentHealth;
        if(currentHealth <= 0)
        {
            DefeatedBoss();
        }
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * 5f);
    }

    public void EnableBoss()
    {
        Debug.Log("EnableBoss");
        if (bossWalls)
        {
            bossWalls.SetActive(true);
        }
        healthBar.enabled = true;
        bossEnabled = true; 
        bossRB.bodyType = RigidbodyType2D.Dynamic;
        thisRoomsBoss.active = true;
        thisRoomsBoss.EnableBoss();
    }

    public void DefeatedBoss()
    {
        Debug.Log("BossDefeated");
        if (bossWalls)
        {
            bossWalls.SetActive(false);
        }
        healthBar.enabled = false;
        bossEnabled = false;
        thisRoomsBoss.active = false;
        thisRoomsBoss.gameObject.SetActive(false);
        if (defeatEvent)
        {
            PauseMenuInputScript.Instance.StartDialogueEvent(defeatEvent);
            if(defeatedWalls)
            {
                defeatedWalls.SetActive(true);
            }
        }
    }
}
