using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonPlayer : MonoBehaviourPunCallbacks
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
    public int hasAmmo;
    public int coin;
    public int health;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;

    [Header("UI")]
    public Text hpText;
    public Text ammoText;
    public Text coinText;
    public GameObject[] equipWeaponImages;
    public GameManager gameManager;
    public GameObject playerCanvas;


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


    public int equipWeaponIndex = -1;
    float fireDelay;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Transform playerHandRt;
    Animator anim;
    Rigidbody rigid;
    PhotonWeapon equipWeapon;
    MeshRenderer[] meshs;
    Camera Camera;
    public PhotonView pv;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        Camera = Camera.main;
        //photonView = GetComponent<PhotonView>();
    }
    void Start()
    {
        playerHandRt = playerWeaponHand.transform;
        //SetCamera();
        //Debug.Log(playerHandRt.localEulerAngles);
        //secondCamera.enabled = false;
    }

    void Update()
    {
        //if (!gameManager.isGame) return;
        if (isDead) return;
        if(pv.IsMine)
        {
            GetInput();
            Move();
            CameraRotation();
            AimSetCamPosition();
            Turn();
            Reload();
            Swap();
            Attack();
            AimTarget();
            PlayerState();
        }



        if (health == 0)
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

    public void SetPlayer()
    {
        Camera.main.transform.parent = curCamTr;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
        playerCanvas.SetActive(true);
    }
    void PlayerState()
    {
        hpText.text = $"{health} / {maxHealth}";
        if (equipWeapon == null)
            ammoText.text = $" - / {hasAmmo}";
        else if (equipWeapon.type == PhotonWeapon.Type.melee)
            ammoText.text = $" - / {hasAmmo}";
        else if (equipWeapon.type == PhotonWeapon.Type.range)
            ammoText.text = $"{equipWeapon.curAmmo} / {hasAmmo}";
        coinText.text = $"{coin}";
    }
    void AimTarget()
    {
        if (isDodge) return;
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red, 0.5f);
        RaycastHit hit;
        RaycastHit bulHit;
        int layerMask = 1 << LayerMask.NameToLayer("MiddleWall");
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit))
        {
            //hit 과 bulhit이 같은 지점이 아니라면 blockedAim true
            Debug.DrawLine(Camera.transform.position, hit.point);
            bulletPos.transform.LookAt(hit.point);
            //layer로 bullet과 충돌 x, bulhit을 플레이어 방향으로 조금 이동
            if (Physics.Linecast(bulletPos.transform.position, hit.point, out bulHit, layerMask))
            {
                Debug.DrawLine(bulletPos.transform.position, bulHit.point);
                if (hit.point != bulHit.point)
                    blockedAim.SetActive(true);
                else
                    blockedAim.SetActive(false);
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

    #region 총관련
    void Attack()
    {
        if (equipWeapon == null) return;
        if (equipWeapon.type == PhotonWeapon.Type.range && equipWeapon.curAmmo == 0) return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use(equipWeaponIndex);
            anim.SetTrigger(equipWeapon.type == PhotonWeapon.Type.melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    [PunRPC]
    public void PunSwap(int weaponIndex)
    {
        if (equipWeapon != null)
        {
            equipWeapon.gameObject.SetActive(false);
            for (int i = 0; i < equipWeaponImages.Length; i++)
            {
                equipWeaponImages[i].SetActive(false);
            }
        }
        equipWeaponIndex = weaponIndex;
        equipWeapon = weapons[weaponIndex].GetComponent<PhotonWeapon>();
        equipWeapon.gameObject.SetActive(true);
        equipWeaponImages[weaponIndex].SetActive(true);
        //if ((s1Down || s2Down || s3Down) && !isDodge && !isSwap)
        //{
        //    if (equipWeapon != null)
        //    {
        //        equipWeapon.gameObject.SetActive(false);
        //        for (int i = 0; i < equipWeaponImages.Length; i++)
        //        {
        //            equipWeaponImages[i].SetActive(false);
        //        }
        //    }
        //    equipWeaponIndex = weaponIndex;
        //    equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
        //    equipWeapon.gameObject.SetActive(true);
        //    equipWeaponImages[equipWeaponIndex].SetActive(true);
        //}
    }
    public void Swap()
    {
        if (s1Down && (!hasweapons[0] || equipWeaponIndex == 0)) return;
        if (s2Down && (!hasweapons[1] || equipWeaponIndex == 1)) return;
        if (s3Down && (!hasweapons[2] || equipWeaponIndex == 2)) return;
        int weaponIndex = -1;
        if (s1Down && hasweapons[0]) weaponIndex = 0;
        if (s2Down && hasweapons[1]) weaponIndex = 1;
        if (s3Down && hasweapons[2]) weaponIndex = 2;
        if ((s1Down || s2Down || s3Down) && !isDodge && !isSwap)
        {
            pv.RPC("PunSwap", RpcTarget.AllBuffered, weaponIndex);
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
        if (!equipWeapon || equipWeapon.type == PhotonWeapon.Type.melee) return;
        anim.SetBool("isAim", f2Down);
        if (f2Down && !isDodge)
        {
            curCamTr.position = Vector3.Lerp(curCamTr.position, Camera2Tr.position, cameraSpeed * Time.deltaTime);
            crossHair.SetActive(true);
            AimTarget();
        }
        else
        {
            if (curCamTr.position == CameraTr.position) return;
            curCamTr.position = Vector3.Lerp(curCamTr.position, CameraTr.position, cameraSpeed * Time.deltaTime);
            crossHair.SetActive(false);
        }
    }
    #endregion
    public void CameraRotation()
    {
        if (isDodge) return;
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float camerRotationX = xRotation * mouseSensitivity;
        currentCameraRotation = Mathf.Clamp(currentCameraRotation, cameraRotationMinLimit, cameraRotationMaxLimit);
        currentCameraRotation -= camerRotationX;
        Camera.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, 0);
        
        pv.RPC("PunCameraRotation", RpcTarget.All, currentCameraRotation);
        

    }
    [PunRPC]
    public void PunCameraRotation(float cameraRt)
    {
        playerHead.transform.localEulerAngles = new Vector3(cameraRt, 0, 0);
        bulletPos.transform.localEulerAngles = new Vector3(cameraRt, 0, 0);
        if (f2Down)
        {
            playerWeaponHand.transform.localEulerAngles = new Vector3(playerWeaponHandRt.x, playerWeaponHandRt.y, playerWeaponHandRt.z - cameraRt);
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
        if (equipWeapon.type == PhotonWeapon.Type.melee) return;
        if (hasAmmo == 0) return;

        if ((rDown && !isDodge & !isSwap && isFireReady) || (equipWeapon.curAmmo == 0 && fDown && !isReload))
        {
            isReload = true;
            anim.SetTrigger("doReload");
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = hasAmmo < equipWeapon.maxAmmo ? hasAmmo : equipWeapon.maxAmmo;
        Debug.Log($"리로드{reAmmo},{hasAmmo}");
        hasAmmo += equipWeapon.curAmmo;
        equipWeapon.curAmmo = reAmmo;
        hasAmmo -= reAmmo;
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
        if (collision.gameObject.CompareTag("Enemy"))
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
        if (other.CompareTag("Weapon"))
        {
            Item item = other.GetComponent<Item>();
            int weaponIndex = item.value;
            hasweapons[weaponIndex] = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    hasAmmo += item.value;
                    if (hasAmmo > maxAmmo) hasAmmo = maxAmmo;
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
                if (health < 0)
                    health = 0;
            }
            if (!bullet.isMelee)
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
