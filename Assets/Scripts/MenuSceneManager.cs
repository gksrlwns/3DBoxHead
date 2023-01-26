using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject title;
    public GameObject startBtn;
    public GameObject loginChoiceObj;
    public GameObject modeChoiceObj;
    public GameObject NickObj;
    public GameObject optionPanel;
    Animation anim;

    private void Awake()
    {
        anim = title.GetComponent<Animation>();
        
    }
    private void Start()
    {
        //메인화면 시작시 UI들을 고정시키기 위함(로그인 상태, 포톤 접속 상태 확인)
        if (BackendManager.instance.nickname != "")
        {

        }
    }
    public void GameStartBtn()
    {
        //타이틀과 이미지가 위로 올라가고 새로운 버튼 생성
        anim.Play();
        startBtn.SetActive(false);
        loginChoiceObj.SetActive(true);
    }
    public void ChoiceModeBtn()
    {
        loginChoiceObj.SetActive(false);
        modeChoiceObj.SetActive(true);
    }
    public void PlayBtn()
    {
        SceneManager.LoadScene("SoloGameScene");
    }

    public void OptionBtn(bool isActive)
    {
        optionPanel.SetActive(isActive);
    }

}
