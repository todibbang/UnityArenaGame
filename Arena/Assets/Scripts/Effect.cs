using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    int SenderId;
    public EffectType Effecttype;
	public float Effectivness;
	public int Duration;
	public int EffectTimes;
	public float EffectCooldown;

    public Vector3 Position;
    public int TimeLived;

    public enum EffectType
    {
        MagicDamage, PhysicalDamage, Heal, Stun, Slow, Root, Silence, MoveTo, BlinkTo
    }

	public Effect(Effect effect, Vector3 position) {
        print("receiving effect");
        this.SenderId = effect.SenderId;
		this.Effecttype = effect.Effecttype;
		this.Effectivness = effect.Effectivness;
		this.Duration = effect.Duration;
		this.EffectTimes = effect.EffectTimes;
		this.EffectCooldown = effect.EffectCooldown;
		this.Position = position;
        this.TimeLived = 0;
    }
	
	public void FrameLived() {
		TimeLived++;
	}

    public bool EffectiveNow()
    {
        return Duration == 0 || EffectTimes == 0 ? true : TimeLived % (Duration / EffectTimes) == 0;
    }

	public bool Active() {
		return TimeLived < Duration;
	}

	public void SetInactive() {
		TimeLived = Duration;
	}

	public void SetPosition(Vector3 position) {
		Position = position;
	}

	public Vector3 GetPosition() {
		return Position;
	}
}
