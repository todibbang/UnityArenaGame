using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    public int Health;
    public int Range;
    public float Speed;




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

	
}
