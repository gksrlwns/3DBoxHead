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
    public InputField signupNameInput;
    public InputField signupEmailInput;
    [Header("로그인")]
    public InputField loginIdInput;
    public InputField loginPwInput;
    public GameObject loginPanel;
    [Header("닉네임")]
    public InputField nickInput;
    public GameObject nickPanel;
    public string nickName;
    public Text nickText;
    public string nowid;
}
