﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
/* SCRIPT UNTUK APLIKASI DI SMARTPHONE */
public class NetworkUIManager : MonoBehaviour {
    public static NetworkUIManager instance;
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private GameObject connectMenu;
    [SerializeField] private GameObject dashboardMenu;
    [SerializeField] private GameObject filterMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject connectButton;
    [SerializeField] private GameObject dashboardPanel;
    [SerializeField] private GameObject dashboardErrorPanel;
    [SerializeField] private GameObject dashboardLoading;
    private TextMeshProUGUI connectButtonText;
    [SerializeField] private TextMeshProUGUI ipInputText;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private TextMeshProUGUI ipText;
    [SerializeField] private Button addNodeSizeBtn;
    [SerializeField] private Button subtractNodeSizeBtn;
    [SerializeField] private TextMeshProUGUI nodeSizeText;
    [SerializeField] private Camera renderCamera;
    private Texture2D textureToSend2D;
    private bool connectSuccess;

    // connect to server
    RequestHandler requestPeneliti = new RequestHandler();
    private string URL = "https://127.0.0.5:5000";
    // dashboard text
    private TextMeshProUGUI journals;
    private TextMeshProUGUI conferences;
    private TextMeshProUGUI books;
    private TextMeshProUGUI thesis;
    private TextMeshProUGUI patents;
    private TextMeshProUGUI research;
    public TextMeshProUGUI debuggingText;
    private bool isDisconnectButtonPressed = false;
    private bool isOrientationUp = false;
    private float nodeSize = 2f;
    private float maxNodeSize = 5f;
    private float minNodeSize = 1f;
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

    private void Start ()
    {
        ipField.onEndEdit.AddListener(SubmitIP);
        connectSuccess = false;
        connectButtonText = connectButton.GetComponentInChildren<TextMeshProUGUI>();
        ipInputText = ipField.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

        journals = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        conferences = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        books = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        thesis = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
        patents = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>();
        research = dashboardMenu.transform.GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>();
    }

    private void Update(){
        if(!isOrientationUp && Input.deviceOrientation == DeviceOrientation.FaceUp){
            isOrientationUp = true;
            ClientSend.SendOrientation(isOrientationUp);
        }else if(isOrientationUp && Input.deviceOrientation != DeviceOrientation.FaceUp){
            isOrientationUp = false;
            ClientSend.SendOrientation(isOrientationUp);
        }
    }

    // to connect to VR
    private void SubmitIP(string _ip)
    {
        Client.instance.ip = _ip;
    }

    // to connect to database
    private void SetURL(string _url){
        URL = _url;
    }

    public void ConnectToServer()
    {
        detailText.gameObject.SetActive(true);
        detailText.text = "Connecting...";
        detailText.color = new Color32(50, 59, 89, 255);
        connectButtonText.text = "Connect";
        connectButtonText.color = new Color32(142, 144, 149, 255);
        connectButton.GetComponent<Button>().interactable = false;
        ipField.interactable = false;
        ipInputText.color = new Color32(142, 144, 149, 255);
        
        StartCoroutine(WaitForNSeconds(1f));
        Client.instance.ConnectToServer();
        ipText.text = "Connected to:" + "\n" + Client.instance.ip;
    }

    IEnumerator WaitForNSeconds(float n)
    {
        yield return new WaitForSeconds(n);
        if(!connectSuccess){
            detailText.text = "Fail to connect";
            detailText.color = new Color32(231, 65, 65, 255);
            connectButtonText.text = "Try Again";
            connectButtonText.color = new Color32(255, 255, 255, 255);
            connectButton.GetComponent<Button>().interactable = true;
            ipField.interactable = true;
            ipInputText.color = new Color32(50, 59, 89, 255);
        } 
    }

    public void ClientConnected(string _url){
        connectSuccess = true;

        // set database url
        SetURL(_url);

        connectMenu.SetActive(false);
        OnTapDashboardMenu();
    }

    public void OnTapDashboardMenu(){
        dashboardLoading.SetActive(true);
        dashboardPanel.SetActive(false);
        dashboardErrorPanel.SetActive(false);
        dashboardMenu.SetActive(true);
        ClientSend.SendPageType("dashboardMenu");
      
        // get dashboard data
        requestPeneliti.URL = URL;
        StartCoroutine(requestPeneliti.RequestData((result) =>
        {
            // mengambil jumlah jurnal, conference, books, thesis, paten dan research yang ada
            hasilPublikasiITS(result);
            
        }, (error) => {
            if (error != "")
            {
                Debug.Log("fail!");
                dashboardLoading.SetActive(false);
                dashboardErrorPanel.SetActive(true);
                ClientSend.SendPageType("dashboardError");
            }
        }));   
    }

