using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject BossBulletPrefab;
    public int BossBulletDamage;
    public Transform BossBulletPosA;
    public Transform BossBulletPosB;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(BossPattern());
    }
    private void Update()
    {
        PlayerSearching();
    }

    void PlayerSearching()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        Vector3 playerMove = new Vector3(hAxis, 0, vAxis) * 5f;
        transform.LookAt(target.position + playerMove);
    }

    IEnumerator BossPattern()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Shot());
        //int random = Random.Range(0, 3);
        //switch(random)
        //{
        //    case 0:
        //        StartCoroutine(Shot());
        //        break;
        //    case 1:
        //        StartCoroutine(BigShot());
        //        break;
        //    case 2:
        //        StartCoroutine(Taunt());
        //        break;
        //}
    }

    IEnumerator Shot()
    {
        anim.SetTrigger("doShot");
        var bulletCloneA = Instantiate(BossBulletPrefab, BossBulletPosA.position, BossBulletPosA.rotation);
        var bulletCloneB = Instantiate(BossBulletPrefab, BossBulletPosB.position, BossBulletPosB.rotation);
        BossBullet bulletA = bulletCloneA.GetComponent<BossBullet>();
        BossBullet bulletB = bulletCloneB.GetComponent<BossBullet>();
        bulletA.BulletDamege(BossBulletDamage);
        bulletB.BulletDamege(BossBulletDamage);
        bulletA.target = target;
        bulletB.target = target;
        yield return new WaitForSeconds(3f);
        StartCoroutine(BossPattern());
    }
    IEnumerator BigShot()
    {
        anim.SetTrigger("doBigShot");
        yield return new WaitForSeconds(3f);
        StartCoroutine(BossPattern());
    }
    IEnumerator Taunt()
    {
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(3f);
        StartCoroutine(BossPattern());
    }
}
