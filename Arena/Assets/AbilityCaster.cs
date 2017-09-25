using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour {

    GameObject AbilityObject;
    RaycastHit Hit;
    GameObject Sender;
	BodyController SenderController;
    Vector3 StartPosition;

    float CastTime;
    int CastTimes;
    Ability.AbilityType currentActivityType;
    GameObject TargetGameObject;
    Vector3 TargetPosition;

    float TargetAbilityRange;

    float AbilityCastTime;
    int AbilityCastTimes;
    bool CanMove;

	bool MovementOverruled;

    public void StartCasting(GameObject ability, RaycastHit hit, GameObject sender)
    {
        AbilityObject = ability;
        Hit = hit;
        Sender = sender;
		SenderController = Sender.GetComponent<BodyController> ();
        StartPosition = Sender.transform.position;
    }

    public void NewActivity(Ability.AbilityType activityType, float castTime, int castTimes, float range, bool move)
    {
        currentActivityType = activityType;
        AbilityCastTime = castTime;
        AbilityCastTimes = castTimes;
        TargetAbilityRange = range;
        CanMove = move;
        TargetGameObject = Hit.collider.gameObject;
        TargetPosition = new Vector3(Hit.point.x, transform.position.y, Hit.point.z);
    }

    void Start()
    {
        transform.position = Sender.transform.position;
		Sender.SendMessage("AddCaster", gameObject);
    }

    void Update()
    {
		Sender.SendMessage ("AttemptToContinue", gameObject);
		//Sender.SendMessage ("AttemptToAct", this);

		if (SenderController.FirstInqueue (gameObject)) {
			print(CanMove +" - "+ MovementOverruled);
			if (!CanMove && MovementOverruled) Stop ();

			if (TargetAbilityRange > 0)
			{
				if (Vector3.Distance(TargetGameObject.transform.position, transform.position) > TargetAbilityRange)
				{
					if (MovementOverruled)
						Stop ();
					else if(!MovementOverruled){
						Sender.SendMessage ("Move", TargetGameObject.transform.position);
						return;
					}
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
					CastSkillShot(AbilityObject, TargetPosition);
					break;
				}
				CastTimes++;
				if (AbilityCastTimes > 0 && CastTimes >= AbilityCastTimes)
					Stop ();
				

			}

			if(TargetAbilityRange > 0)
			{
				if (Vector3.Distance(TargetGameObject.transform.position, transform.position) > TargetAbilityRange) Stop();
			}
		}
    }

    void CastSkillShot(GameObject ability, Vector3 targetDirection)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(targetDirection, transform.position, Sender.gameObject);
    }

    void CastTargetAbility(GameObject ability, GameObject target)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(target, transform.position, Sender.gameObject);
    }

	public void OverruleMovement(){
		MovementOverruled = true;
	}

	public void Stop() {
		Sender.SendMessage ("RemoveCaster", gameObject);
	}
}
