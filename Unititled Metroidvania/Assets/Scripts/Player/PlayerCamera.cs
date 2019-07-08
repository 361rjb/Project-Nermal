using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    LevelObject currentLevel;

    [SerializeField]
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        CheckBounds();
    }

    void CheckBounds()
    {
        Vector3 newPos = transform.position;
        if (transform.position.x > currentLevel.maxX)
        {
            newPos = new Vector3(currentLevel.maxX, transform.position.y, transform.position.z);

        }else if (transform.position.x < currentLevel.minX)
        {
            newPos = new Vector3(currentLevel.minX, transform.position.y, transform.position.z);
        }

        if (transform.position.y > currentLevel.maxY)
        {
            newPos = new Vector3(newPos.x, currentLevel.maxY, transform.position.z);

        } else if (transform.position.y <  currentLevel.minY)
        {
            newPos = new Vector3(newPos.x, currentLevel.minY, transform.position.z);
        }

        transform.position = newPos;
    }

    public void SetLevel(LevelObject thisLevel)
    {
        currentLevel = thisLevel;
    }

}
