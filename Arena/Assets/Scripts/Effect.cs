using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    int SenderId;
    public EffectType Effecttype;
	public float Damage;
	public int Duration;
	public int EffectTimes = 1;
	public bool OverrulesControles;
	public float EffectCooldown;

	Vector3 Position;
	int TimeLived;

    public enum EffectType
    {
        MagicDamage, PhysicalDamage, Stun, Slow, Root, MoveTo, BlinkTo
    }

	public Effect(Effect effect, Vector3 position) {
		this.SenderId = effect.SenderId;
		this.Effecttype = effect.Effecttype;
		this.Damage = effect.Damage;
		this.Duration = effect.Duration;
		this.EffectTimes = effect.EffectTimes;
		this.OverrulesControles = effect.OverrulesControles;
		this.EffectCooldown = effect.EffectCooldown;
		this.Position = position;
	}
	
	public void FrameLived() {
		TimeLived++;
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
