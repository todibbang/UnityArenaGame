using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour {

    GameObject AbilityObject;
    RaycastHit Hit;
    GameObject Sender;
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

    public void StartCasting(GameObject ability, RaycastHit hit, GameObject sender)
    {
        AbilityObject = ability;
        Hit = hit;
        Sender = sender;
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
        Sender.SendMessage("StopMoving");
    }

    void Update()
    {
        if (TargetAbilityRange > 0)
        {
            if (Vector3.Distance(TargetGameObject.transform.position, transform.position) > TargetAbilityRange)
            {
                Sender.SendMessage("Move", TargetGameObject.transform.position);
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
                    CastSkillShot(AbilityObject, TargetPosition);
                    break;
            }
            CastTimes++;
            if(CastTimes >= AbilityCastTimes)
                Destroy(gameObject);
        }

        

        if (!CanMove)
        {
            if(Vector3.Distance(StartPosition, transform.position) > 0.1)  Destroy(gameObject);
        }

        if(TargetAbilityRange > 0)
        {
            if (Vector3.Distance(TargetGameObject.transform.position, transform.position) > TargetAbilityRange) Destroy(gameObject);
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
}
