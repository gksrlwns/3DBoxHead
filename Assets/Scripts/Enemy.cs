using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {A, B, C};
    public Transform player;
    public float maxHp;
    public float curHp;
    
    public BoxCollider EnemyMelee;
    Type type;
    NavMeshAgent nav;
    Rigidbody rigid;
    Material mat;
    

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material;
    }
    void FixRotation()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
    private void FixedUpdate()
    {
        FixRotation();
    }
    private void Update()
    {
        nav.SetDestination(player.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.bullet_damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Debug.Log(bullet.bullet_damage);
            StartCoroutine(OnDamage(reactVec));
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
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);            
            Destroy(this.gameObject, 4f);
        }
    }
}
