using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreato : MonoBehaviour {

    float dis;
    BoxCollider bc;
    Transform disRef;

    Vector3 init;
    void Start() {

        bc = GetComponent<BoxCollider>();

        disRef = GameObject.Find("disRef").transform;

        init = bc.center;
    }
    private void Update()
    {

        if (PlayerMovement.isMoving)
        {
            if (Mathf.Abs(RotRef.side) % 2 == 0)
                dis = Mathf.Abs(transform.position.z - disRef.position.z);
            else if (Mathf.Abs(RotRef.side) % 2 != 0)
                dis = Mathf.Abs(transform.position.x - disRef.position.x);


            if (Mathf.Abs(RotRef.side) % 2 == 0)
            {
                bc.center = init;
                if (RotRef.side == 0)
                    bc.center = new Vector3(bc.center.x, bc.center.y, -dis);
                else
                    bc.center = new Vector3(bc.center.x, bc.center.y, dis);
            }
            else
            {
                bc.center = init;
                if (RotRef.side == 1 || RotRef.side == -3)
                    bc.center = new Vector3(dis, bc.center.y, bc.center.z);
                else if (RotRef.side == -1 || RotRef.side == 3)
                    bc.center = new Vector3(-dis, bc.center.y, bc.center.z);
            }
        }

        if (RotRef.isRot == true)
        {
            bc.center = init;

            Invoke("resetColl",0.1f);            
        }
    }

    public void resetColl()
    {
       RotRef.isRot = false;
    }
    
}
