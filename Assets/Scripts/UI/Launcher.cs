using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] public PhotonView Player;
    [SerializeField] public Transform spawnPoint;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        spawnPoint = GameObject.Find("SpawnPoint").transform;
    }

    public override void OnConnectedToMaster() 
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() 
    {
        PhotonNetwork.Instantiate(Player.name, spawnPoint.position, spawnPoint.rotation);
    }
}
