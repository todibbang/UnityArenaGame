using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stats : NetworkBehaviour
{
	//public Text Text;
	public RectTransform healthBar;

    public float Health;
    public int Range;
    public float Speed;

    [SyncVar]
    float speedMultiplier;
    [SyncVar]
    float CurrentHealth;
    bool canMove;
    bool canCast;

    bool hasOverruleMoveTo;
    Vector3 overruleMoveTo;

    //[SyncVar]
    //List<Effect> Effects = new List<Effect>();

    /*
    public struct SyncEffect
    {
        int SenderId;
        public Effect.EffectType Effecttype;
        public float Effectivness;
        public int Duration;
        public int EffectTimes;
        public float EffectCooldown;

        Vector3 Position;
        int TimeLived;

        public SyncEffect(Effect effect)
        {
            SenderId = 0;
            Effecttype = effect.Effecttype;
            Effectivness = effect.Effectivness;
            Duration = effect.Duration;
            EffectTimes = effect.EffectTimes;
            EffectCooldown = effect.EffectCooldown;
            Position = effect.Position;
            TimeLived = effect.TimeLived;
        }
    }*/

    //public class SyncListSyncEffect : SyncListStruct<Effect> {  }
    //public SyncListSyncEffect SyncEffects = new SyncListSyncEffect();
    //public SyncListSyncEffect EffectsToLose = new SyncListSyncEffect();
    List<Effect> SyncEffects = new List<Effect>();
    List<Effect> EffectsToLose = new List<Effect>();

    void Start()
    {
		CurrentHealth = Health;
    }

	void Update() {
		//print(CurrentHealth+" / "+Health);
		var w = ((float)CurrentHealth / (float)Health * 200);
		healthBar.sizeDelta = new Vector2(w, healthBar.sizeDelta.y);
		healthBar.anchoredPosition = new Vector2 (200 - (w / 2), 0);

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
                        transform.position = effect.GetPosition();
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

    
	/*
    public int[] GetHealth()
    {
        return new[] { Health, CurrentHealth };
    }*/
}
