using UnityEngine;
using System.Collections;

public class DruidTower : MonoBehaviour
{
    [Header("State Settings")]
    [SerializeField] private DruidState currentState = DruidState.Regular;
    [SerializeField] private float switchInterval = 5f; 
    [Header("Bear State Settings")]
    [SerializeField] private float healthMultiplier = 1.5f; 
    [SerializeField] private float damageReduction = 0.75f; 
    [Header("Wolf State Settings")]
    [SerializeField] private float damageMultiplier = 1.5f; 
    [SerializeField] private float attackSpeedMultiplier = 1.25f; 

    private TowerHealthManager healthManager;
    private TowerStatsManager statsManager;
    private float switchTimer;
    private float originalHealth;
    private float originalMaxHealth;
    private int originalDmg;
    private float originalAttackRate;

    public enum DruidState
    {
        Regular,
        Bear,
        Wolf
    }

    private void Start()
    {
        healthManager = GetComponent<TowerHealthManager>();
        statsManager = GetComponent<TowerStatsManager>();

        originalHealth = healthManager.GetHealth();
        originalMaxHealth = healthManager._hitPointsMax;
        originalDmg = statsManager.GetDmg();
        originalAttackRate = statsManager.GetAttackRate();

        switchTimer = switchInterval;
    }

    private void Update()
    {
        switchTimer -= Time.deltaTime;
        if (switchTimer <= 0f)
        {
            SwitchToNextState();
            switchTimer = switchInterval;
        }
    }

    private void SwitchToNextState()
    {
        if (currentState == DruidState.Regular) SwitchState(DruidState.Bear);
        else if (currentState == DruidState.Bear) SwitchState(DruidState.Wolf);
        else SwitchState(DruidState.Regular);
    }

    private void SwitchState(DruidState newState)
    {
        healthManager.SetHealth((int)originalHealth);
        healthManager._hitPointsMax = (int)originalMaxHealth;
        statsManager.SetDmg(originalDmg);
        statsManager.SetRate((int)originalAttackRate);

        currentState = newState;
        if (currentState == DruidState.Bear)
        {
            healthManager.SetHealth((int)(originalHealth * healthMultiplier));
            healthManager._hitPointsMax = (int)(originalMaxHealth * healthMultiplier);
        }
        else if (currentState == DruidState.Wolf)
        {
            statsManager.SetDmg((int)(originalDmg * damageMultiplier));
            statsManager.SetRate((int)(originalAttackRate * attackSpeedMultiplier));
        }
    }

    public DruidState GetCurrentState()
    {
        return currentState;
    }

    public float ApplyDamageReduction(float damage)
    {
        if (currentState == DruidState.Bear) return damage * damageReduction;
        return damage;
    }
}