﻿// using System.Collections;
// using System.Collections.Generic;
// using System.Net;
// using UnityEngine;

// public class ClientHandle : MonoBehaviour
// {
//     public static void Welcome(PacketNetwork _packet){
//         string _msg = _packet.ReadString();
//         int _myId = _packet.ReadInt();

//         Debug.Log($"Message from server: {_msg}");
//         Client.instance.myId = _myId;

//         // if smartphone
//         if(Client.instance.myId == 2){
//             //UIManager.instance.ClientConnected(_msg);
//         }

//         //Send welcome received packet
//         ClientSend.WelcomeReceived();
//     }

//     public static void PhoneStatusReceived(PacketNetwork _packet){
//         Manager.instance.SetVirtualSmartphoneCanvasActive();
//     }

//     // receive command from server
//     // public static void TouchReceived(PacketNetwork _packet){
//     //     string _touch = _packet.ReadString();

//     //     if (_touch == "touch"){
//     //         Debug.Log("touch detected");
//     //         ScreenManager.instance.TouchButton();
//     //     }
//     // }

//     // public static void SwipeReceived(PacketNetwork _packet){
//     //     string _swipeType = _packet.ReadString();
//     //     Debug.Log("receive swipe type " + _swipeType);
//     //     ScreenManager.instance.SetScreenMode(_swipeType);
//     // }

//     // public static void ScrollSpeedReceived(PacketNetwork _packet){
//     //     float _scrollSpeed = _packet.ReadFloat();
//     //     ScreenManager.instance.SetScroll(_scrollSpeed);
//     // }

//     // public static void RotationReceived(PacketNetwork _packet){
//     //     float _x = _packet.ReadFloat();
//     //     float _y = _packet.ReadFloat();
//     //     float _z = _packet.ReadFloat();
//     //     float _w = _packet.ReadFloat();
//     //     SmartphoneGyro.instance.SetPhoneRotation(_x, _y, _z, _w);
//     // }    
// }
