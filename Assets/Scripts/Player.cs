using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("무기")]
    public GameObject[] weapons;
    public bool[] hasweapons;
    public GameObject bulletPos;
    public GameObject grenadePref;

    [Header("속도")]
    public float moveSpeed;
    public float mouseSensitivity;
    public float cameraSpeed;
    public float throwAngle;
    public GameObject lineRendererPrefab;

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
    public int score;
    public int health;
    public int hasGrenade;
    public int maxAmmo;
    public int maxScore;
    public int maxHealth;
    public int maxGrenade;

    [Header("UI")]
    public Text hpText;
    public Text ammoText;
    public Text scoreText;
    public Text grenadeText;
    public GameObject[] equipWeaponImages;
    public GameManager gameManager;
    

    float hAxis;
    float vAxis;

    bool shiftDown;
    bool spaceDown;
    bool rDown;
    bool s1Down;
    bool s2Down;
    bool s3Down;
    bool s4Down;
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
    float hitpointDistance;


    Vector3 moveVec;
    Vector3 hitDir;
    Vector3 hitPointVec;
    Vector3 vo;
    RaycastHit throwHit;
    Animator anim;
    Rigidbody rigid;
    Weapon equipWeapon;
    MeshRenderer[] meshs;
    Camera playerCamera;
    LineRenderer lr;
    GameObject lrObj;

    private void Awake()
    {
        lrObj = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        lr = lrObj.GetComponent<LineRenderer>();
        playerCamera = Camera.main;
    }
    void Start()
    {
        SetCamera();
        PlayerState();
        //Debug.Log(playerHandRt.localEulerAngles);
        //secondCamera.enabled = false;
    }

    void Update()
    {
        if (!gameManager.isGame) return;
        if (isDead) return;
        PlayerState();
        GetInput();
        Move();
        CameraRotation();
        AimSetCamPosition();
        Turn();
        //Dodge();
        Reload();
        Swap();
        Attack();
        //AimTarget();
        

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

    void SetCamera()
    {
        playerCamera.transform.parent = curCamTr;
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
    }
    void PlayerState()
    {
        hpText.text = $"{health} / {maxHealth}";
        if (!equipWeapon)
            ammoText.text = $" - / {hasAmmo}";
        else if (equipWeapon.type == Weapon.Type.Melee)
            ammoText.text = $" - / {hasAmmo}";
        else if (equipWeapon.type == Weapon.Type.Range)
            ammoText.text = $"{equipWeapon.curAmmo} / {hasAmmo}";
        scoreText.text = $"{score}";
        grenadeText.text = $"{hasGrenade}";
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
        s4Down = Input.GetButtonDown("Swap4");
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

    void Attack()
    {
        if (equipWeapon == null) return;
        if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo == 0) return;
        if (equipWeapon.type == Weapon.Type.Grenade && hasGrenade == 0) return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        
        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            switch (equipWeapon.type)
            {
                case Weapon.Type.Melee:
                    equipWeapon.Use();
                    anim.SetTrigger("doSwing");
                    break;
                case Weapon.Type.Range:
                    equipWeapon.Use();
                    anim.SetTrigger("doShot");
                    break;
                case Weapon.Type.Grenade:
                    Throw();
                    anim.SetTrigger("doThrow");
                    break;
            }
            //anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void Throw()
    {
        //Vector3 vo = CalculateVelcoity(throwHit.point, equipWeapon.transform.position, 1.5f);
        Rigidbody voRigid = Instantiate(grenadePref, equipWeapon.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        hasGrenade--;
        Debug.Log($"던지는 Pos{equipWeapon.transform.position}");
        voRigid.velocity = vo;
    }
    void Swap()
    {
        if (s1Down && (!hasweapons[0] || equipWeaponIndex == 0)) return;
        if (s2Down && (!hasweapons[1] || equipWeaponIndex == 1)) return;
        if (s3Down && (!hasweapons[2] || equipWeaponIndex == 2)) return;
        if (s4Down && (!hasweapons[3] || equipWeaponIndex == 3 || hasGrenade == 0)) return;

        int weaponIndex = -1;
        //if (equipWeapon.type == Weapon.Type.Grenade && hasGrenade == 0)
        //{
        //    weaponIndex = -1;
        //    equipWeapon = null;
        //    equipWeapon.gameObject.SetActive(false);
        //    for (int i = 0; i < equipWeaponImages.Length; i++)
        //    {
        //        equipWeaponImages[i].SetActive(false);
        //    }
        //    anim.SetTrigger("doSwap");
        //    isSwap = true;
        //    Invoke("SwapOut", 0.5f);
        //}
        if (s1Down && hasweapons[0]) weaponIndex = 0;
        if (s2Down && hasweapons[1]) weaponIndex = 1;
        if (s3Down && hasweapons[2]) weaponIndex = 2;
        if (s4Down && hasweapons[3] && hasGrenade != 0) weaponIndex = 3;
        if((s1Down || s2Down || s3Down || s4Down) && !isDodge && !isSwap)
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
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            equipWeaponImages[equipWeaponIndex].SetActive(true);

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
        if (equipWeapon.type == Weapon.Type.Range) anim.SetBool("isAim", f2Down);
        else if (equipWeapon.type == Weapon.Type.Grenade) anim.SetBool("isThrowAim", f2Down);
        if (f2Down && !isDodge)
        {
            curCamTr.position = Vector3.Lerp(curCamTr.position, Camera2Tr.position, cameraSpeed * Time.deltaTime);
            
            if(equipWeapon.type == Weapon.Type.Range)
            {
                AimShot();
                crossHair.SetActive(true);
            }
            else if(equipWeapon.type == Weapon.Type.Grenade && isFireReady)
            {
                AimThrow();
                lrObj.SetActive(true);
            }
            else
            {
                crossHair.SetActive(false);
                lrObj.SetActive(false);
            }
            
        }
        else
        {
            if (curCamTr.position == CameraTr.position) return;
            curCamTr.position = Vector3.Lerp(curCamTr.position, CameraTr.position, cameraSpeed * Time.deltaTime);
            crossHair.SetActive(false);
            lrObj.SetActive(false);
        }
    }
    void AimShot()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red, 0.5f);
        RaycastHit hit;
        RaycastHit bulHit;
        int layerMask = 1 << LayerMask.NameToLayer("MiddleWall");
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
        {
            //hit 과 bulhit이 같은 지점이 아니라면 blockedAim true
            Debug.DrawLine(playerCamera.transform.position, hit.point);
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
                blockedAim.transform.position = bulHit.point + dir * 0.1f;
                //Debug.Log(bulHit.point);
            }
            else
                blockedAim.SetActive(false);
            //blockedAim.SetActive(false);
            //Debug.DrawRay(bulletPos.transform.position, bulletPos.transform.forward * 100f, Color.green, 0.5f);
        }
    }
    void AimThrow()
    {
        //var target = AngleToDirection(throwAngle);       
        int layerMask = 1 << LayerMask.NameToLayer("Floor");
        //Debug.DrawLine(playerCamera.transform.position, playerCamera.transform.forward * 50f, Color.blue);
        
        //if (Physics.Raycast(playerCamera.transform.position, target.normalized, out throwHit, 50f, layerMask))
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out throwHit, layerMask))
        {
            //Debug.Log(throwHit.point);
            //Debug.DrawLine(playerCamera.transform.position, throwHit.point, Color.red);
            if (throwHit.point != Vector3.zero)
            {
                hitPointVec = throwHit.point;
                hitpointDistance = Vector3.Distance(playerCamera.transform.position, throwHit.point);
            }
        }
        else
        {
            hitPointVec = playerCamera.transform.position + playerCamera.transform.forward * hitpointDistance;
            Debug.DrawLine(playerCamera.transform.position, hitPointVec, Color.black);
        }
        //Debug.Log($"{hitpointDistance}");
        //Debug.Log($"{throwHit.point}\n{hitPointVec}");
        vo = CalculateVelcoity(hitPointVec, equipWeapon.transform.position, 2f);
        DrawPath(vo);
        //else도 만들어서 Raycast 없는 경우 사거리에 맞게 + 라인렌더러 만들어서 포물선 보여주기.
    }
    void DrawPath(Vector3 velocity)
    {
        Vector3 previousDrawPoint = equipWeapon.transform.position;
        
        //int resolution = 30;
        //lineRenderer.positionCount = resolution;
        for (int i = 1; i <= lr.positionCount; i++)
        {
            //float simulationTime = i / (float)resolution * launchData.timeToTarget;
            float simulationTime = i / (float)lr.positionCount * 1f;

            Vector3 displacement = velocity * simulationTime + Vector3.up * Physics.gravity.y * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = equipWeapon.transform.position + displacement;
            //DebugExtension.DebugPoint(drawPoint, 1, 1000f);//유니티 에셋스토어 Debug Extension
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            lr.SetPosition(i - 1, drawPoint);
            previousDrawPoint = drawPoint;
        }
    }
    Vector3 AngleToDirection(float angle)
    {
        Vector3 dir = playerCamera.transform.forward;
        var quat = Quaternion.Euler(angle, 0, 0);
        Vector3 newDir = quat * dir;
        return newDir;
    }
    Vector3 CalculateVelcoity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; //x와z의 평면이면 기본적으로 거리와 같은 벡터
        distanceXZ.y = 0f;//y는 0으로 설정

        //create a float the represent our distance
        float Sy = distance.y;//세로 높이의 거리를 지정
        float Sxz = distanceXZ.magnitude;

        //속도 계산
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        //계산으로 인해 두축의 초기 속도 가지고 새로운 벡터를 만들수 있음
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
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
        if (f2Down && equipWeapon.type == Weapon.Type.Range)
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
        if (equipWeapon.type != Weapon.Type.Range) return;
        if (hasAmmo == 0) return;

        if((rDown && !isDodge & !isSwap && isFireReady ) || (equipWeapon.curAmmo == 0 && fDown && !isReload))
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
                    hasAmmo += item.value;
                    if (hasAmmo > maxAmmo) hasAmmo = maxAmmo;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) health = maxHealth;
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
