using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkorbscript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float orbSpeed = 5f;
    [SerializeField] private int orbDmg = 10;

    private Transform target; // Single target per orb

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * orbSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other == null) return;

        Health health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(orbDmg);
        }
        Destroy(gameObject);
    }
}
