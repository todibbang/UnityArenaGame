using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	public List<GameObject> ActiveCasters;
    public List<string> ActiveCasterNames;

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
                    if (hit.collider.tag == "Enemy") AutoAttack.GetComponent<Ability>().UseAbility(gameObject);
                    else if (hit.collider.tag == "Ground")  //NewActivity(ActivityType.Move, null, 0, 0, false, null, );
                    {
						//gameObject.SendMessage ("OverruleMovement");
                        Moving = true;
                        MoveToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Q)) FirstAbility.GetComponent<Ability>().UseAbility(gameObject);
            if (Input.GetKeyDown(KeyCode.W)) SecondAbility.GetComponent<Ability>().UseAbility(gameObject);
            if (Input.GetKeyDown(KeyCode.E)) ThirdAbility.GetComponent<Ability>().UseAbility(gameObject);
            if (Input.GetKeyDown(KeyCode.R)) ForthAbility.GetComponent<Ability>().UseAbility(gameObject);
        }

        //if (GlobalCooldown > 0) GlobalCooldown--;


        if (Moving)
        {
            Move(MoveToPosition);
        }
    }

    void Move(Vector3 position)
    {
        float step = Stats.GetSpeed() * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(position.x, transform.position.y, position.z), step);
        if (Vector3.Distance(transform.position, position) < 0.1)
            Moving = false;
    }
		
	void AttemptToContinue(GameObject caster) {
		var abilityCaster = caster.GetComponent<AbilityCaster> ();
		if (Moving) {
			abilityCaster.OverruleMovement();
		}

		print (ActiveCasters.Count + " index: " + ActiveCasters.IndexOf (caster));
		if (ActiveCasters.Count > 1 && ActiveCasters.IndexOf(caster) != ActiveCasters.Count-1) {
			caster.SendMessage("Stop");
		}
	}

    void Hit(Effect effect)
    {
        print(gameObject.tag + " is hit!!!");
    }

	public void AddCaster(GameObject caster, GameObject ability) {
		Moving = false;
        if (ActiveCasterNames.Any(c => c == ability.name)) Destroy(caster);
        else
        {
            ActiveCasters.Add(caster);
            ActiveCasterNames.Add(ability.name);
        }
    }

	void RemoveCaster(GameObject caster) {
        print("index of: " + ActiveCasters.IndexOf(caster));
        ActiveCasterNames.RemoveAt(ActiveCasters.IndexOf(caster));
        ActiveCasters.Remove (caster);
		Destroy(caster);
	}

	public bool FirstInqueue(GameObject caster) {
		return ActiveCasters.IndexOf (caster) == 0;
	}
}
