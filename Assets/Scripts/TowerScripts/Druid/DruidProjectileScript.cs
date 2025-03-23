using UnityEngine;

public class DruidProjectileScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float stopDuration = 0.5f; 
    [SerializeField] private float stopCooldown = 1.5f; 

    private Transform target;
    private DruidTower.DruidState state;
    private float stopTimer;
    private bool canStop = true;

    public void SetTarget(Transform _target, DruidTower.DruidState _state)
    {
        target = _target;
        state = _state;
        stopTimer = stopCooldown;
    }

    private void FixedUpdate()
    {
        if (!target || rb == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        if (state == DruidTower.DruidState.Regular)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                canStop = true;
                stopTimer = stopCooldown;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other == null) return;

        Health health = other.gameObject.GetComponent<Health>();
        if (health != null) health.TakeDamage(damage);

        if (state == DruidTower.DruidState.Regular && canStop)
        {
            EnemyPathing enemy = other.gameObject.GetComponent<EnemyPathing>();
            if (enemy != null)
            {
                enemy.UpdateSpeed(0f);
                enemy.Invoke("ResetSpeed", stopDuration); 
                canStop = false;
            }
        }

        Destroy(gameObject);
    }
}