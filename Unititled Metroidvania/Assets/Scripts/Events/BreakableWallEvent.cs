using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallEvent : MonoBehaviour
{
    [SerializeField]
    string wallName;
    
    private void Start()
    {
        CheckReoccur();
    }

    private void Update()
    {
        CheckReoccur();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            GameManagerScript.Instance.occuredEvents.Add(wallName);
        }
    }


    void CheckReoccur()
    {
        if (GameManagerScript.Instance.occuredEvents.Contains(wallName))
        {
            gameObject.SetActive(false);
        }
    }
}
