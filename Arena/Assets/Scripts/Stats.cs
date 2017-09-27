using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    public int Health;
    public int Range;
    public float Speed;

    int CurrentHealth;

    void Start()
    {
        CurrentHealth = Health;
    }

    public bool InRange(GameObject target)
    {
        return Vector3.Distance(transform.position, target.transform.position) < Range;
    }

    public bool InRange(GameObject target, float targetAbilityRange)
    {
        return Vector3.Distance(transform.position, target.transform.position) < targetAbilityRange;
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public float AutoAttackCastTime()
    {
        return 50;
    }

    public int[] GetHealth()
    {
        return new[] { Health, CurrentHealth };
    }
}
