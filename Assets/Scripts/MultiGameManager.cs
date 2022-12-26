using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiGameManager : MonoBehaviour
{
    public Transform[] playerSpots;
    private void Start()
    {
        var player = PhotonNetwork.Instantiate("PhotonPlayer", playerSpots[PhotonNetwork.CurrentRoom.PlayerCount].position, playerSpots[PhotonNetwork.CurrentRoom.PlayerCount].rotation);
    }
}
