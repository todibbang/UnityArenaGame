using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Stats : MonoBehaviour
{
	public Text Text;
	public RectTransform healthBar;

    public float Health;
    public int Range;
    public float Speed;

	float CurrentHealth;

    void Start()
    {
		CurrentHealth = Health;

    }

	void Update() {
		print(CurrentHealth+" / "+Health);
		var w = ((float)CurrentHealth / (float)Health * 200);
		healthBar.sizeDelta = new Vector2(w, healthBar.sizeDelta.y);
		healthBar.anchoredPosition = new Vector2 (200 - (w / 2), 0);
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
	/*
    public int[] GetHealth()
    {
        return new[] { Health, CurrentHealth };
    }*/
}
