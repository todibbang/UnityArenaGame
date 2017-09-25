using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect {

    int SenderId;
    EffectType Effecttype;
    float Modifier;
    float EffectCooldown;


    public enum EffectType
    {
        MagicDamage, PhysicalDamage, Stun, Slow
    }
	

}
