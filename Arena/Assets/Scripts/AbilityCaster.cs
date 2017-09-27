using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class AbilityCaster : MonoBehaviour {

    GameObject AbilityObject;
    //RaycastHit Hit;
    GameObject Sender;
	BodyController SenderController;
    Vector3 StartPosition;

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


    public void StartCasting(int abilityNumber, GameObject ability, Vector3 targetPosition, GameObject targetGameObject)
    {
        CastTime = 0;
        CastTimes = 0;
        ignoreCaster = false;
        MovementOverruled = false;
        Active = true;

        AbilityNumber = abilityNumber;
        AbilityObject = ability;
        Sender = gameObject;
		SenderController = Sender.GetComponent<BodyController> ();
        StartPosition = Sender.transform.position;

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
			var x = TargetDirection.x - Sender.transform.position.x;
			var z = TargetDirection.z - Sender.transform.position.z;
			TargetDirection = new Vector3(x, transform.position.y, z);
		}
    }

    void Start()
    {
        if (!Active) return;
        transform.position = Sender.transform.position;
		this.name = AbilityObject.name;
        SenderController.AddCaster(gameObject, AbilityObject);
    }

    void Update()
    {
        //print(isLocalPlayer + " - " + Active);

        //if (!isLocalPlayer) return;
        if (!Active) return;
        Sender.SendMessage ("AttemptToContinue", gameObject);

        var position = new Vector3();
        if (currentActivityType == Ability.AbilityType.TargetAbility) position = TargetGameObject.transform.position;
        else position = TargetDirection;

        print(CanMove + " - " + MovementOverruled);

        if (!CanMove && MovementOverruled) Stop ();

		if (TargetAbilityRange > 0)
		{
            if (Vector3.Distance(position, transform.position) > TargetAbilityRange)
			{
				if (MovementOverruled) 
					Stop ();
				else if(!MovementOverruled)
					Sender.SendMessage ("Move", position);
				return;
			}
		}

		CastTime++;
		if (CastTime >= AbilityCastTime)
		{
            print("Shooting : " + currentActivityType);

            CastTime = 0;
			switch (currentActivityType)
			{
				case Ability.AbilityType.TargetAbility:
                    CmdCastTargetAbility(AbilityObject, TargetGameObject);
					break;
				case Ability.AbilityType.SkillShotAbility:
                    CmdCastSkillShot(AbilityObject, TargetDirection);
					break;
                case Ability.AbilityType.Aoe:
                    CmdCastSkillShot(AbilityObject, TargetDirection);
                    break;
			}
			CastTimes++;
			if (AbilityCastTimes > 0 && CastTimes >= AbilityCastTimes)
				Stop ();
		}

		if(TargetAbilityRange > 0)
		{
			if (Vector3.Distance(position, transform.position) > TargetAbilityRange) Stop();
		}
    }

    //[Command]
    void CmdCastSkillShot(GameObject ability, Vector3 targetDirection)
    {
        SenderController.CmdCast(AbilityNumber, null, targetDirection, ignoreCaster, currentActivityType);
    }

    //[Command]
    void CmdCastTargetAbility(GameObject ability, GameObject target)
    {
        SenderController.CmdCast(AbilityNumber, target, new Vector3(), ignoreCaster, currentActivityType);
    }

	public void OverruleMovement(){
		MovementOverruled = true;
	}

	public void Stop() {
        Active = false;
	}

    public void IgnoreCaster()
    {
        ignoreCaster = true;
    }
}
