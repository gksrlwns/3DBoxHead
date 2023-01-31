using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpots;
    public GameObject chatOBJ;
    public PhotonView _photonView;
    public ScrollRect scrollRect;
    public InputField chatInput;
    public Text chattingText;
    public Text roomInfoText;
    public GameObject startBtn;
    public GameObject multiGameManager;
    
    GameObject player;

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        player = PhotonNetwork.Instantiate
            ("PhotonPlayer", playerSpots[(int)PhotonNetwork.CurrentRoom.PlayerCount-1].position,
            playerSpots[(int)PhotonNetwork.CurrentRoom.PlayerCount-1].rotation);
        player.GetComponent<PhotonPlayer>().SetPlayer();
        //player.GetComponent<PhotonPlayer>().playerCanvas.SetActive(false);
    }
    private void Update()
    {
        if (!player) return;
        roomInfoText.text = $"{PhotonNetwork.CurrentRoom.Name} : {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        if(Input.GetKey(KeyCode.Return) && !chatInput.isFocused)
        {
            SendMessage();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        if ((int)PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
            startBtn.SetActive(true);
        else
            startBtn.SetActive(false);

    }
    public void GameStartBtn()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("MultiGameScene");
    }
    public override void OnJoinedRoom()
    {
        _photonView.RPC("ReceiveMessage", RpcTarget.All,
            PhotonNetwork.LocalPlayer.NickName, "님이 입장하셨습니다.");
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnClickLeaveRoom()
    {
        //PhotonNetwork.LoadLevel("MainScene");
        _photonView.RPC("ReceiveMessage", RpcTarget.AllBuffered,
            PhotonNetwork.LocalPlayer.NickName, "님이 퇴장하셨습니다.");
        PhotonNetwork.LeaveRoom();
        
    }
    #region 채팅기능 관리
    
    public void SendMessage()
    {
        //if 1, 채팅을 보낼 내용이 없으면? : 그냥 채팅 송신을 하지 말자.
        if (chatInput.text == "") return;
        //우리가 보낼 채팅을, 누가 보냇느냐와 함께 RPC함수에 전송
        _photonView.RPC("ReceiveMessage", RpcTarget.All,
            PhotonNetwork.LocalPlayer.NickName, chatInput.text);
        //if 2,채팅을 보내고 난 다음에, 입력창은?
        chatInput.ActivateInputField();
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
