using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShotAbility : Ability {

	public int ProjectileSpeed;
	public int TravelLength;

    /*
	public override void UseAbility(GameObject sender)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Ground"))))
		{
			if (hit.collider == null) return;
            //GameObject caster = Instantiate(GameObject.Find("Caster"), sender.transform) as GameObject;
            var abilityCaster = sender.GetComponent<AbilityCaster>();
			abilityCaster.StartCasting(gameObject, sender, new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
			abilityCaster.NewActivity(AbilityType.SkillShotAbility, CastTime, ExecutionTimes, 0, CanMove);
		}
	} */

	public override void Prepare(GameObject target, Vector3 start, GameObject sender) { }

	public override void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender)
	{
		Sender = sender;
		StartPosition = start;
		//TargetPosition = new Vector3(clickPosition.x + start.x, transform.position.y, clickPosition.z + start.z);
        TargetPosition = new Vector3(clickPosition.x * 10000 + start.x, transform.position.y, clickPosition.z * 10000 + start.z);
    }

	void Start () {
        
		transform.position = StartPosition;
        var v2 = new Vector2(TargetPosition.x, TargetPosition.z);
        var v1 = new Vector2(Sender.transform.position.x, Sender.transform.position.z);
        Vector2 diference = v2 - v1;
        float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
        var Angel = Vector2.Angle(Vector2.right, diference) * sign;
        transform.Rotate(new Vector3(0, Angel + 90, 0));
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 6; 
    }

	void Update () {
        /*
		Vector3 position = new Vector3();
		if (TargetGameObject != null) position = TargetGameObject.transform.position;
		else position = TargetPosition;

		float step = ProjectileSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, position, step); */
        
		if (Vector3.Distance(transform.position, StartPosition) > TravelLength)
			AbilityReaction(LifeExpired); 
	}
}
