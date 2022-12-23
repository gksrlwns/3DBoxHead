using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject title;
    public GameObject startBtn;
    public GameObject soloBtn;
    public GameObject coBtn;
    Animation anim;

    private void Awake()
    {
        anim = title.GetComponent<Animation>();
    }

    public void GameStartBtn()
    {
        //타이틀과 이미지가 위로 올라가고 새로운 버튼 생성
        anim.Play();
        startBtn.gameObject.SetActive(false);
        soloBtn.SetActive(true);
        coBtn.SetActive(true);
    }
    public void SoloPlayBtn()
    {
        
    }
    public void CoPlayBtn()
    {

    }
}
