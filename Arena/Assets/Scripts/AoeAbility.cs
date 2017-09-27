using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAbility : Ability {

	public int CastRange;
	public int LiveDuration;
	int LivedTime;
  

	public override void Prepare(GameObject target, GameObject sender) { }

	public override AbilityType GetType() {
		return AbilityType.Aoe;
	}

	public override void Prepare(Vector3 clickPosition, GameObject sender)
	{
        
		Sender = sender;
		TargetPosition = new Vector3(clickPosition.x, transform.position.y, clickPosition.z);
    }

	void Start () {
        /*
		transform.position = TargetPosition;
        var v2 = new Vector2(TargetPosition.x, TargetPosition.z);
        var v1 = new Vector2(Sender.transform.position.x, Sender.transform.position.z);
        Vector2 diference = v2 - v1;
        float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
        var Angel = Vector2.Angle(Vector2.right, diference) * sign;
        transform.Rotate(new Vector3(0, Angel - 90, 0));  */
    }

    void Update () {
		LivedTime++;
		if(LivedTime >= LiveDuration) AbilityReaction(LifeExpired);
	}
}
