using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public Vector3 offset;
    Player player;
    Vector3 cameraPosition;

    private void Awake()
    {
        player = target.GetComponent<Player>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = player.CameraTr.transform.position;
        //transform.position = player.playerCamera.transform.position;
        //transform.position = cameraPosition;
        //transform.localEulerAngles = player.playerCamera.transform.localEulerAngles;
        
        //transform.position = Vector3.Lerp(transform.position, cameraPosition, speed * Time.deltaTime);
    }
}
