using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour {

	public Effect Effect;

    public int CastTime;
    public int ExecutionTimes = 1;
    //public float LifeValue;
    //public AbilityType ActivityType;
    //public int CastRange;
    //public float Speed;
    public bool CanMove;

	public InterractsWith Interraction;

    //public Reaction AfterCastReaction;
    public Reaction CollisionReaction;
	public Reaction LifeExpired = Reaction.Destroy;
    public GameObject SecondaryAbility;
    public GameObject AbilityCaster;

	[HideInInspector]
    public GameObject TargetGameObject;
	[HideInInspector]
    public Vector3 StartPosition;
	[HideInInspector]
    public Vector3 TargetPosition;
	[HideInInspector]
    public GameObject Sender;
    int LivedTime;

    public enum Reaction { None, Destroy, SecondaryAbility, Recast, EffectOnSender }
    public enum AbilityType { TargetAbility, SkillShotAbility, Aoe}
	public enum InterractsWith { Enemy, Friendly, Self, FriendlyAndSelf }
		
	public abstract void UseAbility (GameObject sender);

	public abstract void Prepare (GameObject target, Vector3 start, GameObject sender);

	public abstract void Prepare (Vector3 clickPosition, Vector3 start, GameObject sender);
	/*
    public abstract void UseAbility(GameObject sender)
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
		abilityCaster.NewActivity(ActivityType, CastTime, ExecutionTimes, CastRange, CanMove);
    }

    public abstract void Prepare(GameObject target, Vector3 start, GameObject sender)
    {
        Sender = sender;
        TargetGameObject = target;
        StartPosition = start;
    }

	public abstract void Prepare(Vector3 clickPosition, Vector3 start, GameObject sender)
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
        if(ActivityType == AbilityType.Aoe) {
            LivedTime++;
			if(LivedTime >= LifeValue) AbilityReaction(LifeExpired);
        } 
		else
        {
            Vector3 position = new Vector3();
            if (TargetGameObject != null) position = TargetGameObject.transform.position;
			else position = TargetPosition;

            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, position, step);

			if (ActivityType == AbilityType.SkillShotAbility && Vector3.Distance(transform.position, StartPosition) > LifeValue)
				AbilityReaction(LifeExpired);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
		print (other.tag);

		if (other.tag == "Caster" || other.tag == "Ability" || other.tag == "Default" || other.tag == "Ground" || other.tag == "Untagged")
			return;

		print (other.tag + " - Still??");

		switch (Interraction) 
		{
			case InterractsWith.Enemy:
				if (other.GetComponent<BodyController> ().TeamID == Sender.GetComponent<BodyController> ().TeamID)
					return;
				other.gameObject.SendMessage ("Hit", new Effect(Effect, transform.position));
				break;
			case InterractsWith.Friendly:
				if (other.GetComponent<BodyController> ().TeamID != Sender.GetComponent<BodyController> ().TeamID)
					return;
				other.gameObject.SendMessage ("Hit", new Effect(Effect, transform.position));
				break;
		}
		AbilityReaction(CollisionReaction);
    }

    public void AbilityReaction(Reaction reaction)
    {
        switch (reaction)
        {
            case Reaction.Destroy:
                Destroy(gameObject);
                break;
		case Reaction.SecondaryAbility:
				print ("casting secondary ability");
				GameObject newObject = Instantiate(SecondaryAbility) as GameObject;
                var proj = newObject.GetComponent<Ability>();
                proj.Prepare(transform.position, transform.position, Sender);
                Destroy(gameObject);
                break;
			case Reaction.EffectOnSender:
				Sender.SendMessage ("Hit", new Effect(Effect, transform.position));
				Destroy(gameObject);
				break;
        }
    }
}
