using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonBullet : MonoBehaviour
{
    public bool isMelee;
    public bool isBossBullet;
    public int bullet_damage;

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
    public void BulletDamege(int damage)
    {
        bullet_damage = damage;
    }
}