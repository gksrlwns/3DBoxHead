using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject title;
    public GameObject startBtn;
    public GameObject choiceObj;
    public GameObject NickObj;
    public GameObject optionPanel;
    Animation anim;

    private void Awake()
    {
        anim = title.GetComponent<Animation>();
    }

    public void GameStartBtn()
    {
        //타이틀과 이미지가 위로 올라가고 새로운 버튼 생성
        anim.Play();
        startBtn.SetActive(false);
        choiceObj.SetActive(true);
    }
    public void PlayBtn()
    {
        SceneManager.LoadScene("SoloGameScene");
    }

    public void CoBtn()
    {
        choiceObj.SetActive(false);
        NickObj.SetActive(true);
    }
    public void OptionBtn()
    {
        optionPanel.SetActive(true);
    }

    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
    }

}
