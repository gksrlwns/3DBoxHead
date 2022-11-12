using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject Player;
    public float speed;
    public Vector3 offset;
    
    Vector3 cameraPosition;

    // Update is called once per frame
    void LateUpdate()
    {
        cameraPosition = Player.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, speed * Time.deltaTime);
    }
}
