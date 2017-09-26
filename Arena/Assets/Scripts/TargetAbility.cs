using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAbility : Ability {

	public int CastRange;
	public int ProjectileSpeed;

	public override void UseAbility(GameObject sender)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Body"))))
		{
			if (hit.collider == null) return;
			GameObject caster = Instantiate(AbilityCaster, sender.transform) as GameObject;
			var abilityCaster = caster.GetComponent<AbilityCaster>();
			abilityCaster.StartCasting(gameObject, sender, new Vector3(), hit.collider.gameObject);
			abilityCaster.NewActivity(AbilityType.TargetAbility, CastTime, ExecutionTimes, CastRange, CanMove);
		}
	}
	public override void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender) { }

	public override void Prepare(GameObject target, Vector3 start, GameObject sender)
	{
		Sender = sender;
		TargetGameObject = target;
		StartPosition = start;
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
	}
}
