using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public float damage;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Collider[] enemiesToDamage = Physics.OverlapSphere(transform.position, 2f, whatIsPlayer);

            for (int i = 0; i < enemiesToDamage.Length; i += 1)
                enemiesToDamage[i].GetComponent<Player>().TakeDamage(damage);
        
            Destroy(gameObject);
        }
    }
}
