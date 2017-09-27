using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System.Linq;

public class AbilityCaster : NetworkBehaviour {

	public GameObject AutoAttack;
	public GameObject[] Abilities;

    float CastTime;
    int CastTimes;
    Ability.AbilityType currentActivityType;
    GameObject TargetGameObject;
	Vector3 TargetDirection;

    float TargetAbilityRange;

    float AbilityCastTime;
    int AbilityCastTimes;
    bool CanMove;
    bool ignoreCaster;
	bool MovementOverruled;
    bool Active;
    int AbilityNumber;

    public void StartCasting(int abilityNumber, Vector3 targetPosition, GameObject targetGameObject)
    {
        CastTime = 0;
        CastTimes = 0;
        ignoreCaster = false;
        MovementOverruled = false;
        Active = true;
        AbilityNumber = abilityNumber;
		TargetGameObject = targetGameObject;
		TargetDirection = targetPosition;
    }

    public void NewActivity(Ability.AbilityType activityType, float castTime, int castTimes, float range, bool move)
    {
        currentActivityType = activityType;
        AbilityCastTime = castTime;
        AbilityCastTimes = castTimes;
        TargetAbilityRange = range;
        CanMove = move;

		if (activityType == Ability.AbilityType.SkillShotAbility) {
			var x = TargetDirection.x - gameObject.transform.position.x;
			var z = TargetDirection.z - gameObject.transform.position.z;
			TargetDirection = new Vector3(x, transform.position.y, z);
		}
    }

