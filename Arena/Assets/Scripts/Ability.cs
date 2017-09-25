using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

    public Effect.EffectType EffectType;
    public float Modifier;

    public int CastTime;
    public int CastTimes = 1;
    public float TravelDistance;
    public AbilityType ActivityType;
    public int Range;
    public float Speed;
    public int LiveTime;
    public bool CanMove;

    //public Reaction AfterCastReaction;
    public Reaction CollisionReaction;
    public Reaction DistanceTraveledReaction;
    public GameObject Effect;
    public GameObject AbilityCaster;

    GameObject TargetGameObject;
    Vector3 StartPosition;
    Vector3 TargetPosition;
    GameObject Sender;
    int LivedTime;

    public enum Reaction
    {
        None, Destroy, Effect, Recast
    }

    public enum AbilityType { TargetAbility, SkillShotAbility, Aoe}


    public void UseAbility(GameObject sender)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        string layer = "Ground";
        if (ActivityType == AbilityType.TargetAbility) layer = "Body";
        if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer(layer))))
        {
            
        }
        if (hit.collider == null) return;

        if (ActivityType == AbilityType.TargetAbility && hit.collider.tag == "Ground")
			return;


        GameObject caster = Instantiate(AbilityCaster, sender.transform) as GameObject;
        var abilityCaster = caster.GetComponent<AbilityCaster>();
        abilityCaster.StartCasting(gameObject, hit, sender);
        abilityCaster.NewActivity(ActivityType, CastTime, CastTimes, Range, CanMove);
    }

    public void Prepare(GameObject target, Vector3 start, GameObject sender)
    {
        Sender = sender;
        TargetGameObject = target;
        StartPosition = start;
    }

    public void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender)
    {
        Sender = sender;
        var x = clickPosition.x - start.x;
        var z = clickPosition.z - start.z;
        StartPosition = start;
        if (ActivityType == AbilityType.Aoe) TargetPosition = new Vector3(clickPosition.x, transform.position.y, clickPosition.z);
        else TargetPosition = new Vector3(start.x + (x * 1000), transform.position.y, start.z + (z * 1000));
    }

    // Use this for initialization
    void Start () {
        if (ActivityType == AbilityType.Aoe) transform.position = TargetPosition;
        else transform.position = StartPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if(ActivityType == AbilityType.Aoe)
        {
            LivedTime++;
            if(LivedTime >= LiveTime) AbilityReaction(DistanceTraveledReaction);
        } else
        {
            Vector3 position = new Vector3();
            if (TargetGameObject != null) position = TargetGameObject.transform.position;
            else if (TargetPosition != null) position = TargetPosition;

            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, position, step);

            if (ActivityType == AbilityType.SkillShotAbility && Vector3.Distance(transform.position, StartPosition) > TravelDistance)
            {
                AbilityReaction(DistanceTraveledReaction);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        print(Sender.tag);
		if (other.tag != Sender.tag && other.tag != "Caster" && other.tag != "Ability")
        {
            //print(other.tag);
            //print(TargetGameObject.tag);
            if (ActivityType == AbilityType.TargetAbility && other.tag == TargetGameObject.tag || ActivityType == AbilityType.SkillShotAbility)
            {
                other.gameObject.SendMessage("Hit", new Effect());
            }
            AbilityReaction(CollisionReaction);
        }
        
    }

    void AbilityReaction(Reaction reaction)
    {
        switch (reaction)
        {
            case Reaction.Destroy:
                Destroy(gameObject);
                break;
            case Reaction.Effect:
                GameObject newObject = Instantiate(Effect) as GameObject;
                var proj = newObject.GetComponent<Ability>();
                proj.Prepare(transform.position, transform.position, Sender);
                Destroy(gameObject);
                break;
        }
    }
}
