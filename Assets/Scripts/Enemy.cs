using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {A, B, C};
    public Type enemyType;
    public Transform player;
    public GameObject enemyBulletPrefab;
    public Transform enemyBulletPos;
    public float maxHp;
    public float curHp;
    public float targetRadius = 1.5f;
    public float targetRange = 3f;
    public int enemyPhysicalDamage = 10;
    public int enemyBulletDamage;

    bool isChase;
    bool isAttack;

    

    Type type;
    NavMeshAgent nav;
    Rigidbody rigid;
    Material mat;
    Animator anim;
    

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        Invoke("ChaseOn", 2f);
    }
    
    private void FixedUpdate()
    {
        FixRotation();
    }
    private void Update()
    {
        if (!nav.enabled) return;
        nav.SetDestination(player.position);
        TargetAttackRange();
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

    void TargetAttackRange()
    {
        
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(raycastHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", isAttack);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                yield return new WaitForSeconds(3f);
                break;
            case Type.C:
                var enemyBulletClone = Instantiate(enemyBulletPrefab, enemyBulletPos.position,enemyBulletPos.rotation);
                Rigidbody enemyBulletRigid = enemyBulletClone.GetComponent<Rigidbody>();
                Bullet enemyBullet = enemyBulletClone.GetComponent<Bullet>();
                enemyBullet.bullet_damage = enemyBulletDamage;
                enemyBulletRigid.velocity = enemyBulletPos.forward * 50f;
                yield return new WaitForSeconds(2f);
                break;
        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", isAttack);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRange);
        Gizmos.DrawWireSphere(transform.position, targetRadius);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Melee"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Debug.Log("근접"+weapon.damage);
            StartCoroutine(OnDamage(reactVec));
        }

        if(other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.bullet_damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Debug.Log("원거리" + bullet.bullet_damage);
            StartCoroutine(OnDamage(reactVec));
            Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (curHp > 0)
            mat.color = Color.white;
        else
        {
            mat.color = Color.gray;
            this.gameObject.layer = 14;
            isChase = false;
            nav.enabled = false;
            //넉백
            reactVec = reactVec.normalized;
            reactVec += Vector3.up * 3;
            rigid.AddForce(reactVec * 10, ForceMode.Impulse);
            anim.SetTrigger("doDie");
            Destroy(this.gameObject, 2f);
        }
    }
}
