﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform[] backgrounds;
    TilemapRenderer[] backgroundLayer;
    float[] pScales;
    [SerializeField]
    float smoothing = 1f;
    [SerializeField]
    float baseMultiplier = 3f;


    [SerializeField]
    float xMultiplier = 1f;
    [SerializeField]
    float yMultiplier = 1f;
    Transform cam;
    Vector3 previousCamPosition;

    [SerializeField] bool XParallax = true;
    [SerializeField] bool YParallax = true;

    

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main.transform;
        previousCamPosition = Vector3.zero;



        pScales = new float[backgrounds.Length];
        backgroundLayer= new TilemapRenderer[backgrounds.Length];
        for(int i =0;i < backgrounds.Length; i++)
        {
            backgroundLayer[i] = backgrounds[i].GetComponent<TilemapRenderer>();
            pScales[i] = backgroundLayer[i].sortingOrder * baseMultiplier;
        }
    }

    Vector3 targetPos;
    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallaxX = (previousCamPosition.x - cam.position.x) * pScales[i] *xMultiplier;
            float parallaxY = (previousCamPosition.y - cam.position.y) * -pScales[i] * yMultiplier;
            float targetPosX = backgrounds[i].position.x + parallaxX;
            float targetPosY = backgrounds[i].position.y + parallaxY;
            targetPos.x = XParallax ? targetPosX : backgrounds[i].position.x;
            targetPos.y = YParallax ?  targetPosY : backgrounds[i].position.y;
            targetPos.z = backgrounds[i].position.z;

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPos, Time.deltaTime * smoothing);     
        }
        previousCamPosition = cam.position;
    }
}