    private void hasilPublikasiITS(RawData rawdata)
    {
        Debug.Log("get!");
        dashboardPanel.SetActive(true);
        dashboardLoading.SetActive(false);
        journals.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[0].journals.ToString();
        conferences.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[1].conferences.ToString();
        books.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[2].books.ToString();
        thesis.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[3].thesis.ToString();
        patents.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[4].paten.ToString();
        research.text = rawdata.data[0].dashboard_data[0].hasil_publikasi[5].research.ToString();

        ClientSend.SendPageType("dashboardData");
    }

    public void OnTapFilterMenu(){
        filterMenu.SetActive(true);
        ClientSend.SendPageType("filterMenu");
    }

    public void GetResearcherDetailData(string _id){
        requestPeneliti.URL = URL + "/detailpeneliti?id_peneliti=" + _id;
        StartCoroutine(requestPeneliti.RequestData((result) =>
        {
            // mengambil jumlah jurnal, conference, books, thesis, paten dan research yang ada
            FilterManager.instance.UpdateResearcherDetail(result);
        }, (error) => {
            if (error != "")
            {
                FilterManager.instance.ErrorResearcherDetail();
            }
        }));
    }

    public void OnTapSettingsMenu(){
        settingsMenu.SetActive(true);
        ClientSend.SendPageType("settingsMenu");
    }

    public void OnTapAddNodeSize(){
        if(nodeSize < maxNodeSize){
            nodeSize++;
            nodeSizeText.text = nodeSize.ToString();
            subtractNodeSizeBtn.interactable = true;
            if(nodeSize == maxNodeSize){
                addNodeSizeBtn.interactable = false;
            }
            ClientSend.SendNodeSize(nodeSize);
        }
    }

    public void OnTapSubtractNodeSize(){
        if(nodeSize > minNodeSize){
            nodeSize--;
            nodeSizeText.text = nodeSize.ToString();
            addNodeSizeBtn.interactable = true;
            if(nodeSize == minNodeSize){
                subtractNodeSizeBtn.interactable = false;
            }
            ClientSend.SendNodeSize(nodeSize);
        }
    }

    private void CopyRenderTextureTo2D(){
        if(textureToSend2D != null){
            DestroyImmediate(textureToSend2D);
        }
        
        RenderTexture oriTexture = renderCamera.targetTexture;
        RenderTexture textureToSend = new RenderTexture(oriTexture.width, oriTexture.height, oriTexture.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        renderCamera.targetTexture = textureToSend;
        renderCamera.Render();
        RenderTexture.active = textureToSend;

        textureToSend2D = new Texture2D(textureToSend.width, textureToSend.height, TextureFormat.ARGB32, false);
        textureToSend2D.ReadPixels(new Rect(0, 0, textureToSend.width, textureToSend.height), 0, 0);
        textureToSend2D.Apply();

        renderCamera.targetTexture = oriTexture;
        renderCamera.Render();
        RenderTexture.active = oriTexture;

        DestroyImmediate(textureToSend);
    }

    public float GetScreenWidth(){
        return Screen.width / Screen.dpi;
    }
    public float GetScreenHeight(){
        return Screen.height / Screen.dpi;
    }
    public byte[] GetTexture(){
        CopyRenderTextureTo2D();
        return textureToSend2D.EncodeToPNG();
    }
    public void OnTapDisconnect(){
        isDisconnectButtonPressed = true;
        Client.instance.Disconnect();
    }
    public void Disconnected(){
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
        connectMenu.SetActive(true);
        if(!isDisconnectButtonPressed){
            detailText.text = "Connection lost";
            detailText.color = new Color32(231, 65, 65, 255);
            connectButtonText.text = "Try Again";
            connectButtonText.color = new Color32(255, 255, 255, 255);
            ipInputText.color = new Color32(50, 59, 89, 255);
            
        }else{
            detailText.gameObject.SetActive(false);
            connectButtonText.text = "Connect";
            connectButtonText.color = new Color32(255, 255, 255, 255);
            ipInputText.color = new Color32(50, 59, 89, 255);
        }
        connectButton.GetComponent<Button>().interactable = true;
        ipField.interactable = true;
        isDisconnectButtonPressed = false;
    }
}