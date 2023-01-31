using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonGrenade : MonoBehaviourPunCallbacks
{
    public GameObject explosionEff;
    public GameObject mesh;
    public Rigidbody rigid;
    public int grenadeDamage;
    public float explosionRadius;
    public PhotonView pv;
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
        foreach (RaycastHit hit in hits)
        {
            hit.transform.GetComponent<PhotonEnemy>().HitByGrenade(transform.position, grenadeDamage);
        }
        Destroy(gameObject, 5);
    }
}
