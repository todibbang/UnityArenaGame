using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stats : NetworkBehaviour
{
	public RectTransform healthBar;

    public float Health;
    public int Range;
    public float Speed;

    [SyncVar]
    float speedMultiplier;
    [SyncVar(hook = "UpdateHealth")]
    public float CurrentHealth;
    [SyncVar]
    bool canMove;
    [SyncVar]
    bool canCast;
    [SyncVar]
    bool hasOverruleMoveTo;
    [SyncVar]
    bool hasOverruleBlinkTo;
    [SyncVar]
    Vector3 overruleMoveTo;
    
    List<Effect> SyncEffects = new List<Effect>();
    List<Effect> EffectsToLose = new List<Effect>();

    void Start()
    {
		CurrentHealth = Health;
    }

    void UpdateHealth(float current)
    {
        var w = ((float)current / (float)Health * 200);
        healthBar.sizeDelta = new Vector2(w, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2(200 - (w / 2), 0);
    }

	void Update() {
        ProcessEffects();
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
        return Speed * speedMultiplier;
    }

    public float AutoAttackCastTime()
    {
        return 50;
    }

    public bool HasOverruleMoveTo() { return hasOverruleMoveTo; }

    public bool HasOverruleBlinkTo() { return hasOverruleBlinkTo;  }

    public Vector3 GetMoveToPosition()
    {
        return overruleMoveTo;
    }

    public void Hit(Effect effect)
    {
        if (!isServer) return;
        print("Body hit with " + effect.Effecttype);
        SyncEffects.Add(effect);
    }

    public void ResetTemporaryValues()
    {
        speedMultiplier = 1;
        canMove = true;
        canCast = true;
        hasOverruleMoveTo = false;
        hasOverruleBlinkTo = false;
    }

    public void ProcessEffects()
    {
        if (!isServer) return;

        ResetTemporaryValues();

        foreach (var effect in SyncEffects)
        {
            print("processing effect " + effect.Effecttype);
            if (effect.EffectiveNow())
            {
                print("effect is effective now");
                switch (effect.Effecttype)
                {
                    case Effect.EffectType.PhysicalDamage:
                        CurrentHealth -= effect.Effectivness;
                        break;
                    case Effect.EffectType.MagicDamage:
                        CurrentHealth -= effect.Effectivness;
                        break;
                    case Effect.EffectType.Heal:
                        CurrentHealth = Mathf.Min(Health, CurrentHealth + effect.Effectivness);
                        break;
                    case Effect.EffectType.Root:
                        canMove = false;
                        break;
                    case Effect.EffectType.Slow:
                        speedMultiplier -= effect.Effectivness;
                        break;
                    case Effect.EffectType.Stun:
                        canMove = false;
                        canCast = false;
                        break;
                    case Effect.EffectType.Silence:
                        canCast = false;
                        break;
                    case Effect.EffectType.MoveTo:
                        if (Vector3.Distance(transform.position, effect.GetPosition()) > 0.1)
                        {
                            hasOverruleMoveTo = true;
                            overruleMoveTo = effect.GetPosition();
                        }
                        else effect.SetInactive();                    
                        break;
                    case Effect.EffectType.BlinkTo:
                        hasOverruleBlinkTo = true;
                        overruleMoveTo = effect.GetPosition();
                        //transform.position = effect.GetPosition();
                        effect.SetInactive();
                        break;
                }
            }
            effect.FrameLived();
            if (!effect.Active())
            {
                EffectsToLose.Add(effect);
            }
        }
        foreach (var effect in EffectsToLose)
        {
            SyncEffects.Remove(effect);
            //Destroy(effect);
        }
        EffectsToLose.Clear();
    }
}
