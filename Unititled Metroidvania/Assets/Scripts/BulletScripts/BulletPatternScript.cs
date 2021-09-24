using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletPatternScript : MonoBehaviour
{
    public BulletPatternObject thisPattern;
    List<SpawnerObject> spawnerList;

    [SerializeField]
    GameObject defaultBulletPrefeb;

    
    public bool patternOn = false;
    List<List<BulletContainer>> bulletObjectPool;

    //Spawning bullets
    int currentSpawner = 0;

    List<int> poolDisableIndex = new List<int>(0);

    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("player").transform;
        if (thisPattern != null)
        {
            spawnerList = thisPattern.bulletSpawners;
        }
        //StartPattern();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FirePattern();
    }

   public void ResetAll()
    {
        int spawnerIndex = 0;
        foreach (SpawnerObject spawner in spawnerList)
        {
            for (int i = 0; i < spawner.bulletCount; i++)
            {
                GameObject.Destroy(bulletObjectPool[spawnerIndex][i].bulletGameObject);
            }
            bulletObjectPool[spawnerIndex].Clear();
            bulletObjectPool.Clear();
            spawnerIndex++;
        }

        poolDisableIndex.Clear();
    }

    public bool CheckPatternComplete()
    {
        foreach(SpawnerObject spawner in spawnerList)
        {
            if(!spawner.complete)
            {
                return false;
            }
        }
        return true;
    }


    public void SpawnBullets()
    {
        SceneManager.SetActiveScene(gameObject.scene);
        spawnerList = thisPattern.bulletSpawners;
        bulletObjectPool = new List<List<BulletContainer>>();
        int spawnerIndex = 0;
        foreach (SpawnerObject spawner in spawnerList)
        {
            bulletObjectPool.Add(new List<BulletContainer>());
            for (int i = 0; i < spawner.bulletCount; i++)
            {
                bulletObjectPool[spawnerIndex].Add(new BulletContainer());
                bulletObjectPool[spawnerIndex][i].bulletGameObject = (GameObject)Instantiate(defaultBulletPrefeb, spawnerList[spawnerIndex].isPositionChildOfOwner ? transform.position + spawnerList[spawnerIndex].position : spawnerList[spawnerIndex].position, transform.rotation);
                bulletObjectPool[spawnerIndex][i].thisBulletScript = bulletObjectPool[spawnerIndex][i].bulletGameObject.GetComponent<BulletGameobjectScript>();
                //bulletObjectPool[spawnerIndex][i].thisBulletScript.EnableBullet();
                //bulletObjectPool[spawnerIndex][i].thisBulletScript.DisableBullet();
                bulletObjectPool[spawnerIndex][i].thisBulletScript.SetBulletComponent(spawnerList[spawnerIndex].bulletType);

                bulletObjectPool[spawnerIndex][i].bulletTransform = bulletObjectPool[spawnerIndex][i].bulletGameObject.transform;

            }
            spawnerIndex++;
            poolDisableIndex.Add(0);
            spawner.currentRotation = spawner.angle;
            spawner.currentSpinSpeed = spawner.spinSpeed;
            spawner.fireRateCount = 0;
            spawner.burstRateCount = 0;
            spawner.nextBullet = 0;
            spawner.burstStopCount = 0;
            spawner.reversing = false;
            spawner.lifeCount = 0;
            spawner.complete = false;

        }
    }

    public void DisablePattern()
    {
        patternOn = false;
    }

    public void StartPattern()
    {
        
        patternOn = true;
        spawnerList = thisPattern.bulletSpawners;        
        int spawnerIndex = 0;
        foreach (SpawnerObject spawner in spawnerList)
        {
            for (int i = 0; i < spawner.bulletCount; i++)
            {
                bulletObjectPool[spawnerIndex][i].bulletTransform = bulletObjectPool[spawnerIndex][i].bulletGameObject.transform;

            }
            spawnerIndex++;
            spawner.currentRotation = spawner.angle;
            spawner.currentSpinSpeed = spawner.spinSpeed;
            spawner.fireRateCount = 0;
            spawner.burstRateCount = 0;
            spawner.nextBullet = 0;
            spawner.burstStopCount = 0;
            spawner.reversing = false;
            spawner.lifeCount = 0;
            spawner.complete = false;

        }

    }

    void FirePattern()
    {
        if (!patternOn)
            return;

        foreach (SpawnerObject spawner in spawnerList)
        {
            SpawnerShooting(spawner);
            currentSpawner++;
            currentSpawner %= spawnerList.Count;
            if(spawner.lifeTime > 0)
            {
                spawner.lifeCount += Time.deltaTime;
                if(spawner.lifeCount >= spawner.lifeTime)
                {
                    spawner.lifeCount = 0;
                    spawner.complete = true;
                }
            }
        }
    }

    void SpawnerShooting(SpawnerObject spawner)
    {
        bool fired = false;

        if (spawner.burstRateCount > spawner.burstRate)
        {
            if (spawner.burstStopCount == 0 && spawner.rotateOnBurst && spawner.burstRate != 0)
            {
                if (spawner.lockOn)
                {
                    Vector2 dir = (playerTransform.position - (transform.position + spawner.position)).normalized;
                    Debug.Log("Locked on");
                    spawner.currentRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                }
                else
                {
                    spawner.currentRotation += spawner.currentSpinSpeed;

                }
            }

            for (int i = 0; i < spawner.totalSpreads; i++)
            {
                for (int j = 0; j < spawner.individualTotalAngles; j++)
                {
                    if (spawner.fireRateCount > spawner.fireRate)
                    {
                        fired = true;

                        int bulletLoopCount = 0;
                        while (bulletObjectPool[currentSpawner][spawner.nextBullet].thisBulletScript.bulletEnabled)
                        {
                            bulletLoopCount++;
                            if (bulletLoopCount > spawner.bulletCount)
                            {
                                bulletObjectPool[currentSpawner][poolDisableIndex[currentSpawner]].thisBulletScript.DisableBullet();
                                spawner.nextBullet = poolDisableIndex[currentSpawner];
                                poolDisableIndex[currentSpawner]++;
                                poolDisableIndex[currentSpawner] %= spawner.bulletCount;

                            }
                            else
                            {
                                spawner.nextBullet++;
                                spawner.nextBullet %= spawner.bulletCount;
                            }
                        }
                        float rotation = 0;

                        int tempIndividual = (int)spawner.individualTotalAngles - 1;
                        if (spawner.individualTotalAngles - 1 == 0)
                        {
                            tempIndividual = (int)spawner.individualTotalAngles;
                        }

                        int tempTotal = (int)spawner.totalSpreads - 1;
                        if (spawner.totalSpreads - 1 == 0)
                        {
                            tempTotal = (int)spawner.totalSpreads;
                        }

                        if (spawner.lockOn && !spawner.rotateOnBurst)
                        {
                                Vector2 dir = (playerTransform.position - (transform.position + spawner.position)).normalized;

                                rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                           
                        }
                        else
                        {
                            rotation = spawner.currentRotation;
                        }
                        
                        rotation += ((spawner.individualSpreadAngle / (tempIndividual)) * j) + ((spawner.totalBulletSpread / (tempTotal)) * i);
                        if (spawner.lockOn )
                        {
                            if (spawner.totalSpreads > 1)
                            {
                                rotation -= ((spawner.totalBulletSpread / 2));
                            }
                            else if (spawner.individualTotalAngles > 1)
                            {
                                rotation -= ((spawner.individualSpreadAngle / 2));
                            }
                        }
                        
                            bulletObjectPool[currentSpawner][spawner.nextBullet].thisBulletScript.SetRotation(rotation);
                        bulletObjectPool[currentSpawner][spawner.nextBullet].bulletTransform.position = spawnerList[currentSpawner].isPositionChildOfOwner ? transform.position + spawnerList[currentSpawner].position : spawnerList[currentSpawner].position;
                        bulletObjectPool[currentSpawner][spawner.nextBullet].thisBulletScript.DisableBullet();
                        bulletObjectPool[currentSpawner][spawner.nextBullet].thisBulletScript.EnableBullet();

                    }


                }
            }

            spawner.fireRateCount += Time.deltaTime;
            if (fired)
            {
                spawner.fireRateCount = 0;
                if (!spawner.rotateOnBurst)
                {
                    spawner.currentRotation += spawner.currentSpinSpeed;
                }
                spawner.burstStopCount++;
            }

            if (spawner.spinSpeedChangeRate != 0 && Mathf.Abs(spawner.currentSpinSpeed) <= Mathf.Abs(spawner.maxSpinSpeed))
            {
                if (!spawner.reversing)
                {
                    spawner.currentSpinSpeed += spawner.spinSpeedChangeRate;
                }
                else
                {

                    spawner.currentSpinSpeed -= spawner.spinSpeedChangeRate;
                }
            }
            else if (spawner.spinReverse)
            {
                if (spawner.reversing)
                {
                    spawner.currentSpinSpeed += spawner.spinSpeedChangeRate;
                }
                else
                {

                    spawner.currentSpinSpeed -= spawner.spinSpeedChangeRate;
                }
                spawner.reversing = !spawner.reversing;
            }

        }


        spawner.burstRateCount += Time.deltaTime;
        if (spawner.burstRate > 0 && spawner.burstStopCount > spawner.burstStop)
        {
            spawner.burstStopCount = 0;
            spawner.burstRateCount = 0;


            if (spawner.burstReturnAngle)
            {
                if (!spawner.rotateOnBurst)
                {
                    spawner.currentRotation = spawner.angle;
                }
                spawner.currentSpinSpeed = spawner.spinSpeed;
                spawner.reversing = false;
            }


        }
    }
}