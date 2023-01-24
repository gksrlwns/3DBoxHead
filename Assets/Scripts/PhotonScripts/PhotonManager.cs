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

    [Header("백엔드")]
    public BackendManager backendManager;

    Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    bool isConnect = false;
    private void Awake()
    {
        //_photonView = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        print("00. 포톤 매니저 시작");
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
        PhotonNetwork.LocalPlayer.NickName = backendManager.nickname;
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

    public void JoinRoom()
    {
        if(isConnect)
        {
            PhotonNetwork.JoinRoom("room_1");
            print("방에 입장하였습니다.");
        }
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
        isConnect = true;
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
            PhotonNetwork.LoadLevel("RoomScene");
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

    #region 채팅기능 관리
    //PhotonView(컴포넌트/스크립트) : 포톤에 접속할 수 있는 기본단위(유저 개개인)

    //1. 채팅이 올라오는 텍스트 창
    //2. 채팅을 입력할 수 있는 입력창
    //3. 채팅을 보낼 수 있는 보내기 버튼

    //0. 채팅에 성공적으로 접속했을 때 실행할 함수
    //1. 채팅을 보내는 함수(RPC 보내는 함수)
    //2. 1의 보내진 함수를 토대로, 적어진 채팅을 띄워주는 함수(RPC)

    void InitChat()
    {
        //1. 접속창(닉네임을 입력받아서, 포톤서버에 접속할 수 있도록 하는 창)을 꺼ㅝ야한다.
        //2. 채팅요소들을 ON(채팅창, 입력창, 보내기버튼)
        connectOBJ.SetActive(false);
        chatOBJ.SetActive(true);
    }

    public void SendMessage()
    {
        //if 1, 채팅을 보낼 내용이 없으면? : 그냥 채팅 송신을 하지 말자.
        if (chatInput.text == "") return;
        //우리가 보낼 채팅을, 누가 보냇느냐와 함께 RPC함수에 전송
        //_photonView.RPC("ReceiveMessage", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
        //if 2,채팅을 보내고 난 다음에, 입력창은?
        chatInput.text = "";
    }

    [PunRPC]
    public void ReceiveMessage(string nickname, string msg)
    {
        //채팅방의 구성원들이 채팅을 쳤을 때, 채팅창에 띄워주도록 하는 RPC 함수
        //통짜 텍스트 형식에서 채팅창처럼 보이도록 만드려면, 한줄 한줄, 띄어서 적어야
        //채팅창처럼 보이게 만들 수 있다.

        //1. 채팅창에 새로이 넣을 한 줄을 만들어 준다.
        //2. 1에서 만든 줄을, 기존 채팅창의 텍스트에서 한 줄을 내리고, 아래에 삽입
        //(기존에 있는 텍스트는 보존)

        //[닉네임] : 채팅쓴 내용 
        //string line = "[" + nickname + "]" + ":" + msg;
        string line = $"[{nickname}]: {msg}";

        chattingText.text = chattingText.text + "\n" + line;

    }
    #endregion
}

