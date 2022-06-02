using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks 
{

    Rigidbody rb;
    public float moveSpeed, jumpSpeed;

    [SerializeField]GameObject rayObject;
    [SerializeField] Transform disObj;
    [HideInInspector]
    public static bool isMoving,isJumping;
    public SpriteRenderer playerRender;
    Animator AM;

    private void Start()
    {       
        isMoving = false;
        AM = playerRender.gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();       
    }

    GameObject currCube = null;

    float horz,vert;
    [HideInInspector]
    public bool grounded,grounded2,walled,wallMove;
    float jump;  
    
    private void Update()
    {       
        if (!photonView.IsMine || Timer.paused) return;
        isMoving = (horz > 0 || horz < 0);
        castRays();        
    }

    int dir = 1;
    private void FixedUpdate()
    {
        keyInputs();        
    }

    void keyInputs()
    {
        horz = Input.GetAxisRaw("Horizontal");
        vert = Input.GetAxisRaw("Vertical");

        grounded2 = (Physics.Raycast(transform.position, Vector3.down, out hit, 0.7f, LayerMask.GetMask("ground")) ||
            Physics.Raycast(transform.position + new Vector3(0.7f,0,0), Vector3.down, out hit, 0.7f, LayerMask.GetMask("ground")) ||
            Physics.Raycast(transform.position- new Vector3(0.7f, 0, 0), Vector3.down, out hit, 0.7f, LayerMask.GetMask("ground")));

        walled = (Physics.Raycast(transform.position, transform.forward, out hit, 5f, LayerMask.GetMask("ladder")));


        if (horz != 0 && grounded2 && !wallMove)
        {
            if (RotRef.side % 2 == 0)
            {
                if (RotRef.side == 0) dir = 1;
                else dir = -1;

                rb.velocity = new Vector3(horz * moveSpeed * dir, rb.velocity.y, rb.velocity.z);
            }
            else
            {
                if (RotRef.side == 1) dir = 1;
                else dir = -1;

                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, horz * moveSpeed * dir);
            }

            if (horz > 0) playerRender.flipX = false;
            else
                playerRender.flipX = true;

            AM.SetBool("isRun", true);

        }
        else
        {
            AM.SetBool("isRun", false);
        }

        

        if (wallMove)
        {
            if (!walled) wallMove = false;

            if (RotRef.side % 2 == 0)
            {
                if (RotRef.side == 0) dir = 1;
                else dir = -1;

                rb.velocity = new Vector3(horz * moveSpeed * dir, vert*moveSpeed, rb.velocity.z);
            }
            else
            {
                if (RotRef.side == 1) dir = 1;
                else dir = -1;

                rb.velocity = new Vector3(rb.velocity.x, vert * moveSpeed, horz * moveSpeed * dir);
            }

            if (horz > 0) playerRender.flipX = false;
            else
                playerRender.flipX = true;
        }

        if (Input.GetButtonDown("Jump") && grounded2)
        {

            if (RotRef.side % 2 == 0)
            {
                rb.velocity = new Vector3(moveSpeed * 4f * dir, jumpSpeed, 0f) * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = new Vector3(0f, jumpSpeed, moveSpeed * 4f * dir) * Time.fixedDeltaTime;
            }


            isJumping = true;            
        }

        if (Input.GetKeyDown(KeyCode.Z) && walled)
        {
            wallMove = true;
        }
        if (Input.GetKeyDown(KeyCode.S) && wallMove && grounded2)
        {
            wallMove = false;
        }
      
    }
  
    RaycastHit hit,hit1;
    RaycastHit[] hits;

    Vector3 targetp;

    public void castRays()
    {
        if (isMoving || isJumping)
        {         
            grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.6f, LayerMask.GetMask("ground")) ? true : false;
            if (RotRef.side % 2 == 0)
            {
                targetp = -rayObject.transform.forward;
                targetp.z *= 1000f;
                targetp.y = rayObject.transform.position.y;
                targetp.x = rayObject.transform.position.x;
            }
            else
            {
                if (RotRef.side == 1)
                    targetp = rayObject.transform.position + Vector3.right * 1000;
                else
                    targetp = rayObject.transform.position + Vector3.left * 1000;
            }
            hits = Physics.RaycastAll(rayObject.transform.position, targetp, 1000f,LayerMask.GetMask("ground"));
            
            Debug.DrawLine(rayObject.transform.position, targetp, Color.red);
            if (hits.Length > 0)
            {
                Vector3 pos = transform.position;

                if (RotRef.side % 2 == 0)
                    pos.z = hits[0].collider.transform.position.z;
                else
                {
                    pos.x = hits[0].collider.transform.position.x;
                    
                }
                transform.position = pos;
                hits = null;
            }
            else
            {
                if (RotRef.side % 2 == 0)
                {
                    targetp = rayObject.transform.forward;
                    targetp.z *= 1000f;
                    targetp.y = rayObject.transform.position.y;
                    targetp.x = rayObject.transform.position.x;
                }
                else
                {
                    if(RotRef.side == 1)
                    targetp = rayObject.transform.position+ Vector3.left * 1000;
                    else
                    targetp = rayObject.transform.position + Vector3.right * 1000;
                }
                Debug.DrawLine(rayObject.transform.position, targetp, Color.blue);
                if (Physics.Raycast(rayObject.transform.position, targetp, out hit1, 1000f, LayerMask.GetMask("ground")) && !grounded)
                {
                    Vector3 pos = transform.position;
                    if (RotRef.side % 2 == 0)
                        pos.z = hit1.collider.transform.position.z;
                    else
                    {
                        pos.x = hit1.collider.transform.position.x;
                        
                    }
                    transform.position = pos;

                    currCube = hit1.collider.gameObject;
                }
            }
        }
    }
}
