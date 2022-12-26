using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiGameManager : MonoBehaviour
{
    public Transform[] playerSpots;
    private void Start()
    {
        var player = PhotonNetwork.Instantiate("PhotonPlayer", playerSpots[0].position, playerSpots[0].rotation);
    }
}
