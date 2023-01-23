using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonBullet : MonoBehaviourPunCallbacks
{
    public bool isMelee;
    public bool isBossBullet;
    public int bullet_damage;
    PhotonView bulletPv;
    private void Awake()
    {
        bulletPv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject, 3f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor")))
        {
            Destroy(this.gameObject);
        }
        if (isBossBullet)
            Destroy(this.gameObject);
    }
    [PunRPC]
    public void BulletDamege(int damage)
    {
        bullet_damage = damage;
    }
}