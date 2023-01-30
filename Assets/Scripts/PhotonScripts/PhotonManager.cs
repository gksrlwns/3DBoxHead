using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//using Photon.Chat;



public class PhotonManager : MonoBehaviourPunCallbacks
{
    //public PhotonView _photonView;
    public InputField nickInput;
    public GameObject connectOBJ;
    public GameObject chatOBJ;
    public GameObject choiceObj;
    [Header("로비")]
    public GameObject LobbyObj;
    public GameObject roomPrefab;
    public Text userText;
    public Transform scrollContent;
    [Header("채팅 요소")]
    public Text chattingText;
    public InputField chatInput;

    Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    //bool isConnect;
    private void Awake()
    {
        //_photonView = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
   
    public void CloseLobbyPanel()
    {
        LobbyObj.SetActive(false);
        choiceObj.SetActive(true);
        //창 닫기를 누르면 포톤네트워크의 접속을 끊어야함
        Disconnect();
    }
    public void Disconnect() => PhotonNetwork.Disconnect();

    public void OnClickConnect()
    {
        //if (nickInput.text == "") nickInput.text = $"User{Random.Range(0, 100)}";
        //PhotonView 스크립트에 Controller에 저장됨
        PhotonNetwork.LocalPlayer.NickName = BackendManager.instance.nickname;
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnClickCreateRoom()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 2;
        //방 들어가기
        PhotonNetwork.CreateRoom($"room{Random.Range(0, 50)}", ro);
    }

    private void Update()
    {
        //콘솔창 흰색print/Debug.Log
        //콘솔창 노란색 
        Debug.LogWarning(PhotonNetwork.NetworkClientState);//현재 포톤상의 네트워크 상황을 알 수 있음

    }
    #region 포톤 서버 접속코드

    public override void OnConnectedToMaster()
    {
        //ConnectUsingSettings가 성공적으로 실행 되었을 때 호출
        print("01. 포톤 서버에 접속");
        string nick = PhotonNetwork.LocalPlayer.NickName;
        print($"당신의 이름은 {nick} 입니다.");
        userText.text = nick;
        //isConnect = true;
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        print("로비 연결");
        connectOBJ.SetActive(false);
        LobbyObj.SetActive(true);
    }

    //방에 들어가는데 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("02. 랜덤 룸 접속 실패");
        Debug.LogWarning($"OnJoinRoomFailed! : {message}");
        
        //방 만들기
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 2;
        //방 들어가기
        PhotonNetwork.CreateRoom($"room{Random.Range(0, 50)}", ro);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성");
    }
    public override void OnJoinedRoom()
    {
        print("04. 방 입장 완료");
        if (PhotonNetwork.IsMasterClient)
            //PhotonNetwork.LoadLevel("RoomScene");
            PhotonNetwork.LoadLevel("MultiGameScene");
        //JoinRoom이 성공적으로 실행되엇을 때 호출
        //base.OnJoinedRoom();
        //InitChat();
        //GameObject player = PhotonNetwork.Instantiate("PhotonPlayer", Vector3.zero, Quaternion.identity);

    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach(var room in roomList)
        {
            if(room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    var roomClone = Instantiate(roomPrefab, scrollContent);
                    roomClone.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, roomClone);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }

            }
        }
    }
    
    //public override void OnJoinedLobby()
    //{
    //    //JoinLobby가 성공적으로 실행되었을 때 호출
    //    PhotonNetwork.JoinRoom("1");// 비어있으면 안됨
    //}





    #endregion

}

