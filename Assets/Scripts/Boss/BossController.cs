using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossController : MonoBehaviour
{
    public GameObject fireball;
    public float speed;
    public float health;

    private float timeBetweenAttack = 3f;
    private bool coolingDown = true;

    public ParticleSystem blood;

    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);

        System.Random random = new System.Random();

        if (!coolingDown)
        {
            int currentSpawnPoint = random.Next(spawnPoints.Length);

            GameObject clone = Instantiate(fireball, spawnPoints[currentSpawnPoint].position, spawnPoints[currentSpawnPoint].rotation) as GameObject;
            Rigidbody fireballRb = clone.GetComponent<Rigidbody>();

            fireballRb.velocity = spawnPoints[currentSpawnPoint].right * speed;

            coolingDown = true;
        }

        else
        {
            timeBetweenAttack -= Time.deltaTime;
            if (timeBetweenAttack <= 0) 
            {
                coolingDown = false;
                timeBetweenAttack = 3f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        blood.Play();
    }
}
