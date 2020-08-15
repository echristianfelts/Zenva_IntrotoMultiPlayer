using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{


    public static NetworkManager instance;
    public ServerSettings serverSettings;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to the Master Server");
    //    CreateRoom("testroom");
    //}

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }


    //public override void OnCreatedRoom()
    //{
    //    Debug.Log("Created Room:" + PhotonNetwork.CurrentRoom.Name);
    //}

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

}

