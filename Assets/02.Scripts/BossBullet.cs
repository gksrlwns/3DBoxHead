using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBullet : Bullet
{
    Transform target;
    public Vector3 targetPosSet;
    public Vector3 targetPos;


    private void Update()
    {
        if (!target) return;
        Vector3 dir = target.position - transform.position;
        //float distThisFrame = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir + targetPosSet, Time.deltaTime * 2, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        //transform.Translate(dir.normalized * distThisFrame, Space.World);
        //transform.LookAt(target);

        transform.Translate(Vector3.forward * Time.deltaTime * 20);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void Seek(Transform enemyTarget)
    {
        target = enemyTarget;
    }
}
