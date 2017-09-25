using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    
    /*
	public void NewActivity(ActivityType activityType, GameObject ability, float castTime, float range, bool move, GameObject target, Vector3 position)
    {
        currentActivityType = activityType;
        Ability = ability;
        AbilityCastTime = castTime;
        TargetAbilityRange = range;
        CanMove = move;
        TargetGameObject = target;
        TargetPosition = position;
        //if (!CanMove) Moving = false;
        Moving = false;
    }

    public enum MovementActivity { None, Move}

    public enum ActivityType{ None, TargetAbility, SkillShotAbility, TargetChannel, SkillShotChannel } */

    public Stats Stats;
    public GameObject AutoAttack;
    public GameObject FirstAbility;
    public GameObject SecondAbility;
    public GameObject ThirdAbility;
    public GameObject ForthAbility;
    
    /*
    GameObject Ability;
    float CastTime;

    ActivityType currentActivityType;
    GameObject TargetGameObject;
    Vector3 TargetPosition;

    float TargetAbilityRange;

    float AbilityCastTime;
    bool CanMove; */

    Ray ray;
    RaycastHit hit;

    bool Moving;
    Vector3 MoveToPosition;

    void Update()
    {
        if (gameObject.tag == "Player")
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.collider.tag == "Enemy") AutoAttack.GetComponent<Ability>().UseAbility(hit, gameObject);
                    else if (hit.collider.tag == "Ground")  //NewActivity(ActivityType.Move, null, 0, 0, false, null, );
                    {
                        Moving = true;
                        MoveToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Q)) FirstAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.W)) SecondAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.E)) ThirdAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
                if (Input.GetKeyDown(KeyCode.R)) ForthAbility.GetComponent<Ability>().UseAbility(hit, gameObject);
            }
        }
        
        if(Moving)
        {
            Move(MoveToPosition);
        }
    }

    void Move(Vector3 position)
    {
        float step = Stats.GetSpeed() * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, position, step);
        if (Vector3.Distance(transform.position, position) < 0.1)
            Moving = false;
    }
    
    void StopMoving()
    {
        Moving = false;
    }

    void Hit(Effect effect)
    {
        print(gameObject.tag + " is hit!!!");
    }
}
