using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject BossBulletPrefab;
    public GameObject[] enemyPrefabs;
    public int BossBulletDamage;
    public Transform BossBulletPosA;
    public Transform BossBulletPosB;
    public Transform spawnSpot;
    public BoxCollider bossMeleeArea;
    Vector3 tauntVec;
    Vector3 playerMove;
    BoxCollider boxCollider;
    bool isLockOn = true;


    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        
        nav.isStopped = true;
        StartCoroutine(BossPattern());
    }
    private void Update()
    {
        if (isDead)
            StopAllCoroutines();
        if(isLockOn)
            PlayerSearching();
        else
            nav.SetDestination(tauntVec);
    }


    void PlayerSearching()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        playerMove = new Vector3(hAxis, 0, vAxis) * 5f;
        transform.LookAt(target.position + playerMove);
    }

    IEnumerator BossPattern()
    {
        yield return new WaitForSeconds(0.1f);
        
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                StartCoroutine(Shot());
                break;
            case 1:
                StartCoroutine(SpawnEnemy());
                break;
            case 2:
                RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
                if (raycastHits.Length > 0 && isLockOn)
                {
                    StartCoroutine(Taunt());
                }
                else
                    StartCoroutine(BossPattern());
                //StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator Shot()
    {
        anim.SetTrigger("doShot");
        var bulletCloneA = Instantiate(BossBulletPrefab, BossBulletPosA.position, BossBulletPosA.rotation);
        BossBullet bulletA = bulletCloneA.GetComponent<BossBullet>();
        bulletA.Seek(target);
        bulletA.BulletDamege(BossBulletDamage);

        yield return new WaitForSeconds(0.5f);

        var bulletCloneB = Instantiate(BossBulletPrefab, BossBulletPosB.position, BossBulletPosB.rotation);
        BossBullet bulletB = bulletCloneB.GetComponent<BossBullet>();
        bulletB.Seek(target);
        bulletB.BulletDamege(BossBulletDamage);

        yield return new WaitForSeconds(3f);
        StartCoroutine(BossPattern());
    }
    IEnumerator SpawnEnemy()
    {
        anim.SetTrigger("doBigShot");
        yield return new WaitForSeconds(1f);
        int random = Random.Range(0, enemyPrefabs.Length);
        var enemyClone = Instantiate(enemyPrefabs[random], spawnSpot.position, spawnSpot.rotation);
        Enemy enemy = enemyClone.GetComponent<Enemy>();
        enemy.target = target;
        yield return new WaitForSeconds(3f);
        StartCoroutine(BossPattern());
    }
    IEnumerator Taunt()
    {
        isLockOn = false;
        nav.isStopped = false;
        anim.SetTrigger("doTaunt");
        tauntVec = target.position;
        boxCollider.enabled = false;
        yield return new WaitForSeconds(1.5f);
        bossMeleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        bossMeleeArea.enabled = false;
        yield return new WaitForSeconds(2f);
        boxCollider.enabled = true;
        isLockOn = true;
        nav.isStopped = true;
        StartCoroutine(BossPattern());
    }
}
