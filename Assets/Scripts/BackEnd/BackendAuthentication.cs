using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class BackendAuthentication : MonoBehaviour
{
    [Header("회원가입")]
    public GameObject signuppanal;
    public InputField signupIdInput;
    public InputField signupPwInput;
    public InputField signupNickInput;

    [Header("로그인")]
    public GameObject loginPanel;
    public InputField loginIdInput;
    public InputField loginPwInput;
    public GameObject userObj;

    [Header("닉네임")]
    string nickName;
    public Text userNickText;
    public Text errorText;

    public void SetSignUpPanel(bool isActive)
    {
        if (isActive == false)
        {
            signupIdInput.text = "";
            signupPwInput.text = "";
            signupNickInput.text = "";
        }
        signuppanal.SetActive(isActive);
    }
    public void SetSignInPanel(bool isActive)
    {
        if (isActive == false)
        {
            loginIdInput.text = "";
            loginPwInput.text = "";
        }
        loginPanel.SetActive(isActive);
    }
    public void BackendSignUp()
    {
        if (signupIdInput.text == "")
        {
            print("아이디를 입력해주세요!");
            StartCoroutine(ErrorText("아이디를 입력해주세요!"));
            return;
        }

        if (signupPwInput.text == "")
        {
            print("비밀번호를 입력해주세요!");
            StartCoroutine(ErrorText("비밀번호를 입력해주세요!"));
            return;
        }

        if (signupNickInput.text == "") signupNickInput.text = $"User{Random.Range(0, 100)}";
        //if (signupNickInput.text == "")
        //{
        //    print("닉네임을 적어주세요!");
        //    return;
        //}


        BackendReturnObject bro =
            Backend.BMember.CustomSignUp
            (signupIdInput.text,
            signupPwInput.text);
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다");
            UserInfoInsert();
        }
        else
        {
            print(bro.GetMessage());
            StartCoroutine(ErrorText(bro.GetMessage()));
        }
    }
    public void UserInfoInsert()
    {
        //아이디, 비밀번호, 이름, 이메일의 정보를 뒤끝 서버에 업로드(user)
        Param param = new Param();
        param.Add("id", signupIdInput.text);
        param.Add("pw", signupPwInput.text);
        param.Add("nickname", signupNickInput.text);
        param.Add("highscore", 0);

        BackendReturnObject bro = Backend.GameData.Insert("user", param);
        if (bro.IsSuccess())
        {
            //업로드에 성공 했을 때
            StartCoroutine(ErrorText("회원가입시 유저정보 업로드에 성공하였습니다!"));
            print("회원가입시 유저정보 업로드에 성공하였습니다!");
            SetSignUpPanel(false);
        }
        else
        {
            //업로드에 실패했을 때
            print("회원가입시 유저정보 업로드에 실패하였습니다!");
            print(bro.GetMessage());
            StartCoroutine(ErrorText(bro.GetMessage()));
        }
    }
    public void BackendSignIn()
    {
        //로그인도 id와 pw가 필요하다. : 인풋필드로 받아온다.
        BackendReturnObject bro = Backend.BMember.CustomLogin(loginIdInput.text, loginPwInput.text);
        if (bro.IsSuccess())
        {
            Debug.Log("로그인에 성공했습니다");
            loginPanel.SetActive(false);
            GetNickname();
            BackendManager.instance.id = loginIdInput.text;
            BackendManager.instance.nickname = nickName;
            userNickText.text = nickName;
            userObj.SetActive(true);
        }
        else
        {
            //에러코드까지 나오게 하면 더 좋다
            Debug.Log(bro.GetMessage());
            StartCoroutine(ErrorText(bro.GetMessage()));
        }
    }
    void GetNickname()
    {
        Where where = new Where();
        where.Equal("id", loginIdInput.text);
        nickName = BackendManager.instance.BackendGetInfo("user", where, "nickname");
    }
    IEnumerator ErrorText(string error)
    {
        errorText.text = error;
        yield return new WaitForSeconds(1f);
        errorText.text = "";
    }
    
}
