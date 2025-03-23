using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FighterScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 2f;
    [SerializeField] private float attackRate = 1f; // Attacks per second
    [SerializeField] private float moveSpeed = 3f;   // Speed to move toward enemy and back
    [SerializeField] private int meleeDamage = 5;   // Damage per melee hit
    [SerializeField] private int baseUpgradeCost = 100;

    private float targetingRangeBase;
    private float attackRateBase;
    private Vector2 startPosition; // Where the tower returns after attacking
    private Transform target;
    private float attackCooldown;
    private Rigidbody2D rb;
    private int level = 1;

    private enum State { Idle, MovingToEnemy, Attacking, Returning }
    private State currentState = State.Idle;

    private void Start()
    {
        targetingRangeBase = targetingRange;
        attackRateBase = attackRate;
        startPosition = transform.position; // Record initial position
        rb = GetComponent<Rigidbody2D>();
        //upgradeButton.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (target == null)
                {
                    FindTarget();
                }
                else if (CheckTargetInRange())
                {
                    currentState = State.MovingToEnemy;
                }
                else
                {
                    target = null; // Out of range, reset target
                }
                RotateTowardTarget();
                break;

            case State.MovingToEnemy:
                if (target == null)
                {
                    currentState = State.Returning; // Enemy gone, return home
                }
                else if (Vector2.Distance(transform.position, target.position) <= 0.1f) // Close enough to attack
                {
                    currentState = State.Attacking;
                    rb.velocity = Vector2.zero; // Stop moving
                }
                else
                {
                    MoveToward(target.position);
                    RotateTowardTarget();
                }
                break;

            case State.Attacking:
                if (target == null)
                {
                    currentState = State.Returning; // Enemy destroyed, return
                }
                else
                {
                    attackCooldown += Time.deltaTime;
                    if (attackCooldown >= 1f / attackRate)
                    {
                        Attack();
                        attackCooldown = 0f;
                    }
                }
                break;

            case State.Returning:
                if (Vector2.Distance(transform.position, startPosition) <= 0.1f)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = startPosition; // Snap to exact position
                    currentState = State.Idle;
                }
                else
                {
                    MoveToward(startPosition);
                    RotateTowardTarget(); // Optional: face target while returning
                }
                break;
        }
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetInRange()
    {
        return target != null && Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void MoveToward(Vector2 destination)
    {
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void RotateTowardTarget()
    {
        if (target == null) return;
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = targetRotation;
    }

    private void Attack()
    {
        if (target != null)
        {
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(meleeDamage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (currentState == State.MovingToEnemy && other.transform == target)
        {
            currentState = State.Attacking;
            rb.velocity = Vector2.zero;
        }
    }

    // Upgrade UI and logic (unchanged)
    public void OpenUpgradeUI() { upgradeUI.SetActive(true); }
    public void CloseUpgradeUI() { upgradeUI.SetActive(false); UIManager.main.SetHoveringState(false); }
    public void Upgrade()
    {
        if (CalcCost() > LevelManager.main.gold) return;
        LevelManager.main.Buy(CalcCost());
        level++;
        attackRate = CalcAttackRate();
        targetingRange = CalcRange();
        CloseUpgradeUI();
    }

    private int CalcCost() { return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f)); }
    private float CalcAttackRate() { return attackRateBase * Mathf.Pow(level, 0.5f); }
    private float CalcRange() { return targetingRange * Mathf.Pow(level, 0.6f); }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.forward, targetingRange);
    }
}