using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    

	public void NewActivity(ActivityType activityType, GameObject ability, float castTime, float range, GameObject target, Vector3 position)
    {
        currentActivityType = activityType;
        Ability = ability;
        AbilityCastTime = castTime;
        TargetAbilityRange = range;
        TargetGameObject = target;
        TargetPosition = position;
    }

    public enum ActivityType{ None, Move, AutoAttack, TargetAbility, SkillShotAbility }

    public Stats Stats;
    public GameObject AutoAttack;
    public GameObject FirstAbility;
    public GameObject SecondAbility;
    public GameObject ThirdAbility;
    public GameObject ForthAbility;

    GameObject Ability;
    float CastTime;

    ActivityType currentActivityType;
    GameObject TargetGameObject;
    Vector3 TargetPosition;

    float TargetAbilityRange;

    float AbilityCastTime;





    Ray ray;
    RaycastHit hit;


    void Update()
    {
        if(gameObject.tag == "Player")
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        print("Attack enemy");
                        //currentActivityType = ActivityType.AutoAttack;
                        //TargetGameObject = hit.collider.gameObject;
                        AutoAttack.GetComponent<Ability>().UseAbility(hit, gameObject);
                    }
                    else if (hit.collider.tag == "Ground")
                    {
                        NewActivity(ActivityType.Move, null, 0, 0, null, new Vector3(hit.point.x, transform.position.y, hit.point.z));
                    }
                }

                if (Input.GetKeyDown(KeyCode.Q)) FirstAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.W)) SecondAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.E)) ThirdAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.R)) ForthAbility.GetComponent<Ability>().UseAbility(hit, gameObject);

            }
        }

        switch (currentActivityType)
        {
            case ActivityType.Move:
                Move(Stats.GetSpeed(), TargetPosition);
                break;
                /*
            case ActivityType.AutoAttack:
                if (Stats.InRange(TargetGameObject))
                {
                    CastTime++;
                    if(CastTime >= Stats.AutoAttackCastTime())
                    {
                        CastTime = 0;
                        CastTargetAbility(AutoAttack, TargetGameObject);
                    }
                }
                else Move(Stats.GetSpeed(), TargetGameObject.transform.position);
                break;*/
            case ActivityType.TargetAbility: 
                if(Stats.InRange(TargetGameObject, TargetAbilityRange))
                {
                    CastTime++;
                    if(CastTime >= AbilityCastTime)
                    {
                        CastTime = 0;
                        //currentActivityType = ActivityType.None;
                        CastTargetAbility(Ability, TargetGameObject);
                    }
                }
                else Move(Stats.GetSpeed(), TargetGameObject.transform.position);
                break;
            case ActivityType.SkillShotAbility:
                CastTime++;
                if (CastTime >= AbilityCastTime)
                {
                    CastTime = 0;
                    currentActivityType = ActivityType.None;
                    CastSkillShot(Ability, TargetPosition);
                }
                break;
        }
    }

    void Move(float speed, Vector3 position)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, position, step);
        CastTime = 0;
        if (Vector3.Distance(transform.position, position) < 0.1)
        {
            currentActivityType = ActivityType.None;
        }
    }
    
    void CastSkillShot(GameObject ability, Vector3 targetDirection)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(targetDirection, transform.position, gameObject);
    }

    void CastTargetAbility(GameObject ability, GameObject target)
    {
        GameObject newObject = Instantiate(ability) as GameObject;
        var proj = newObject.GetComponent<Ability>();
        proj.Prepare(target, transform.position, gameObject);
    }

    void Hit(Effect effect)
    {
        print(gameObject.tag + " is hit!!!");
    }
}
