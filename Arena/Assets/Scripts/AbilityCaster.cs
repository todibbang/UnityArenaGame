using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool InActive = true;

	public void StartCasting(GameObject ability, GameObject sender, Vector3 targetPosition, GameObject targetGameObject)
    {
        InActive = false;
        AbilityObject = ability;
        Sender = sender;
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
        if (InActive) return;
        transform.position = Sender.transform.position;
		this.name = AbilityObject.name;
        SenderController.AddCaster(gameObject, AbilityObject);
    }

    void Update()
    {
        if (InActive) return;
        Sender.SendMessage ("AttemptToContinue", gameObject);

        var position = new Vector3();
        if (currentActivityType == Ability.AbilityType.TargetAbility) position = TargetGameObject.transform.position;
        else position = TargetDirection;

        
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
			CastTime = 0;
			switch (currentActivityType)
			{
				case Ability.AbilityType.TargetAbility:
					CastTargetAbility(AbilityObject, TargetGameObject);
					break;
				case Ability.AbilityType.SkillShotAbility:
					CastSkillShot(AbilityObject, TargetDirection);
					break;
                case Ability.AbilityType.Aoe:
                    CastSkillShot(AbilityObject, TargetDirection);
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

    void CastSkillShot(GameObject ability, Vector3 targetDirection)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(targetDirection, transform.position, Sender);
        if (ignoreCaster) proj.IgnoreCaster();
    }

    void CastTargetAbility(GameObject ability, GameObject target)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(target, transform.position, Sender);
        if (ignoreCaster) proj.IgnoreCaster();
    }

	public void OverruleMovement(){
		MovementOverruled = true;
	}

	public void Stop() {
		Sender.SendMessage ("RemoveCaster", gameObject);
	}

    public void IgnoreCaster()
    {
        ignoreCaster = true;
    }
}
