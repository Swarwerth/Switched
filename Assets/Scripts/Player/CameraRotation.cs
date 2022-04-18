using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

   
    Vector3 playerPos;
    public Transform player;
    public Transform rotRef;
   
	void Update () 
    {
        player = GameObject.Find("player").transform;
        followPlayer();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotRef.transform.rotation,4f);
    }

    void followPlayer()
    {
        if (Mathf.Abs(RotRef.side) % 2 == 0)
        {
            playerPos = player.transform.position; playerPos.z = transform.position.z;
            transform.position = playerPos;
        }
        else
        {
            playerPos = player.transform.position; playerPos.x = transform.position.x;
            transform.position = playerPos;
        }
    }
}
