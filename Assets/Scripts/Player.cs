using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject CameraTr;
    public GameObject[] weapons;

    public bool[] hasweapons;
    public float speed;

    float hAxis;
    float vAxis;

    bool shiftDown;
    bool spaceDown;
    bool rDown;
    

    bool isRun;
    bool isDodge;
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //SetCamPosition();
    }

    // Update is called once per frame
    void Update()
    {

        GetInput();
        Move();
        Turn();
        Dodge();
        Reload();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        shiftDown = Input.GetButton("Walk");
        spaceDown = Input.GetButtonDown("Dodge");
        rDown = Input.GetButtonDown("Reload");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;
        transform.position += moveVec * (shiftDown ? speed/2 : speed) * Time.deltaTime;
        anim.SetBool("isWalk", shiftDown);
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        if(spaceDown && !isDodge && !shiftDown)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
        
    }

    void DodgeOut()
    {
        speed /= 2;
        isDodge = false;
    }

    void Reload()
    {
        if(rDown && moveVec == Vector3.zero && !isDodge)
        {
            anim.SetTrigger("doReload");
        }
    }
    void SetCamPosition()
    {
        Camera.main.transform.parent = CameraTr.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
}
