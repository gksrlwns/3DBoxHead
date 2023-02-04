using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonEnemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public Transform target;
    public GameObject enemyBulletPrefab;
    public MultiGameManager multiGameManager;
    public Transform enemyBulletPos;
    public float maxHp;
    public float curHp;
    public int enemyScore;
    public float targetRadius = 1.5f;
    public float targeAttackRange = 2f;
    public float targetSearchRange = 20f;
    public int enemyPhysicalDamage = 10;
    public int enemyBulletDamage;
    public bool bossSpawnEnemy = false;
    public PhotonView pv;

    bool isChase;
    bool isAttack;
    public bool isDead;

    protected NavMeshAgent nav;
    protected Rigidbody rigid;
    protected MeshRenderer[] meshs;
    protected Animator anim;


    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        //nav.enabled = false;
        if (enemyType != Type.D)
            Invoke("ChaseOn", 2f);
    }


    void FixedUpdate()
    {
        FixRotation();
    }
    private void Update()
    {
        if (multiGameManager && !multiGameManager.isGame) return;
        TargetSearching();
        if (!target)
        {
            anim.SetBool("isWalk", false);
            return;
        }
        if (nav.enabled)
            nav.SetDestination(target.position);
        TargetAttackRange();
    }
    private void OnDestroy()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!bossSpawnEnemy)
            multiGameManager.enemyCnt--;

    }

    void ChaseOn()
    {
        isChase = true;
        anim.SetBool("isWalk", isChase);
    }
    void FixRotation()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    protected void TargetSearching()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetSearchRange);

        float shortesDistance = Mathf.Infinity;
        GameObject nearPlayer = null;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                float distanceToenemy = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (distanceToenemy < shortesDistance)
                {
                    shortesDistance = distanceToenemy;
                    nearPlayer = colliders[i].gameObject;
                }
            }
        }
        if (nearPlayer)
        {
            target = nearPlayer.transform;
            //nav.enabled = true;
        }
        else
            target = null;
    }

    void TargetAttackRange()
    {
        if (isDead && enemyType != Type.D) return;

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targeAttackRange, LayerMask.GetMask("Player"));

        if (raycastHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        nav.enabled = false;
        yield return new WaitForSeconds(0.5f);
        switch (enemyType)
        {
            case Type.A:
                rigid.AddForce(transform.forward * 5, ForceMode.Impulse);
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                rigid.AddForce(transform.forward * 30, ForceMode.Impulse);
                yield return new WaitForSeconds(3f);
                break;
            case Type.C:
                var enemyBulletClone = Instantiate(enemyBulletPrefab, enemyBulletPos.position, enemyBulletPos.rotation);
                Rigidbody enemyBulletRigid = enemyBulletClone.GetComponent<Rigidbody>();
                Bullet enemyBullet = enemyBulletClone.GetComponent<Bullet>();
                enemyBullet.bullet_damage = enemyBulletDamage;
                enemyBulletRigid.velocity = enemyBulletPos.forward * 50f;
                yield return new WaitForSeconds(2f);
                break;
        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
        nav.enabled = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetSearchRange);
        Gizmos.DrawWireSphere(transform.position, targetRadius);
        Gizmos.DrawWireSphere(transform.position, targeAttackRange);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            PhotonWeapon weapon = other.GetComponent<PhotonWeapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Debug.Log("근접" + weapon.damage);
            //pv.RPC("PunOnDamage", RpcTarget.All, reactVec, false);
            StartCoroutine(OnDamage(reactVec));
        }

        if (other.CompareTag("Bullet"))
        {
            PhotonBullet bullet = other.GetComponent<PhotonBullet>();
            curHp -= bullet.bullet_damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Debug.Log("원거리" + bullet.bullet_damage);
           // pv.RPC("PunOnDamage", RpcTarget.All, reactVec, false);
            StartCoroutine(OnDamage(reactVec));
            Destroy(other.gameObject);
        }
    }
    public void HitByGrenade(Vector3 explosionPos, int damage)
    {
        curHp -= damage;
        Vector3 reactVec = transform.position - explosionPos;
        //pv.RPC("PunOnDamage", RpcTarget.All, reactVec, true);
        StartCoroutine(OnDamage(reactVec, true));
    }
    [PunRPC]
    public void Dead(Vector3 reactVec, bool isGrenade = false)
    {
        for (int i = 0; i < meshs.Length; i++)
        {
            meshs[i].material.color = Color.gray;
        }
        this.gameObject.layer = 14;
        isChase = false;
        isDead = true;
        nav.enabled = false;
        if (isGrenade)
        {
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.freezeRotation = false;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            rigid.AddTorque(reactVec * 5, ForceMode.Impulse);
        }
        else
        {
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 10, ForceMode.Impulse);
        }
        //넉백
        anim.SetTrigger("doDie");
        Destroy(this.gameObject, 2f);
    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade = false)
    {
        for (int i = 0; i < meshs.Length; i++)
        {
            meshs[i].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        if (curHp > 0)
        {
            for (int i = 0; i < meshs.Length; i++)
            {
                meshs[i].material.color = Color.white;
            }
        }
        else
        {
            if(PhotonNetwork.IsMasterClient&&pv.IsMine)
                pv.RPC("Dead", RpcTarget.All, reactVec, true);
        }
        //else
        //{
        //    for (int i = 0; i < meshs.Length; i++)
        //    {
        //        meshs[i].material.color = Color.gray;
        //    }
        //    this.gameObject.layer = 14;
        //    isChase = false;
        //    isDead = true;
        //    nav.enabled = false;
        //    if (isGrenade)
        //    {
        //        reactVec = reactVec.normalized;
        //        reactVec += Vector3.up;
        //        rigid.freezeRotation = false;
        //        rigid.AddForce(reactVec * 5, ForceMode.Impulse);
        //        rigid.AddTorque(reactVec * 5, ForceMode.Impulse);
        //    }
        //    else
        //    {
        //        reactVec = reactVec.normalized;
        //        reactVec += Vector3.up;
        //        rigid.AddForce(reactVec * 10, ForceMode.Impulse);
        //    }
        //    //넉백
        //    anim.SetTrigger("doDie");
        //    Destroy(this.gameObject, 2f);
        //}
    }
}