    void Update()
    {
        if (!isLocalPlayer) return;

		if (Input.GetMouseButtonDown (1)) {
			var hit = GetRayhit ("Body");
			if (hit.collider != null && hit.collider.tag == "Enemy") {
				UseAbility (0, AutoAttack);
				return;
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) UseAbility(0, Abilities[0]);
		if (Input.GetKeyDown(KeyCode.Alpha2)) UseAbility(1, Abilities[1]);
		if (Input.GetKeyDown(KeyCode.Alpha3)) UseAbility(2, Abilities[2]);
		if (Input.GetKeyDown(KeyCode.Alpha4)) UseAbility(3, Abilities[3]);
		if (Input.GetKeyDown(KeyCode.Alpha5)) UseAbility(4, Abilities[4]);
		if (Input.GetKeyDown(KeyCode.Alpha6)) UseAbility(5, Abilities[5]);
		if (Input.GetKeyDown(KeyCode.Alpha7)) UseAbility(6, Abilities[6]);
		if (Input.GetKeyDown(KeyCode.Alpha8)) UseAbility(7, Abilities[7]);
		if (Input.GetKeyDown(KeyCode.Alpha9)) UseAbility(8, Abilities[8]);
		if (Input.GetKeyDown(KeyCode.Alpha0)) UseAbility(9, Abilities[9]);

		if (!Active) return;

		gameObject.SendMessage ("AttemptToContinue", gameObject);

        var position = new Vector3();
        if (currentActivityType == Ability.AbilityType.TargetAbility) position = TargetGameObject.transform.position;
        else position = TargetDirection;

        if (!CanMove && MovementOverruled) Stop ();

		if (TargetAbilityRange > 0 && Vector3.Distance (position, transform.position) > TargetAbilityRange) {
			if (MovementOverruled) 
				Stop ();
			else
				gameObject.SendMessage ("Move", position);
			return;
		}

		CastTime++;
		if (CastTime >= AbilityCastTime)
		{
            CastTime = 0;
			SpawnAbility (Abilities [AbilityNumber], TargetGameObject, TargetDirection);
			/*
			switch (currentActivityType)
			{
			case Ability.AbilityType.TargetAbility:
				CmdSpawnTarget (Abilities [AbilityNumber].name, TargetGameObject, gameObject);
				break;
			case Ability.AbilityType.SkillShotAbility:
				CmdSpawnSkillShot (Abilities [AbilityNumber].name, TargetDirection, gameObject);
				break;
			case Ability.AbilityType.Aoe:
				CmdSpawnAoe (Abilities [AbilityNumber].name, TargetDirection, gameObject);
				break;
			} */

			CastTimes++;
			if (AbilityCastTimes > 0 && CastTimes >= AbilityCastTimes)
				Stop ();
		}
    }

	public void OverruleMovement(){
		MovementOverruled = true;
	}

	public void Stop() {
        Active = false;
	}

	public void UseAbility(int i, GameObject ability)
	{
		var t = ability.GetComponent<TargetAbility>();
		var a = ability.GetComponent<AoeAbility>();
		var s = ability.GetComponent<SkillShotAbility>();
		if (t != null) UseTargetAbility(i, ability);
		if (a != null) UseAoeAbility(i, ability);
		if (s != null) UseSkillShotAbility(i, ability);
	}


	public void UseAoeAbility(int i, GameObject ability)
	{
		RaycastHit hit = GetRayhit("Ground");
		if (hit.collider == null) return;
		var targetAbility = ability.GetComponent<AoeAbility>();
		StartCasting(i, targetAbility.CastRange == 0 ? transform.position : new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
		NewActivity(Ability.AbilityType.Aoe, targetAbility.CastTime, targetAbility.ExecutionTimes, targetAbility.CastRange, targetAbility.CanMove);
	}


	public void UseTargetAbility(int i, GameObject ability)
	{
		print ("using target ability");

		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Body"))))
		{
			if (hit.collider == null) return;

			int hitID = hit.collider.gameObject.GetComponent<BodyController>().ID, hitTeam = hit.collider.gameObject.GetComponent<BodyController>().TeamID;
			int senderID = gameObject.GetComponent<BodyController>().ID, senderTeam = gameObject.GetComponent<BodyController>().TeamID;

			var targetAbility = ability.GetComponent<TargetAbility>();

			print ("sender: " + senderID + ", team " + senderTeam);
			print ("Hit   : " + hitID + ", team " + hitTeam);


			var IgnoreCaster = false;
			switch (targetAbility.Interraction)
			{
			case Ability.InterractsWith.Enemy:
				if (hitTeam == senderTeam) return;
				break;
			case Ability.InterractsWith.Friendly:
				if (hitTeam != senderTeam || hitID == senderID) return;
				break;
			case Ability.InterractsWith.Self:
				if (hitID != senderID) return;
				break;
			case Ability.InterractsWith.FriendlyAndSelf:
				if (hitTeam != senderTeam) return;
				if (hitID != senderID) IgnoreCaster = true;
				break;
			}
			StartCasting(i, new Vector3(), hit.collider.gameObject);
			NewActivity(Ability.AbilityType.TargetAbility, targetAbility.CastTime, targetAbility.ExecutionTimes, targetAbility.CastRange, targetAbility.CanMove);
		}
	}

	public void UseSkillShotAbility(int i, GameObject ability)
	{
		RaycastHit hit = GetRayhit("Ground");
		if (hit.collider == null) return;
		var targetAbility = ability.GetComponent<SkillShotAbility>();
		StartCasting(i, new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
		NewActivity(Ability.AbilityType.SkillShotAbility, targetAbility.CastTime, targetAbility.ExecutionTimes, 0, targetAbility.CanMove);
	}

	RaycastHit GetRayhit(string layer)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer(layer))))
			return hit;
		return hit;
	}

	GameObject NewAbility(string name, out Ability abil) {
		int i = Abilities.ToList().FindIndex(a => a.name == name);
		GameObject newAbility = Instantiate(Abilities[i]);
		abil = newAbility.GetComponent<Ability>();
		return newAbility;
	}

	public void SpawnAbility(GameObject ability, GameObject target, Vector3 position) {
		var type = ability.GetComponent<Ability>().GetType ();
		var name = ability.name;

		switch (type)
		{
		case Ability.AbilityType.TargetAbility:
			CmdSpawnTarget (name, target, gameObject);
			break;
		case Ability.AbilityType.SkillShotAbility:
			CmdSpawnSkillShot (name, position, gameObject);
			break;
		case Ability.AbilityType.Aoe:
			CmdSpawnAoe (name, position, gameObject);
			break;
		}
	}

	[Command]
	public void CmdSpawnSkillShot(string name, Vector3 position, GameObject sender)
	{
		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(position, sender);
		abil.IgnoreCaster();

		var StartPosition = sender.transform.position;
		newAbility.transform.position = StartPosition;
		var v2 = new Vector2(position.x * 10000 + StartPosition.x, position.z * 10000 + StartPosition.z);
		var v1 = new Vector2(StartPosition.x, StartPosition.z);
		Vector2 diference = v2 - v1;
		float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
		var Angel = Vector2.Angle(Vector2.right, diference) * sign;
		newAbility.transform.Rotate(new Vector3(0, Angel + 90, 0));
		newAbility.GetComponent<Rigidbody>().velocity = newAbility.transform.forward * newAbility.GetComponent<SkillShotAbility>().ProjectileSpeed;

		NetworkServer.Spawn(newAbility);
	}

	[Command]
	public void CmdSpawnAoe(string name, Vector3 position, GameObject sender)
	{
		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(position, sender);
		abil.IgnoreCaster();

		var TargetPosition = new Vector3(position.x, newAbility.transform.position.y, position.z);

		newAbility.transform.position = TargetPosition;
		var v2 = new Vector2(TargetPosition.x, TargetPosition.z);
		var v1 = new Vector2(sender.transform.position.x, sender.transform.position.z);
		Vector2 diference = v2 - v1;
		float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
		var Angel = Vector2.Angle(Vector2.right, diference) * sign;
		newAbility.transform.Rotate(new Vector3(0, Angel - 90, 0));  

		NetworkServer.Spawn(newAbility);
	}

	[Command]
	public void CmdSpawnTarget(string name, GameObject target, GameObject sender)
	{
		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(target, sender);
		abil.IgnoreCaster();
		newAbility.transform.position = sender.transform.position;

		NetworkServer.Spawn(newAbility);
	}
}
