using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject CameraTr;
    public GameObject Camera2Tr;
    public GameObject[] weapons;
    public bool[] hasweapons;
    [Header("속도")]
    public float moveSpeed;
    public float mouseSensitivity;

    [Header("카메라")]
    public float cameraRotationMaxLimit;
    public float cameraRotationMinLimit;
    public float currentCameraRotation = 0;
    [Header("아이템")]
    public int ammo;
    public int coin;
    public int health;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;

    float hAxis;
    float vAxis;

    bool shiftDown;
    bool spaceDown;
    bool rDown;
    bool s1Down;
    bool s2Down;
    bool s3Down;
    bool fDown;
    bool f2Down;
    

    bool isRun;
    bool isReload;
    bool isDodge;
    bool isSwap;
    bool isFireReady;

    int equipWeaponIndex = -1;
    float fireDelay;
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    Weapon equipWeapon;
    Camera playerCamera;

    private void Awake()
    {
        SetCamPosition();
        playerCamera = GetComponentInChildren<Camera>();
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
        Move();
        CameraRotation();
        AimSetCamPosition();
        Turn();
        Dodge();
        Reload();
        Swap();
        Attack();
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
        fDown = Input.GetButton("Fire1");
        f2Down = Input.GetButton("Fire2");
    }

    void Move()
    {
        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        Vector3 moveX = transform.right * hAxis;
        Vector3 moveZ = transform.forward * vAxis;
        moveVec = (moveX + moveZ).normalized * (shiftDown ? moveSpeed / 2 : moveSpeed);
        rigid.MovePosition(transform.position + moveVec * Time.deltaTime);
        if (isDodge) moveVec = dodgeVec;
        if (isSwap) moveVec = Vector3.zero;
        //transform.position += moveVec * (shiftDown ? speed/2 : speed) * Time.deltaTime;
        anim.SetBool("isWalk", shiftDown);
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Attack()
    {
        if (equipWeapon == null) return;
        //if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo == 0) return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        
        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
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
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.5f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void AimSetCamPosition()
    {
        if (f2Down && !isDodge)
        {
            Camera.main.transform.parent = Camera2Tr.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }
        else
            SetCamPosition();
    }
    void SetCamPosition()
    {
        Camera.main.transform.parent = CameraTr.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
    void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float camerRotationX = xRotation * mouseSensitivity;
        currentCameraRotation = Mathf.Clamp(currentCameraRotation, cameraRotationMinLimit, cameraRotationMaxLimit);
        currentCameraRotation -= camerRotationX;
        Camera.main.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, 0);
    }
    void Turn()
    {
        //transform.LookAt(transform.position + moveVec);
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 playerRotationY = new Vector3(0, yRotation, 0) * mouseSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(playerRotationY));
    }
    void Dodge()
    {
        if(spaceDown && moveVec != Vector3.zero && !isDodge && !shiftDown &&!isSwap)
        {
            dodgeVec = moveVec;
            moveSpeed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }        
    }
    void DodgeOut()
    {
        moveSpeed /= 2;
        isDodge = false;
    }
    void Reload()
    {
        if (!equipWeapon) return;
        if (equipWeapon.type == Weapon.Type.Melee) return;
        if (ammo == 0) return;

        if((rDown && !isDodge & !isSwap && isFireReady ) || (equipWeapon.curAmmo == 0 && fDown && !isReload))
        {
            isReload = true;
            anim.SetTrigger("doReload");
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        Debug.Log($"리로드{reAmmo},{ammo}");
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            Item item = other.GetComponent<Item>();
            int weaponIndex = item.value;
            hasweapons[weaponIndex] = true;
            Destroy(other.gameObject);
        }
        if(other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo) ammo = maxAmmo;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) health = maxHealth;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) coin = maxCoin;
                    break;
            }
            Destroy(other.gameObject);
            
        }
    }
}
