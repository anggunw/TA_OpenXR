﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.XR;

public class Manager : EventHandler
{

    [Header("Koneksi dari unity ke database - Manager")]
    public InputField ip;
    public InputField port;
    public string ipAddress = "";
    public string portServer = "";
    private string param;
    public static Manager instance;
    [Header("Hand, Virtual Smartphone dan Desktop Screen - Manager")]
    [SerializeField] private HandPoseSwitch handPoseSwitch;
    [SerializeField] private GameObject virtualSmartphone;
    [SerializeField] private GameObject desktopScreen;

    private void Awake() {
        if (instance == null){
            Debug.Log("client instantiated");
            instance = this;
        }    
        else if(instance != this){
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    void Start()
    {
        StartConnection();
        XRSettings.eyeTextureResolutionScale = 2f;
    }

    public void SubmitIP(string ip){
        ipAddress = ip;
        StartConnection();
    }
    
    public void StartConnection()
    {
        Config config = new Config();
        Debug.Log("Manager -> " +ipAddress +":"+ portServer);
        config.SetUrl(ipAddress);
        config.SetPort(portServer);
        ApplyURL(config);

        // start server
        Server.Start(6000);
        Client.instance.ConnectToServer();
    }

    public void Disconnected(){
        GameObject playerRef = GameObject.Find("Stage Object/OVRCameraRig");
        if(playerRef == null){
            playerRef = GameObject.Find("Stage Object/[VRSimulator_CameraRig]");
        }

        disconnectCanvas.SetActive(true);
        Vector3 offset = playerRef.transform.forward;
        offset *= 2f;
        offset += new Vector3(0, 1.6f, 0);
        disconnectCanvas.transform.position = playerRef.transform.position + offset;
        flushNode();
        virtualSmartphone.SetActive(false);
        desktopScreen.SetActive(false);
        handPoseSwitch.SetStaticHandStatus(false);
    }

    public void SetVirtualSmartphoneCanvasActive(){
        Debug.Log("set canvas active");
        virtualSmartphone.SetActive(true);
        disconnectCanvas.SetActive(false);
        handPoseSwitch.SetStaticHandStatus(true);
    }

    public void SetScreenMode(string _swipeType){
        if(_swipeType == "up"){
            Debug.Log("screen mode VR");
            desktopScreen.SetActive(true);
            virtualSmartphone.SetActive(false);
            GameObject playerRef = GameObject.Find("Stage Object/OVRCameraRig");
            if(playerRef == null){
                playerRef = GameObject.Find("Stage Object/[VRSimulator_CameraRig]");
            }
            Vector3 offset = playerRef.transform.forward;
            offset *= 2f;
            offset += new Vector3(0, 1.6f, 20f);
            desktopScreen.transform.position = playerRef.transform.position + offset;
        }else if(_swipeType == "down"){
            Debug.Log("screen mode smartphone");
            desktopScreen.SetActive(false);
            virtualSmartphone.SetActive(true);
        }
    }
}
