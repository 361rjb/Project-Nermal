using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Spawner Object")]
public class BulletPatternObject : ScriptableObject
{
    public List<SpawnerObject> bulletSpawners;
    
}

[System.Serializable]
public class SpawnerObject
{
    public Vector3 position;    //*DONE*
    public bool isPositionChildOfOwner;    //*DONE*

    public BulletObject bulletType;    //*DONE*
    public int bulletCount;    //*DONE*
    [HideInInspector]
    public int nextBullet;    //*DONE*

    public float individualTotalAngles;    //*DONE*
    public float individualSpreadAngle;    //*DONE*


    public float totalSpreads;    //*DONE*
    public float totalBulletSpread;    //*DONE*

    public bool isRandomSpreads;
    public float randomMinAngle;
    public float randomMaxAngle;

    public float spinSpeed;    //*DONE*
    public float spinSpeedChangeRate;    //*DONE*
    public bool spinReverse;    //*DONE*
    [HideInInspector]
    public bool reversing;    //*DONE*
    public float maxSpinSpeed;    //*DONE*
    [HideInInspector]
    public float currentRotation;    //*DONE*
    [HideInInspector]
    public float currentSpinSpeed;    //*DONE*

    public bool rotateOnBurst = false;    //*DONE*


    public float fireRate;
    [HideInInspector]
    public float currentfireRate;
    
    public float fireRateAcceleration;
    public float maxFireRate;
    [HideInInspector]
    public float fireRateCount;    //*DONE*


    public float burstRate;    //*DONE*
    [HideInInspector]
    public float burstRateCount;    //*DONE*
    public int burstStop;    //*DONE*
    [HideInInspector]
    public int burstStopCount;    //*DONE*

    public bool burstReturnAngle = false;    //*DONE*

    public float lifeTime;
    public float angle;    //*DONE*

    public bool lockOn;    //*DONE*

    /* probably should go in 
    public bool lockOnOverTime;

    public float lockOnRate;
    [HideInInspector]
    public float lockOnRateCount;
    */
    

        
}

public class BulletContainer
{
    public GameObject bulletGameObject;
    public BulletGameobjectScript thisBulletScript;
    public Transform bulletTransform;
}
