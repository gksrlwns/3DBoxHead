using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class BackendManager : MonoBehaviour
{
    public static BackendManager instance;
    public string nickname;
    public string id;
    public bool isPhoton;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        //var obj = FindObjectsOfType<BackendManager>();
        //if (obj.Length == 1)
        //    DontDestroyOnLoad(this.gameObject);
        //else
        //    Destroy(this.gameObject);
    }

    private void Start()
    {
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess()) Debug.Log("초기화 성공");
        else Debug.Log("초기화 실패");
    }
    public string BackendGetInfo(string tablename, Where _where, string colunmName)
    {
        var bro = Backend.GameData.Get(tablename, _where);
        if (bro.IsSuccess())
        {
            //Get이 성공 했을때 - 받아온 데이터 : Json <- 이 친구를 파싱!
            JsonData returnvalue = bro.GetReturnValuetoJSON();
            //print(returnvalue.ToJson());
            //print(returnvalue["rows"].ToJson());
            //print(returnvalue["rows"][0]["highscore"].ToJson());
            //print(returnvalue["rows"][0]["highscore"][0].ToJson());

            string value = returnvalue["rows"][0][colunmName][0].ToString();
            return value;

        }
        else
        {
            //Get이 실패를 했을 때!
            print(bro.GetMessage());
            return null;
        }
    }
    //특정 테이블에 삽입된 정보를 수정 해주는 함수
    //테이블이름 / Param / Where
    public void BackendUpdateInfo(string tablename, Where _where, Param _param)
    {
        BackendReturnObject bro = Backend.GameData.Update(tablename, _where, _param);
        if (bro.IsSuccess())
        {
            print($"BackendUpdateInfo Success : in {tablename}");
        }
        else
        {
            print(bro.GetMessage());
        }
    }
}
