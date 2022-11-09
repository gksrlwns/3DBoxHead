using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject CameraTr;
    float hAxis;
    float vAxis;
    public float speed;
    Vector3 moveVec;
    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        //SetCamPosition();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
        transform.LookAt(transform.position + moveVec);
        
    }

    void SetCamPosition()
    {
        Camera.main.transform.parent = CameraTr.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
}
