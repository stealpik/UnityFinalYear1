using UnityEngine;

public class PaladinSmiteScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float smiteSpeed = 6f;
    [SerializeField] private int smiteDmg = 15;

    private Transform target;
    private bool isUndead = false;

    public void SetTarget(Transform _target, bool _isUndead)
    {
        target = _target;
        isUndead = _isUndead; // Set whether the target is undead
    }

    private void FixedUpdate()
    {
        if (!target || rb == null)
        {
            if (rb == null) Debug.LogWarning("Rigidbody2D is not assigned on " + gameObject.name);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * smiteSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other == null) return;

        Health health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            int damageToDeal = smiteDmg;
            if (isUndead)
            {
                damageToDeal *= 2; // Double damage to undead
                Debug.Log("Smite dealt double damage to undead: " + damageToDeal);
            }
            health.TakeDamage(damageToDeal);
        }
        else
        {
            Debug.LogWarning("No Health component found on " + other.gameObject.name);
        }
        Destroy(gameObject);
    }
}