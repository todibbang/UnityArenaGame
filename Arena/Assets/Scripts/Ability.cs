using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

    public Effect.EffectType EffectType;
    public float Modifier;


    public int CastTime;
    public float TravelDistance;
    public BodyController.ActivityType ActivityType;
    public int Range;

    public Reaction AfterCastReaction;
    public Reaction CollisionReaction;
    public Reaction DistanceTraveledReaction;
    public float Speed;

    GameObject TargetGameObject;
    Vector3 StartPosition;
    Vector3 TargetPosition;
    GameObject Sender;

    public enum Reaction
    {
        Destroy, Effect, Recast
    }


    public void UseAbility(RaycastHit hit, GameObject sender)
    {
        sender.GetComponent<BodyController>().NewActivity(ActivityType, gameObject, CastTime, Range, hit.collider.gameObject, new Vector3(hit.point.x, transform.position.y, hit.point.z));
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
        TargetPosition = new Vector3(start.x + (x * 10), 1.5f, start.z + (z * 10));
        //AbilityReaction(AfterCastReaction);
    }



    // Use this for initialization
    void Start () {
        transform.position = StartPosition;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position = new Vector3();
        if (TargetGameObject != null) position = TargetGameObject.transform.position;
        else if (TargetPosition != null) position = TargetPosition;

        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, position, step);

        if (ActivityType == BodyController.ActivityType.SkillShotAbility && Vector3.Distance(transform.position, StartPosition) > TravelDistance)
        {
            AbilityReaction(DistanceTraveledReaction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        print(Sender.tag);
        if (other.tag != Sender.tag)
        {
            //print(other.tag);
            //print(TargetGameObject.tag);
            if (ActivityType == BodyController.ActivityType.TargetAbility && other.tag == TargetGameObject.tag || ActivityType == BodyController.ActivityType.SkillShotAbility)
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
        }
    }
}
