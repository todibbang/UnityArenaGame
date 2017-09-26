using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShotAbility : Ability {

	public int ProjectileSpeed;
	public int TravelLength;

	public override void UseAbility(GameObject sender)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Ground"))))
		{
			if (hit.collider == null) return;
			GameObject caster = Instantiate(AbilityCaster, sender.transform) as GameObject;
			var abilityCaster = caster.GetComponent<AbilityCaster>();
			abilityCaster.StartCasting(gameObject, sender, new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
			abilityCaster.NewActivity(AbilityType.SkillShotAbility, CastTime, ExecutionTimes, 0, CanMove);
		}
	}

	public override void Prepare(GameObject target, Vector3 start, GameObject sender) { }

	public override void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender)
	{
		Sender = sender;
		//var x = clickPosition.x - start.x;
		//var z = clickPosition.z - start.z;
		StartPosition = start;
		//print (x + ", " + z);
		TargetPosition = new Vector3(clickPosition.x*10000 + start.x, transform.position.y, clickPosition.z*10000 + start.z);
	}

	void Start () {
		transform.position = StartPosition;
	}

	void Update () {
		Vector3 position = new Vector3();
		if (TargetGameObject != null) position = TargetGameObject.transform.position;
		else position = TargetPosition;

		float step = ProjectileSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, position, step);

		if (Vector3.Distance(transform.position, StartPosition) > TravelLength)
			AbilityReaction(LifeExpired);
	}
}
