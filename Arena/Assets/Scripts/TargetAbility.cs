using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAbility : Ability {

	public int CastRange;
	public int ProjectileSpeed;

    /*
	public override void UseAbility(GameObject sender)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Body"))))
		{
			if (hit.collider == null) return;

            int hitID = hit.collider.gameObject.GetComponent<BodyController>().ID, hitTeam = hit.collider.gameObject.GetComponent<BodyController>().TeamID;
            int senderID = sender.gameObject.GetComponent<BodyController>().ID, senderTeam = sender.gameObject.GetComponent<BodyController>().TeamID;

            var IgnoreCaster = false;
            switch (Interraction)
            {
                case InterractsWith.Enemy:
                    if (hitTeam == senderTeam) return;
                    break;
                case InterractsWith.Friendly:
                    if (hitTeam != senderTeam || hitID == senderID) return;
                    break;
                case InterractsWith.Self:
                    if (hitID != senderID) return;
                    break;
                case InterractsWith.FriendlyAndSelf:
                    if (hitTeam != senderTeam) return;
                    if (hitID != senderID) IgnoreCaster = true;
                    break;
            }

            //GameObject caster = Instantiate(GameObject.Find("Caster"), sender.transform) as GameObject;
            var abilityCaster = sender.GetComponent<AbilityCaster>();
            abilityCaster.StartCasting(gameObject, sender, new Vector3(), hit.collider.gameObject);
			abilityCaster.NewActivity(AbilityType.TargetAbility, CastTime, ExecutionTimes, CastRange, CanMove);
            if (IgnoreCaster) abilityCaster.IgnoreCaster();
		}
	} */

	public override void Prepare(Vector3 clickPosition, GameObject sender) { }

	public override AbilityType GetType() {
		return AbilityType.TargetAbility;
	}

	public override void Prepare(GameObject target, GameObject sender)
	{
		Sender = sender;
		TargetGameObject = target;
	}
	/*
	void Start () {
		transform.position = StartPosition;
	}*/

	void Update () {
		Vector3 position = new Vector3();
		if (TargetGameObject != null) position = TargetGameObject.transform.position;
		else position = TargetPosition;

		float step = ProjectileSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, position, step);
	}
}
