using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class PhotonChat : MonoBehaviour
{
    public Transform[] playerSpots;
    public GameObject chatOBJ;
    public PhotonView _photonView;
    public InputField chatInput;
    public Text chattingText;


    private void Start()
    {
        var player = PhotonNetwork.Instantiate("PhotonPlayer", playerSpots[0].position, playerSpots[0].rotation);
        player.GetComponent<PhotonPlayer>().SetPlayer();
    }
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
        chatOBJ.SetActive(true);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainScene");
        //PhotonNetwork.LoadLevel("MainScene");
    }
    public void SendMessage()
    {
        //if 1, 채팅을 보낼 내용이 없으면? : 그냥 채팅 송신을 하지 말자.
        if (chatInput.text == "") return;
        //우리가 보낼 채팅을, 누가 보냇느냐와 함께 RPC함수에 전송
        _photonView.RPC("ReceiveMessage", RpcTarget.AllBuffered,
            PhotonNetwork.LocalPlayer.NickName, chatInput.text);
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
        string line = "[" + nickname + "]" + ":" + msg;
        line = $"[{nickname}]: {msg}";

        chattingText.text = chattingText.text + "\n" + line;

    }
    #endregion

}
