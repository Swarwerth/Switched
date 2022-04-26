using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed, attackRange, viewRange;
    private float distance;
    public Transform player;
    public int health;

    public Transform attackPos;
    public float hitRange;
    public LayerMask whatIsPlayer;
    public int damage;
    public ParticleSystem blood;

    [HideInInspector]
    public bool mustPatrol = true;
    public bool mustFlip;
    public Rigidbody rb;
    public Transform grdCheckPos;
    public LayerMask ground;
    public Animator animator;

    void Update()
    {
        player = GameObject.Find("player").transform;
        distance = Vector2.Distance(transform.position, player.position);

        if (health <= 0) Destroy(gameObject);

        if (distance <= attackRange) 
        {
            mustPatrol = false;
            rb.velocity = Vector3.zero;
            Attack();
        }
        else 
        {
            mustPatrol = true;
            animator.SetBool("canAttack", false);
        }
        if (distance <= viewRange && mustPatrol) 
        {
            DoPatrol();
            animator.SetBool("isRun", true);
        }
        else animator.SetBool("isRun", false);

    }

    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            Collider[] hitGrd = Physics.OverlapSphere(grdCheckPos.position, 0.5f, ground);
            mustFlip = hitGrd.Length <= 0;
        } 
    }

    void DoPatrol()
    {
        if (mustFlip) Flip();
        rb.velocity = new Vector3(speed * Time.deltaTime, rb.velocity.y, rb.velocity.z);
    }

    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        speed *= -1;
        mustPatrol = true;
    }

    void Attack()
    {
        animator.SetBool("canAttack", true);
        Collider[] enemiesToDamage = Physics.OverlapSphere(attackPos.position, hitRange, whatIsPlayer);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Player>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        blood.Play();
    }
}
