﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEff;
    public GameObject bullet;
    public Transform bulletPos;
    public GameObject bulletCase;
    public Transform bulletCasePos;

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }else if(type == Type.Range)
        {
            StartCoroutine("Shot");
        }
        
        
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEff.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.5f);
        trailEff.enabled = false;
    }

    IEnumerator Shot()
    {
        yield return null;
        GameObject bulletClone = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = bulletClone.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50f;
        yield return null;
        GameObject bulletCaseClone = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = bulletCaseClone.GetComponent<Rigidbody>();
        bulletCaseRigid.AddForce(bulletCasePos.forward * Random.Range(-3,-2) + Vector3.up * Random.Range (2,3), ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }

   
}
