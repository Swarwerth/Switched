using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject rayObject;
    [SerializeField] public Transform refForGameover;
    [SerializeField] public Transform spawnPoint;
    public bool disableRotation;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;
    public float dashSpeed = 5f;
    private int dashCount = 1;
    

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;
    private bool canJump;
    private int jumpCount = 2;

    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.4f;
    public Vector3 colliderOffset;

    [Header("Life")]
    public float health;
    public float maxHealth;
    public Image healthBar;
    private int cntDeath;

    [Header("Attack")]
    public float startTimeBetweenAttack;
    private float timeBetweenAttack;
    public Transform attackPos;
    public float attackRange;
    public LayerMask whatIsEnemy;
    public LayerMask whatIsBoss;
    public float damage;
    public ParticleSystem blood;

    [Header("Wall Movement")]
    public bool walled, wallMove;
    bool[] wallHits = new bool[4];

    [Header("Sound")]
    public AudioSource soundSource;
    public AudioClip JumpSound;
    public AudioClip DashSound;
    public AudioClip AttackSound;

    private void Start()
    {
        cntDeath = 0;
        refForGameover = GameObject.Find("RefForGameover").transform;
        spawnPoint = GameObject.Find("SpawnPoint").transform;
    }

    void Update()
    {
        if (!photonView.IsMine || Timer.paused) return;

        refForGameover = GameObject.Find("RefForGameover").transform;

        if (health <= 0) Respawn();

        if (cntDeath == 3) Debug.Log("-GAMEOVER-");

        if (timeBetweenAttack >= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Collider[] enemiesToDamage = Physics.OverlapSphere(attackPos.position, attackRange, whatIsEnemy);
                Collider[] bossToDamage = Physics.OverlapSphere(attackPos.position, attackRange, whatIsBoss);

                timeBetweenAttack = startTimeBetweenAttack;
                animator.SetTrigger("attack");
                soundSource.PlayOneShot(AttackSound);

                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    if ((whatIsEnemy.value & 1 << 3) > 0) enemiesToDamage[i].GetComponent<Patrol>().TakeDamage(damage);
                    if (((whatIsEnemy.value & 1 << 6) > 0) && enemiesToDamage[i] != gameObject) enemiesToDamage[i].GetComponent<Player>().TakeDamage(damage);
                }

                for (int i = 0; i < bossToDamage.Length; i++)
                {
                    if ((whatIsBoss.value & 1 << 7) > 0) bossToDamage[i].GetComponent<BossController>().TakeDamage(damage);
                }
            }
        }
        else timeBetweenAttack -= Time.deltaTime;

        bool wasOnGround = onGround;
        onGround = Physics.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics.Raycast(transform.position, Vector2.down, groundLength, groundLayer);

        if (onGround || walled) 
        {
            jumpCount = 2;
            dashCount = 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
            if (!onGround && !walled && jumpCount > 0) canJump = true;
            else canJump = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("Pause").GetComponent<Timer>().TogglePause();
        }

        walled = Physics.Raycast(transform.position, transform.forward, out hit, 4f, LayerMask.GetMask("ladder"));
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isMoving = (direction.x != 0);


        castRays();

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.D))
        {
            if (onGround) rb.velocity = new Vector3(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.Z) && walled)
        {
            wallMove = true;
            animator.SetBool("isClimb", true);
            rb.isKinematic = true;
        }
        if (Input.GetKeyDown(KeyCode.S) && wallMove)
        {
            rb.isKinematic = false;
            wallMove = false;
            animator.SetBool("isClimb", false);
        }
        if (transform.position.y <= refForGameover.position.y) Respawn();
        if (!onGround && Input.GetKeyDown(KeyCode.LeftShift) && dashCount > 0) Dash();
    }

    void FixedUpdate()
    {
        if (!wallMove)
            moveCharacter(direction.x);

        if (wallMove)
        {
            if (!walled) { wallMove = false; animator.SetBool("isClimb", false); rb.isKinematic = false; };
            if (RotRef.side % 2 == 0)
            {
                if (RotRef.side == 0) dir = 1;
                else dir = -1;

                transform.Translate(direction.x * dir * 0.05f, direction.y * 0.05f, 0);
            }
            else
            {
                if (RotRef.side == 1) dir = 1;
                else dir = -1;

                transform.Translate(0, direction.y * 0.05f, direction.x * dir * 0.05f);
            }
        }
        if (jumpTimer > Time.time && (onGround || walled || canJump))
        {
            walled = false; 
            wallMove = false;
            rb.isKinematic = false;
            Jump();
        }
        modifyPhysics();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        blood.Play();
        healthBar.fillAmount = health / 100f;
    }

    void Respawn()
    {
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        health = maxHealth;
        healthBar.fillAmount = health / 100f;
        transform.position = spawnPoint.position;
        cntDeath += 1;
    }

    void Dash()
    {
        if (facingRight) 
        {
            rb.velocity = Vector2.right * dashSpeed;
            rb.AddForce(Vector2.up * 5f, ForceMode.Impulse);
        }
        if (!facingRight) 
        {
            rb.velocity = Vector2.left * dashSpeed;
            rb.AddForce(Vector2.up * 5f, ForceMode.Impulse);
        }
        soundSource.PlayOneShot(DashSound);
        dashCount --;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode.Impulse);
        soundSource.PlayOneShot(JumpSound);
        jumpTimer = 0;
        jumpCount--;
    }


    int dir = 1;
    void moveCharacter(float horizontal)
    {
        if (RotRef.side % 2 == 0)
        {
            if (RotRef.side == 0) dir = 1;
            else dir = -1;
            rb.AddForce(new Vector3(horizontal * dir * moveSpeed,0,0));
        }
        else
        {
            if (RotRef.side == 1) dir = 1;
            else dir = -1;
            rb.AddForce(new Vector3(0, 0, horizontal * dir * moveSpeed));
        }

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }
        

        if (direction.x != 0)
        {
            animator.SetBool("isRun", true);
            animator.SetBool("isClimb", false);
        }
        else
        {
            animator.SetBool("isRun", false);
            animator.SetBool("isClimb", false);
            if (onGround)
                rb.velocity = new Vector3(0,0,0);
        }
    }

    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        characterHolder.gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }

    RaycastHit hit, hit1;
    RaycastHit[] hits;
    public static bool isMoving, isJumping;
    Vector3 targetp;
    [HideInInspector]
    public bool grounded;
    public void castRays()
    {
        if (isMoving || isJumping)
        {

            grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f, LayerMask.GetMask("ground")) ? true : false;
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
            hits = Physics.RaycastAll(rayObject.transform.position, targetp, 1000f, LayerMask.GetMask("ground"));

            Debug.DrawLine(rayObject.transform.position, targetp, Color.red);
            if (hits.Length > 0)
            {
                if (!Physics.Raycast(rayObject.transform.position, transform.forward, out hit, 20f, LayerMask.GetMask("Default")))
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
                    if (RotRef.side == 1)
                        targetp = rayObject.transform.position + Vector3.left * 1000;
                    else
                        targetp = rayObject.transform.position + Vector3.right * 1000;
                }
                Debug.DrawLine(rayObject.transform.position, targetp, Color.blue);

                if (Physics.Raycast(rayObject.transform.position, targetp, out hit1, 1000f, LayerMask.GetMask("ground")) && !grounded)
                {
                    Vector3 pos = transform.position;

                    if (!Physics.Raycast(rayObject.transform.position, transform.forward, out hit, 20f, LayerMask.GetMask("Default")))
                    {
                        if (RotRef.side % 2 == 0)
                            pos.z = hit1.collider.transform.position.z;
                        else
                        {
                            pos.x = hit1.collider.transform.position.x;

                        }
                        transform.position = pos;
                    }
                }

            }
        }
    }
}