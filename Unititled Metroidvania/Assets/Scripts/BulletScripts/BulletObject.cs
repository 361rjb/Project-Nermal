using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Object")]
public class BulletObject : ScriptableObject
{
    public string bulletName = "new bullet";
    public Sprite bulletImage;

    public List<BulletWaypoints> bulletPath;

    public bool playPathOnce;

    
    public float acceleration;
    public float maxSpeed;
    public float minSpeed;

    
    
    public Collider2D colliderComponent;

    public bool effectedByGravity;

    public bool isLaser;

    public bool shouldFollowPlayer;
    

    public TrailRenderer bulletTrail;
    
}

//Class for the list of bullet waypoints
[System.Serializable]
public class BulletWaypoints
{
    public Vector2 position;
    public float slowRadius= 0.1f;
    public float slowSpeed = 2f;

    public float targetRadius =0.01f;
    public float speed = 5f;
}
