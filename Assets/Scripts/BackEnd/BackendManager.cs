using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class BackendManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess()) Debug.Log("초기화 성공");
        else Debug.Log("초기화 실패");
    }
}
