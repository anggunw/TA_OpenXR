﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneGyro : MonoBehaviour
{
    public static SmartphoneGyro instance;
    private Quaternion rot;
    public int newX;
    public int newY;
    public int newZ;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SetPhoneRotation(float x, float y, float z, float w){
        rot.x = x;
        rot.y = y;
        rot.z = z;
        rot.w = w;

        rot = Quaternion.Euler(newX, newY, newZ) * rot; 
        transform.rotation = rot;
    }
}

