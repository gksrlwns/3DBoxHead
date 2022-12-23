﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("무기")]
    public GameObject[] weapons;
    public bool[] hasweapons;
    public GameObject bulletPos;
    [Header("속도")]
    public float moveSpeed;
    public float mouseSensitivity;
    public float cameraSpeed;

    [Header("카메라")]
    public float cameraRotationMaxLimit;
    public float cameraRotationMinLimit;
    public float currentCameraRotation = 0;
    public Transform CameraTr;
    public Transform Camera2Tr;
    public Transform curCamTr;
    public Vector3 playerWeaponHandRt;
    public GameObject playerHead;
    public GameObject playerWeaponHand;
    public GameObject crossHair;
    public GameObject blockedAim;
    
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
    bool isDamage;
    bool isAim;
    bool isMove = true;
    public bool isDead;
    

    int equipWeaponIndex = -1;
    float fireDelay;
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Transform playerHandRt;
    Animator anim;
    Rigidbody rigid;
    Weapon equipWeapon;
    MeshRenderer[] meshs;
    Camera playerCamera;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        playerCamera = Camera.main;
    }
    void Start()
    {
        playerHandRt = playerWeaponHand.transform;
        //Debug.Log(playerHandRt.localEulerAngles);
        playerCamera.transform.parent = curCamTr;
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        //secondCamera.enabled = false;
    }

    void Update()
    {
        if (isDead) return;
        GetInput();
        Move();
        CameraRotation();
        AimSetCamPosition();
        Turn();
        //Dodge();
        Reload();
        Swap();
        Attack();
        AimTarget();
        if(health <= 0)
        {
            Dead();
        }
    }

    void FixRotation()
    {
        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
    }
    private void FixedUpdate()
    {
        FixRotation();
    }

    void AimTarget()
    {
        
        if (isDodge) return;
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red, 0.5f);
        RaycastHit hit;
        RaycastHit bulHit;
        int layerMask = 1 << LayerMask.NameToLayer("MiddleWall");
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
        {
            //hit 과 bulhit이 같은 지점이 아니라면 blockedAim true
            Debug.DrawLine(playerCamera.transform.position, hit.point);
            bulletPos.transform.LookAt(hit.point);
            //layer로 bullet과 충돌 x, bulhit을 플레이어 방향으로 조금 이동
            if(Physics.Linecast(bulletPos.transform.position, hit.point, out bulHit, layerMask))
            {
                Debug.DrawLine(bulletPos.transform.position, bulHit.point);
                Vector3 dir = transform.position - bulHit.point;
                dir.Normalize();
                blockedAim.transform.forward = dir;
                blockedAim.transform.position = bulHit.point + dir * 0.5f;
                //Debug.Log(bulHit.point);
            }
            //Debug.DrawRay(bulletPos.transform.position, bulletPos.transform.forward * 100f, Color.green, 0.5f);
        }
        
        
    }

    #region 플레이어 동작

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        shiftDown = Input.GetButton("Walk");
        //spaceDown = Input.GetButtonDown("Dodge");
        rDown = Input.GetButtonDown("Reload");
        s1Down = Input.GetButtonDown("Swap1");
        s2Down = Input.GetButtonDown("Swap2");
        s3Down = Input.GetButtonDown("Swap3");
        fDown = Input.GetButton("Fire1");
        f2Down = Input.GetButton("Fire2");
    }

    void Move()
    {
        //dodgeVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (!isMove) return;
        Vector3 moveX = transform.right * hAxis;
        Vector3 moveZ = transform.forward * vAxis;
        moveVec = (moveX + moveZ).normalized * (shiftDown ? moveSpeed / 2 : moveSpeed);
        rigid.MovePosition(transform.position + moveVec * Time.deltaTime);
        //if (isDodge) moveVec = dodgeVec;
        //transform.LookAt(transform.position + moveVec);
        //if (isSwap) moveVec = Vector3.zero;
        //transform.position += moveVec * (shiftDown ? moveSpeed/2 : moveSpeed) * Time.deltaTime;
        anim.SetBool("isWalk", shiftDown);
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }
    //void Dodge()
    //{
    //    if (spaceDown && moveVec != Vector3.zero && !isDodge && !shiftDown && !isSwap)
    //    {
    //        isMove = false;
    //        dodgeVec = new Vector3(0, moveVec.y, 0);
    //        mesh.transform.rotation = Quaternion.Lerp(mesh.transform.rotation, Quaternion.LookRotation(moveVec), 10f);
    //        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
    //        //Vector3 playerRotationY = new Vector3(0, yRotation, 0) * mouseSensitivity;
    //        //rigid.MoveRotation(rigid.rotation * Quaternion.Euler(playerRotationY));
    //        //dodgeVec = moveVec;
    //        //mesh.transform.localRotation = Quaternion.Euler(0,moveVec.y,0);
    //        rigid.AddForce(moveVec * 2f, ForceMode.Impulse);
    //        //moveSpeed *= 2;
    //        anim.SetTrigger("doDodge");
    //        isDodge = true;
    //        //playerCamera.enabled = false;
    //        //secondCamera.enabled = true;
    //        Invoke("DodgeOut", 0.5f);
    //    }
    //}
    //void DodgeOut()
    //{
    //    //moveSpeed /= 2;
    //    rigid.velocity = Vector3.zero;
    //    //playerCamera.enabled = true;
    //    //secondCamera.enabled = false;
    //    isMove = true;
    //    isDodge = false;
    //}

    void Attack()
    {
        if (equipWeapon == null) return;
        if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo == 0) return;
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
        if((s1Down || s2Down || s3Down) && !isDodge && !isSwap)
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
        if (!equipWeapon || equipWeapon.type == Weapon.Type.Melee) return;
        anim.SetBool("isAim", f2Down);
        if (f2Down && !isDodge)
        {
            curCamTr.position = Vector3.Lerp(curCamTr.position, Camera2Tr.position, cameraSpeed * Time.deltaTime);
            crossHair.SetActive(true);
            blockedAim.SetActive(true);
            AimTarget();
        }
        else
        {
            if (curCamTr.position == CameraTr.position) return;
            curCamTr.position = Vector3.Lerp(curCamTr.position, CameraTr.position, cameraSpeed * Time.deltaTime);
            crossHair.SetActive(false);
            blockedAim.SetActive(false);
        }
    }
    void CameraRotation()
    {
        if (isDodge) return;
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float camerRotationX = xRotation * mouseSensitivity;
        currentCameraRotation = Mathf.Clamp(currentCameraRotation, cameraRotationMinLimit, cameraRotationMaxLimit);
        currentCameraRotation -= camerRotationX;
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, 0);
        playerHead.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, 0);
        bulletPos.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, 0);
        if (f2Down)
        {
            playerWeaponHand.transform.localEulerAngles = new Vector3(playerWeaponHandRt.x, playerWeaponHandRt.y, playerWeaponHandRt.z - currentCameraRotation);
                //new Vector3(playerHandRt.localEulerAngles.x, playerHandRt.localEulerAngles.y, playerHandRt.localEulerAngles.z - currentCameraRotation);
        }

    }
    void Turn()
    {
        //transform.LookAt(transform.position + moveVec);
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 playerRotationY = new Vector3(0, yRotation, 0) * mouseSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(playerRotationY));
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
    void Dead()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        this.gameObject.layer = 18;
        this.gameObject.tag = "PlayerDead";
        for (int i = 0; i < meshs.Length; i++)
            meshs[i].material.color = Color.gray;
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if (!isDamage)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                health -= enemy.enemyPhysicalDamage;
                StartCoroutine(OnDamage());
            }
        }
        
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
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (!isDamage)
            {
                bool isBossAttack = other.name == "BossMeleeArea";
                StartCoroutine(OnDamage(isBossAttack));
                Debug.Log($"{bullet.bullet_damage} 데미지");
                health -= bullet.bullet_damage;
            }
            if(!bullet.isMelee)
                Destroy(other.gameObject);
        }
    }
    IEnumerator OnDamage(bool isBossAttack = false)
    {
        isDamage = true;
        if (isBossAttack)
            rigid.AddForce(transform.forward * -20f, ForceMode.Impulse);
        for (int i = 0; i < meshs.Length; i++)
            meshs[i].material.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(1f);
        isDamage = false;
        for (int i = 0; i < meshs.Length; i++)
            meshs[i].material.color = new Color(1, 1, 1, 1);
        if (isBossAttack)
            rigid.velocity = Vector3.zero;
    }
}
