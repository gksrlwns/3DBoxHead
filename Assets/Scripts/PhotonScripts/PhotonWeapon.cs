using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonWeapon : MonoBehaviourPunCallbacks
{
    public enum Type { melee, range };
    public Type type;
    public int damage;
    public int curAmmo;
    public int maxAmmo;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEff;
    public GameObject bulletPrefab;
    public Transform bulletPos;
    public GameObject bulletCase;
    public Transform bulletCasePos;
    public PhotonView weaponPv;

    GameObject bulletClone;

    private void Awake()
    {
        weaponPv = GetComponent<PhotonView>();
    }

    public void Use(int weaponIndex)
    {
        if (type == Type.melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.range && curAmmo != 0)
        {
            curAmmo--;
            weaponPv.RPC("ShotRPC", RpcTarget.AllBuffered, weaponIndex);
        }
    }
    [PunRPC]
    public void ShotRPC(int weaponIndex)
    {
        StartCoroutine("Shot", weaponIndex);
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        trailEff.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.5f);
        trailEff.enabled = false;
    }

    IEnumerator Shot(int weaponIndex)
    {
        yield return null;
        if(weaponIndex == 1)
        {
            bulletClone = PhotonNetwork.Instantiate("PhotonBullet HandGun", bulletPos.position, bulletPos.rotation);
        }
        else if( weaponIndex == 2)
        {
            bulletClone = PhotonNetwork.Instantiate("PhotonBulletSubMachineGun", bulletPos.position, bulletPos.rotation);
        }
        bulletClone.GetComponent<PhotonView>().RPC("BulletDamege", RpcTarget.All, damage);
        Rigidbody bulletRigid = bulletClone.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50f;
        Destroy(bulletClone, 5f);

        yield return null;
        GameObject bulletCaseClone = PhotonNetwork.Instantiate("PhotonBulletCase", bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = bulletCaseClone.GetComponent<Rigidbody>();
        bulletCaseRigid.AddForce(bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3), ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }
}
//    [PunRPC]
//    public void FireBullet()
//    {
//        GameObject bulletClone = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
//        Rigidbody bulletRigid = bulletClone.GetComponent<Rigidbody>();
//        Bullet bullet = bulletClone.GetComponent<Bullet>();
//        bullet.BulletDamege(damage);
//        bulletRigid.velocity = bulletPos.forward * 50f;
//        Destroy(bulletClone, 5f);
//    }
//    [PunRPC]
//    public void FireBulletCase()
//    {
//        GameObject bulletCaseClone = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
//        Rigidbody bulletCaseRigid = bulletCaseClone.GetComponent<Rigidbody>();
//        bulletCaseRigid.AddForce(bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3), ForceMode.Impulse);
//        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
//    }
