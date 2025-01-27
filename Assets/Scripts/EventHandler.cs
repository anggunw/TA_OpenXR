﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Android;
using System.IO;
using TMPro;

public class EventHandler : MonoBehaviour
{
    public string URL;

    [Header("Pengaturan Node")]
    // peneliti ( secara umum )
    public GameObject ParentNode;
    public GameObject ParentCard;
    public GameObject NodePeneliti;
    public GameObject CardPeneliti;
    public GameObject ParentNode2D;
    public GameObject ParentCard2D;
    public GameObject NodePeneliti2D;
    public GameObject CardPeneliti2D;
    public float sizeCoef;
    GameObject[] listPeneliti;
    public GameObject centerEyeAnchor;
    public GameObject wall;
    public float minSizeNode;
    public float maxSizeNode;
    public float x_space;
    public float y_space;
    public float columnLength;
    public float x_start;
    public float y_start;

    [Header("Material")]
    public Material AbjadMaterial;
    public Material InisialMaterial;
    public Material F_SCIENTICS;
    public Material F_INDSYS;
    public Material F_CIVPLAN;
    public Material F_MARTECH;
    public Material F_ELECTICS;
    public Material F_CREABIZ;
    public Material F_VOCATIONS;
    public Material hoverMaterial;
    public Material pressedMaterial;

    [Header("Animation")]
    public IEnumerator animate;
    [Header("Node Sprite")]
    public Sprite AbjadSprite;
    public Sprite InisialSprite;
    public Sprite F_SCIENTICSSprite;
    public Sprite F_INDSYSSprite;
    public Sprite F_CIVPLANSprite;
    public Sprite F_MARTECHSprite;
    public Sprite F_ELECTICSSprite;
    public Sprite F_CREABIZSprite;
    public Sprite F_VOCATIONSSprite;

    [Header("Disconnect")]
    public GameObject disconnectCanvas;

    public TextMeshProUGUI debugText;

    RequestHandler requestPeneliti = new RequestHandler();

    public void ApplyURL(Config config)
    {
        if (config.GetPort() == "")
        {
            URL = config.GetUrl();
        }
        else
        {
            URL = config.GetWebAPI();
        }
    }

    public void Dashboard()
    {
        requestPeneliti.URL = URL;
        StartCoroutine(requestPeneliti.RequestData((result) =>
        {
            // mengambil jumlah jurnal, conference, books, thesis, paten dan research yang ada
            ScreenManager.instance.ShowDashboardData(result);
        }, (error) => {
            if (error != "")
            {
                Debug.Log("fail to get dashboard data");
            }
        }));   
    }

    public void getResearcherDetailData(string _id){
        requestPeneliti.URL = URL + "/detailpeneliti?id_peneliti=" + _id;
        StartCoroutine(requestPeneliti.RequestData((result) =>
        {
            // mengambil jumlah jurnal, conference, books, thesis, paten dan research yang ada
            ScreenManager.instance.UpdateResearcherDetailData(result);
        }, (error) => {
            if (error != "")
            {
                Debug.Log("fail to get researcher detail data");
            }
        }));
    }

