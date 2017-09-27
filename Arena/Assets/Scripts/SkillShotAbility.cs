using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShotAbility : Ability {

	public int ProjectileSpeed;
	public int TravelLength;

	public override void Prepare(GameObject target, GameObject sender) { }

	public override AbilityType GetType() {
		return AbilityType.SkillShotAbility;
	}

	public override void Prepare(Vector3 clickPosition, GameObject sender)
	{
		//Sender = sender;
        //StartPosition = sender.transform.position;
        //TargetPosition = new Vector3(clickPosition.x * 10000 + sender.transform.position.x, transform.position.y, clickPosition.z * 10000 + sender.transform.position.z);
    }

	void Start () {
    }

	void Update () {
        if (!isServer) return;
		if (Vector3.Distance(transform.position, StartPosition) > TravelLength)
			AbilityReaction(LifeExpired); 
	}
}
