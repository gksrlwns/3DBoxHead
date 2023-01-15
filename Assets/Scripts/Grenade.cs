using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionEff;
    public GameObject mesh;
    public Rigidbody rigid;
    public int grenadeDamage;
    public float explosionRadius;
    void Start()
    {
        StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        explosionEff.SetActive(true);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hit in hits)
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position, grenadeDamage);
        }
        Destroy(gameObject, 5);
    }
    
}
