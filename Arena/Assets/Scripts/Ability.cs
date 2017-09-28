using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public abstract class Ability : NetworkBehaviour {

	public Effect Effect;

    public int CastTime;
    public int ExecutionTimes = 1;
    public bool CanMove;

	public InterractsWith Interraction;

    public Reaction CollisionReaction;
	public Reaction LifeExpired = Reaction.Destroy;
    public GameObject SecondaryAbility;

	[HideInInspector]
    public GameObject TargetGameObject;
	[HideInInspector]
    public Vector3 StartPosition;
	[HideInInspector]
    public Vector3 TargetPosition;
	[HideInInspector]
    public GameObject Sender;

    bool ignoreCaster;

    public enum Reaction { None, Destroy, SecondaryAbility, Recast, EffectOnSender }
    public enum AbilityType { TargetAbility, SkillShotAbility, Aoe}
	public enum InterractsWith { Enemy, Friendly, Self, FriendlyAndSelf, Terrain, Noone }

	public abstract void Prepare (GameObject target, GameObject sender);

	public abstract void Prepare (Vector3 clickPosition, GameObject sender);

	public abstract AbilityType GetType ();

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

		//print (other.tag);

		if (other.tag == "Ability" || other.tag == "Ground" || other.tag == "Untagged")
			return;

		print (other.tag + " - Still?? " + ignoreCaster);



        int hitID = other.gameObject.GetComponent<BodyController>().ID, hitTeam = other.gameObject.GetComponent<BodyController>().TeamID;
        int senderID = Sender.gameObject.GetComponent<BodyController>().ID, senderTeam = Sender.gameObject.GetComponent<BodyController>().TeamID;

        switch (Interraction)
        {
            case InterractsWith.Enemy:
                if (hitTeam == senderTeam) return;
                break;
            case InterractsWith.Friendly:
                if (hitTeam != senderTeam || hitID == senderID) return;
                break;
            case InterractsWith.FriendlyAndSelf:
                if (hitTeam != senderTeam || hitID == senderID && ignoreCaster) return;
                break;
            case InterractsWith.Self:
                if (hitID != senderID) return;
                break;
        }
        //other.gameObject.SendMessage("Hit", new Effect(Effect, transform.position));
        print("sending effect");
        other.gameObject.GetComponent<Stats>().Hit(new Effect(Effect, transform.position));


        AbilityReaction(CollisionReaction);
    }

    public void IgnoreCaster()
    {
        ignoreCaster = true;
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
				//Sender.GetComponent<AbilityCaster> ().SpawnAbility(
			Sender.GetComponent<AbilityCaster> ().SpawnAbility (SecondaryAbility, null, transform.position);
                Destroy(gameObject);
                break;
			case Reaction.EffectOnSender:
				Sender.SendMessage ("Hit", new Effect(Effect, transform.position));
				Destroy(gameObject);
				break;
        }
    }
}