    public void getPenelitiAbjadITS()
    {
        flushNode();
        StartCoroutine(MoveWall(2f));

        requestPeneliti.URL = URL + "/peneliti?abjad=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            float med = 100 * sizeCoef;
            int idx = 0;
            foreach (var data in result.data[0].inisial_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeAbjadPeneliti.name = data.inisial;
                NodeAbjadPeneliti.tag = "ListPenelitiAbjad";
                
                int jumlah = data.total;
                float size = 100 * sizeCoef;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.inisial;
                tambahan.nama = data.inisial;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = size;
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNodeAbjad(NodeAbjadPeneliti, size, idx);
                idx++;
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiAbjad");
            foreach(GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get abjad data");
            }
        }
        ));
    }

    public void getPenelitiAbjadITS2D()
    {
        flushNode();

        requestPeneliti.URL = URL + "/peneliti?abjad=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].inisial_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeAbjadPeneliti.name = data.inisial;
                NodeAbjadPeneliti.tag = "ListPenelitiAbjad";
                
                int jumlah = data.total;

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.inisial;
                tambahan.nama = data.inisial;
                tambahan.jumlah = jumlah;

                SpawnNode2D(NodeAbjadPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiAbjad");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get abjad 2d data");
            }
        }
        ));
    }

    public void getPenelitiInisialITS(string inisial)
    {
        flushNode();
        requestPeneliti.URL = URL + "/peneliti?abjad=" + inisial;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti);
                CardPenelitiDetail.name = data.nama;
                int jumlah = data.jumlah;
                CardPenelitiDetail.tag = "ListPenelitiInisial";

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen;
                tambahan.nama = data.nama;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiInisial");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get initial data");
            }
        }
        ));
    }

    public void getPenelitiInisialITS2D(string inisial)
    {
        flushNode();
        requestPeneliti.URL = URL + "/peneliti?abjad=" + inisial;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti2D);
                CardPenelitiDetail.name = data.nama;
                int jumlah = data.jumlah;
                CardPenelitiDetail.tag = "ListPenelitiInisial";

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen;
                tambahan.nama = data.nama;
                tambahan.jumlah = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard2D);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiInisial");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get initial 2d data");
            }
        }
        ));
    }

    public void getPenelitiFakultasITS()
    {
        flushNode();
        StartCoroutine(MoveWall(2f));

        requestPeneliti.URL = URL + "/peneliti?fakultas=none&size=true";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            int idx = result.data[0].fakultas_peneliti.Count / 2;
            float med = result.data[0].fakultas_peneliti[idx].jumlah * sizeCoef;
            foreach (var data in result.data[0].fakultas_peneliti)
            {
                GameObject NodeFakultasPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeFakultasPeneliti.name = data.nama_fakultas;
                NodeFakultasPeneliti.tag = "ListPenelitiFakultas";

                int jumlah = data.jumlah;
                float size = jumlah * sizeCoef;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeFakultasPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = data.nama_fakultas;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = size;
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNode(NodeFakultasPeneliti, size, med);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiFakultas");
            foreach (GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get fakultas data");
            }
        }
        )); 
    }

    public void getPenelitiFakultasITS2D()
    {
        flushNode();
        requestPeneliti.URL = URL + "/peneliti?fakultas=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].fakultas_peneliti)
            {
                GameObject NodeFakultasPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeFakultasPeneliti.name = data.nama_fakultas;
                NodeFakultasPeneliti.tag = "ListPenelitiFakultas";

                int jumlah = data.jumlah;

                NodeVariable tambahan = NodeFakultasPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = data.nama_fakultas;
                tambahan.jumlah = jumlah;

                // change node material
                SpawnNode2D(NodeFakultasPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiFakultas");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get fakultas 2d data");
            }
        }
        ));   
    }

    public void getPenelitiDepartemenITS(string kode_fakultas)
    {
        flushNode();
        StartCoroutine(MoveWall(2f));
        requestPeneliti.URL = URL + "/peneliti?fakultas=" + kode_fakultas.ToString() + "&size=true";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            int idx = result.data[0].departemen_peneliti.Count / 2;
            float med = result.data[0].departemen_peneliti[idx].jumlah * sizeCoef * 5f;
            foreach (var data in result.data[0].departemen_peneliti)
            {
                GameObject NodeDepartemenPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeDepartemenPeneliti.name = data.nama_departemen;
                NodeDepartemenPeneliti.tag = "ListPenelitiDepartemen";

                int jumlah = data.jumlah;
                float size = jumlah * sizeCoef * 5f;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeDepartemenPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_departemen.ToString();
                tambahan.nama = data.nama_departemen;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = size;
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNode(NodeDepartemenPeneliti, size, med);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiDepartemen");
            foreach (GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetInfoNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get departemen data");
            }
        }
        ));
    }

    public void getPenelitiDepartemenITS2D(string kode_fakultas)
    {
        flushNode();
        requestPeneliti.URL = URL + "/peneliti?fakultas=" + kode_fakultas.ToString();
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].departemen_peneliti)
            {
                GameObject NodeDepartemenPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeDepartemenPeneliti.name = data.nama_departemen;
                NodeDepartemenPeneliti.tag = "ListPenelitiDepartemen";

                int jumlah = data.jumlah;

                NodeVariable tambahan = NodeDepartemenPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_departemen.ToString();
                tambahan.nama = data.nama_departemen;
                tambahan.jumlah = jumlah;

                SpawnNode2D(NodeDepartemenPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiDepartemen");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetInfoNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get departemen 2d data");
            }
        }
        ));
    }

    public void getPenelitiDepartemenDetailITS(string kode_departemen)
    {
        flushNode();

        requestPeneliti.URL = URL + "/peneliti?departemen=" + kode_departemen.ToString();
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListPenelitiDepartemenDetail";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiDepartemenDetail");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get departemen peneliti data");
            }
        }
        ));
    }

    public void getPenelitiDepartemenDetailITS2D(string kode_departemen)
    {
        flushNode();

        requestPeneliti.URL = URL + "/peneliti?departemen=" + kode_departemen.ToString();
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti2D);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListPenelitiDepartemenDetail";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard2D);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPenelitiDepartemenDetail");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get departemen peneliti 2d data");
            }
        }
        ));
    }


    public void getGelarPenelitiITS()
    {
        flushNode();
        StartCoroutine(MoveWall(2f));

        requestPeneliti.URL = URL + "/gelar?kode=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            int idx = result.data[0].gelar_peneliti.Count / 2;
            float med = result.data[0].gelar_peneliti[idx].jumlah * sizeCoef;
            foreach (var data in result.data[0].gelar_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeAbjadPeneliti.name = data.gelar;
                NodeAbjadPeneliti.tag = "ListGelar";

                int jumlah = data.jumlah;
                float size = jumlah * sizeCoef * 0.5f;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.gelar.ToString();
                tambahan.nama = tambahan.kode_peneliti;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = size;
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNode(NodeAbjadPeneliti, size, med);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListGelar");
            foreach (GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get gelar data");
            }
        }
        ));
    }

    public void getGelarPenelitiITS2D()
    {
        flushNode();

        requestPeneliti.URL = URL + "/gelar?kode=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].gelar_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeAbjadPeneliti.name = data.gelar;
                NodeAbjadPeneliti.tag = "ListGelar";

                int jumlah = data.jumlah;

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.gelar.ToString();
                tambahan.nama = tambahan.kode_peneliti;
                tambahan.jumlah = jumlah;

                SpawnNode2D(NodeAbjadPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListGelar");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get gelar 2d data");
            }
        }
        ));
    }

    public void getGelarPenelitiDetail(string kode)
    {
        flushNode();

        requestPeneliti.URL = URL + "/gelar?kode="+kode;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListGelarDetail";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListGelarDetail");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get peneliti gelar data");
            }
        }
        ));
    }

    public void getGelarPenelitiDetail2D(string kode)
    {
        flushNode();

        requestPeneliti.URL = URL + "/gelar?kode="+kode;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti2D);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListGelarDetail";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard2D);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListGelarDetail");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get peneliti gelar 2d data");
            }
        }
        ));
    }

    public void getPublikasiFakultas()
    {
        flushNode();
        StartCoroutine(MoveWall(2f));

        requestPeneliti.URL = URL + "/publikasi?fakultas=none&size=true";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            int idx = result.data[0].fakultas_peneliti.Count / 2;
            float med = result.data[0].fakultas_peneliti[idx].jumlah * sizeCoef * 0.05f;
            foreach (var data in result.data[0].fakultas_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeAbjadPeneliti.name = data.nama_fakultas;
                NodeAbjadPeneliti.tag = "ListPublikasiFakultas";

                //jumlah disini adalah jumlah publikasi, bukan jumlah peneliti
                int jumlah = data.jumlah;
                float size = jumlah * sizeCoef * 0.05f;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = NodeAbjadPeneliti.name;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = size;
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNode(NodeAbjadPeneliti, size, med);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPublikasiFakultas");
            foreach (GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get publikasi fakultas data");
            }
        }
        ));
    }

    public void getPublikasiFakultas2D()
    {
        flushNode();

        requestPeneliti.URL = URL + "/publikasi?fakultas=none";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].fakultas_peneliti)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeAbjadPeneliti.name = data.nama_fakultas;
                NodeAbjadPeneliti.tag = "ListPublikasiFakultas";

                //jumlah disini adalah jumlah publikasi, bukan jumlah peneliti
                int jumlah = data.jumlah;

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = NodeAbjadPeneliti.name;
                tambahan.jumlah = jumlah;

                SpawnNode2D(NodeAbjadPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPublikasiFakultas");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetDefaultNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get publikasi fakultas 2d data");
            }
        }
        ));
    }

    public void getPublikasiKataKunci(string kode)
    {
        flushNode();
        StartCoroutine(MoveWall(2f));

        requestPeneliti.URL = URL + "/publikasi?fakultas=" + kode + "&size=true";
        StartCoroutine(requestPeneliti.RequestData((result) => {
            int idx = result.data[0].fakultas_publikasi.Count / 2;
            float med = int.Parse(result.data[0].fakultas_publikasi[idx].df) * sizeCoef * 2;
            foreach (var data in result.data[0].fakultas_publikasi)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti);
                NodeAbjadPeneliti.name = data.kata_kunci;
                NodeAbjadPeneliti.tag = "ListPublikasiKataKunci";

                int jumlah = int.Parse(data.df);
                float size = jumlah * sizeCoef * 2f;
                size = Mathf.Clamp(size, minSizeNode, maxSizeNode);

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = NodeAbjadPeneliti.name;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = float.Parse(data.idf);
                tambahan.ukuran2 = new Vector3(size, size, size);

                SpawnNode(NodeAbjadPeneliti, size, med);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPublikasiKataKunci");
            foreach (GameObject node in listPeneliti)
            {
                animate = animateNode(node, node.GetComponent<NodeVariable>().ukuran2);
                StartCoroutine(animate);
            }
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetInfoNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get publikasi kata kunci data");
            }
        }
        ));
    }

    public void getPublikasiKataKunci2D(string kode)
    {
        flushNode();

        requestPeneliti.URL = URL + "/publikasi?fakultas=" + kode;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].fakultas_publikasi)
            {
                GameObject NodeAbjadPeneliti = (GameObject)Instantiate(NodePeneliti2D);
                NodeAbjadPeneliti.name = data.kata_kunci;
                NodeAbjadPeneliti.tag = "ListPublikasiKataKunci";

                int jumlah = int.Parse(data.df);

                NodeVariable tambahan = NodeAbjadPeneliti.AddComponent<NodeVariable>();
                tambahan.kode_alternate = data.kode_fakultas.ToString();
                tambahan.kode_peneliti = data.kode_fakultas.ToString();
                tambahan.nama = NodeAbjadPeneliti.name;
                tambahan.jumlah = jumlah;

                SpawnNode2D(NodeAbjadPeneliti);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListPublikasiKataKunci");
            ScreenManager.instance.SetLoadingNodeScreen(false);
            ScreenManager.instance.SetInfoNodeScreen(true);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get publikasi kata kunci 2d data");
            }
        }
        ));
    }

    public void getKataKunciPeneliti(string fakultas, string katakunci)
    {
        flushNode();

        requestPeneliti.URL = URL + "/publikasi?fakultas=" + fakultas + "&katakunci=" + katakunci;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListKataKunciPeneliti";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;
                tambahan.ukuran = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListKataKunciPeneliti");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get peneliti kata kunci data");
            }
        }
        ));
    }

    public void getKataKunciPeneliti2D(string fakultas, string katakunci)
    {
        flushNode();

        requestPeneliti.URL = URL + "/publikasi?fakultas=" + fakultas + "&katakunci=" + katakunci;
        StartCoroutine(requestPeneliti.RequestData((result) => {
            foreach (var data in result.data[0].nama_peneliti)
            {
                GameObject CardPenelitiDetail = (GameObject)Instantiate(CardPeneliti2D);
                CardPenelitiDetail.name = data.nama;
                CardPenelitiDetail.tag = "ListKataKunciPeneliti";

                int jumlah = data.jumlah;

                NodeVariable tambahan = CardPenelitiDetail.AddComponent<NodeVariable>();
                tambahan.kode_peneliti = data.kode_dosen.ToString();
                tambahan.nama = CardPenelitiDetail.name;
                tambahan.jumlah = jumlah;

                SpawnCard(CardPenelitiDetail, ParentCard2D);
            }
            listPeneliti = GameObject.FindGameObjectsWithTag("ListKataKunciPeneliti");
            ScreenManager.instance.SetLoadingCardScreen(false);
        }, error => {
            if (error != "")
            {
                Debug.Log("fail to get peneliti kata kunci 2d data");
            }
        }
        ));
    }

    private IEnumerator MoveWall(float time)
    {
        Vector3 startingPos  = centerEyeAnchor.transform.position;
        wall.transform.position = startingPos;
        Vector3 finalPos = startingPos + centerEyeAnchor.transform.forward * 1.5f;
        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            wall.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        wall.transform.LookAt(wall.transform.position + centerEyeAnchor.transform.rotation * Vector3.forward, centerEyeAnchor.transform.rotation * Vector3.up);
    }

    public void SpawnNodeAbjad(GameObject node, float size, int i)
    {
        node.transform.SetParent(ParentNode.transform, false);
        float x = ParentNode.transform.position.x + x_start;
        float y = ParentNode.transform.position.y + y_start;
        float z = ParentNode.transform.position.z;
        node.transform.localScale = new Vector3(0f, 0f, 0f);

        node.transform.localPosition = new Vector3(x + (x_space * (i % columnLength)), y + (-y_space * Mathf.FloorToInt(i / columnLength)), z);

        node.GetComponentInChildren<TextMeshProUGUI>().text = node.name;

        NodeVariable nodeVariable = node.GetComponent<NodeVariable>();

        Material abjadMat = Resources.Load("Alphabet Materials/" + node.name, typeof(Material)) as Material;
        node.GetComponent<Renderer>().material = abjadMat;
        nodeVariable.defaultMaterial = abjadMat;
        nodeVariable.hoverMaterial = Resources.Load("Alphabet Materials/" + node.name + "-hover", typeof(Material)) as Material;
        nodeVariable.pressedMaterial = Resources.Load("Alphabet Materials/" + node.name + "-pressed", typeof(Material)) as Material;

        // turn off text panel on top of sphere
        node.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        node.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        // make collider smaller
        node.GetComponent<SphereCollider>().radius = 0.5f;
    }

    public void SpawnNode(GameObject node, float size, float med)
    {
        node.transform.SetParent(ParentNode.transform, false);
        float x = ParentNode.transform.position.x;
        float y = ParentNode.transform.position.y;
        float z = ParentNode.transform.position.z;
        node.transform.localScale = new Vector3(0f, 0f, 0f);

        if(size < med){
            node.transform.localPosition = new Vector3(Random.Range(x - 5f, x + 5f), Random.Range(y, y + 0.5f), z);
        }else{ 
            node.transform.localPosition = new Vector3(Random.Range(x - 5f, x + 5f), Random.Range(y, y + 0.5f), z + 2f);
        }

        node.GetComponentInChildren<TextMeshProUGUI>().text = node.name;

        NodeVariable nodeVariable = node.GetComponent<NodeVariable>();

        if (node.CompareTag("ListPenelitiInisial"))
        {
            node.GetComponent<Renderer>().material = InisialMaterial;
            nodeVariable.hoverMaterial = hoverMaterial;
        }
        else if (node.CompareTag("ListPenelitiFakultas") || node.CompareTag("ListPenelitiDepartemen") || node.CompareTag("ListPublikasiFakultas") || node.CompareTag("ListPublikasiKataKunci") || node.CompareTag("ListPublikasiKataKunci"))
        {
            switch (int.Parse(node.GetComponent<NodeVariable>().kode_alternate))
            {
                case 1:
                    node.GetComponent<Renderer>().material = F_SCIENTICS;
                    break;
                case 2:
                    node.GetComponent<Renderer>().material = F_INDSYS;
                    break;
                case 3:
                    node.GetComponent<Renderer>().material = F_ELECTICS;
                    break;
                case 4:
                    node.GetComponent<Renderer>().material = F_CIVPLAN;
                    break;
                case 5:
                    node.GetComponent<Renderer>().material = F_CREABIZ;
                    break;
                case 6:
                    node.GetComponent<Renderer>().material = F_MARTECH;
                    break;
                case 8:
                    node.GetComponent<Renderer>().material = F_VOCATIONS;
                    break;
                default:
                    node.GetComponent<Renderer>().material = AbjadMaterial;
                    break;

            }
            nodeVariable.hoverMaterial = hoverMaterial;
        }
        else 
        {
            nodeVariable.hoverMaterial = hoverMaterial;
        }
        nodeVariable.defaultMaterial = node.GetComponent<Renderer>().material;
        nodeVariable.pressedMaterial = pressedMaterial;
    }

    public void SpawnNode2D(GameObject node)
    {
        node.transform.SetParent(ParentNode2D.transform, false);
        node.GetComponentInChildren<TextMeshProUGUI>().text = node.name;

        if (node.CompareTag("ListPenelitiAbjad"))
        {
            Sprite abjadSprite = Resources.Load("Alphabet Sprites/" + node.name, typeof(Sprite)) as Sprite;
            node.GetComponentInChildren<Image>().sprite = abjadSprite;
            Sprite abjadHoverSprite = Resources.Load("Alphabet Sprites/" + node.name + "-hover", typeof(Sprite)) as Sprite;
            SpriteState ss = new SpriteState();
            ss.highlightedSprite = abjadHoverSprite;
            node.GetComponentInChildren<Button>().spriteState = ss;

            // turn off text on top of node
            node.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (node.CompareTag("ListPenelitiInisial"))
        {
            node.GetComponentInChildren<Image>().sprite = InisialSprite;
        }
        else if (node.CompareTag("ListPenelitiFakultas") || node.CompareTag("ListPenelitiDepartemen") || node.CompareTag("ListPublikasiFakultas") || node.CompareTag("ListPublikasiKataKunci") || node.CompareTag("ListPublikasiKataKunci"))
        {
            switch (int.Parse(node.GetComponentInChildren<NodeVariable>().kode_alternate))
            {
                case 1:       
                    node.GetComponentInChildren<Image>().sprite = F_SCIENTICSSprite;
                    break;
                case 2:
                    node.GetComponentInChildren<Image>().sprite = F_INDSYSSprite;
                    break;
                case 3:
                    node.GetComponentInChildren<Image>().sprite = F_ELECTICSSprite;
                    break;
                case 4:
                    node.GetComponentInChildren<Image>().sprite = F_CIVPLANSprite;
                    break;
                case 5:
                    node.GetComponentInChildren<Image>().sprite = F_CREABIZSprite;
                    break;
                case 6:
                    node.GetComponentInChildren<Image>().sprite = F_MARTECHSprite;
                    break;
                case 8:
                    node.GetComponentInChildren<Image>().sprite = F_VOCATIONSSprite;
                    break;
                default:
                    node.GetComponentInChildren<Image>().sprite = AbjadSprite;
                    break;
            }
        }
    }

    public void SpawnCard(GameObject penelitiCard, GameObject parentCardReference)
    {
        penelitiCard.transform.SetParent(parentCardReference.transform, false);
        penelitiCard.GetComponentInChildren<TextMeshProUGUI>().text = penelitiCard.name;
    }

    public IEnumerator animateNode(GameObject node, Vector3 nodeScale)
    {
        if(!ScreenManager.instance.isSearching){
            Destroy(node);
            yield break;
        }
        float timeElapsed = 0f;
        float waitTime = 2f;
        while (node != null && node.transform.localScale.x < nodeScale.x){
            node.transform.localScale = Vector3.Lerp(node.transform.localScale, nodeScale, (timeElapsed / waitTime));

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if(node != null){
            node.transform.localScale = nodeScale;
        }
        yield return null;
    }

    public void resizeNode(float zoom)
    {
        sizeCoef = zoom / 3000;
    }    

    public void flushNode()
    {
        if(listPeneliti != null)
        {
            foreach (GameObject node in listPeneliti)
            {
                Destroy(node);
            }
            listPeneliti = null;
        }

        foreach (Transform child in ParentNode2D.transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in ParentNode.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void GetNode(string _nodeId, string _nodeId2, string _tagName){
        if (_tagName == "ListPenelitiAbjad"){
            Manager.instance.getPenelitiInisialITS(_nodeId);
        } 
        else if(_tagName == "ListPenelitiFakultas"){
            Manager.instance.getPenelitiDepartemenITS(_nodeId);
        }
        else if(_tagName == "ListPenelitiDepartemen"){
            Manager.instance.getPenelitiDepartemenDetailITS(_nodeId);
        }    
        else if(_tagName == "ListGelar"){
            Manager.instance.getGelarPenelitiDetail(_nodeId);
        }
        else if(_tagName == "ListPublikasiFakultas"){
            Manager.instance.getPublikasiKataKunci(_nodeId);
        }
        else if (_tagName == "ListPublikasiKataKunci"){
            Manager.instance.getKataKunciPeneliti(_nodeId, _nodeId2);
        }
    }

    public bool IsListPenelitiEmpty(){
        if(listPeneliti == null || listPeneliti.Length == 0){
            return true;
        }else{
            return false;
        }
    }
}   