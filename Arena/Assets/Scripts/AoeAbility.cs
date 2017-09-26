using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAbility : Ability {

	public int CastRange;
	public int LiveDuration;
	int LivedTime;
    float Angel;

	public override void UseAbility(GameObject sender)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Ground"))))
		{
			if (hit.collider == null) return;
			GameObject caster = Instantiate(GameObject.Find("Caster"), sender.transform) as GameObject;
			var abilityCaster = caster.GetComponent<AbilityCaster>();

			var position = new Vector3 (hit.point.x, transform.position.y, hit.point.z);
			if (CastRange == 0)
				position = sender.transform.position;
			abilityCaster.StartCasting(gameObject, sender, position, null);
			abilityCaster.NewActivity(AbilityType.Aoe, CastTime, ExecutionTimes, CastRange, CanMove);
		}
	}

	public override void Prepare(GameObject target, Vector3 start, GameObject sender) { }

	public override void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender)
	{
        
		Sender = sender;
		var x = clickPosition.x - start.x;
		var z = clickPosition.z - start.z;
		StartPosition = start;
		TargetPosition = new Vector3(clickPosition.x, transform.position.y, clickPosition.z);
    }

	void Start () {
		transform.position = TargetPosition;
        var v2 = new Vector2(TargetPosition.x, TargetPosition.z);
        var v1 = new Vector2(Sender.transform.position.x, Sender.transform.position.z);
        Vector2 diference = v2 - v1;
        float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
        Angel = Vector2.Angle(Vector2.right, diference) * sign;
        transform.Rotate(new Vector3(0, Angel - 90, 0));
    }

    void Update () {
		LivedTime++;
		if(LivedTime >= LiveDuration) AbilityReaction(LifeExpired);
	}
}
