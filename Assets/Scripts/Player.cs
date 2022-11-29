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
    bool s1Down;
    bool s2Down;
    bool s3Down;
    

    bool isRun;
    bool isDodge;
    bool isSwap;

    int equipWeaponIndex = -1;
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;
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
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        shiftDown = Input.GetButton("Walk");
        spaceDown = Input.GetButtonDown("Dodge");
        rDown = Input.GetButtonDown("Reload");
        s1Down = Input.GetButtonDown("Swap1");
        s2Down = Input.GetButtonDown("Swap2");
        s3Down = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge) moveVec = dodgeVec;
        if (isSwap) moveVec = Vector3.zero;
        transform.position += moveVec * (shiftDown ? speed/2 : speed) * Time.deltaTime;
        anim.SetBool("isWalk", shiftDown);
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Swap()
    {
        if (s1Down && (!hasweapons[0] || equipWeaponIndex == 0)) return;
        if (s2Down && (!hasweapons[1] || equipWeaponIndex == 1)) return;
        if (s3Down && (!hasweapons[2] || equipWeaponIndex == 2)) return;

        int weaponIndex = -1;
        if (s1Down && hasweapons[0]) weaponIndex = 0;
        if (s2Down && hasweapons[1]) weaponIndex = 1;
        if (s3Down && hasweapons[2]) weaponIndex = 2;
        if((s1Down || s2Down || s3Down) && !isDodge)
        {
            for(int i = 0; i < weapons.Length; i++)
            {
                weapons[i].SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            weapons[weaponIndex].SetActive(true);
            
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.5f);
        }
        
    }

    void SwapOut()
    {
        isSwap = false;
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        if(spaceDown && !isDodge && !shiftDown &!isSwap)
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
        if(rDown && moveVec == Vector3.zero && !isDodge & !isSwap)
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            nearObject = other.gameObject;
            Item item = nearObject.GetComponent<Item>();
            int weaponIndex = item.value;
            hasweapons[weaponIndex] = true;
            Destroy(other.gameObject);
        }
    }
}
