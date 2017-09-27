using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.Networking;
using System;

public class BodyController : NetworkBehaviour
{

    public int ID;
	public int TeamID;
    public Stats Stats;
    public GameObject AutoAttack;
    public GameObject FirstAbility;
    public GameObject SecondAbility;
    public GameObject ThirdAbility;
    public GameObject ForthAbility;
	public GameObject FifthAbility;
	public GameObject SixthAbility;
	public GameObject SeventhAbility;
	public GameObject EighthAbility;
	public GameObject NinthAbility;
	public GameObject TenthAbility;

    public Camera Camera;
    public AbilityCaster Caster;
	public List<GameObject> ActiveCasters;
	public int JumpSpeed;
	Ray ray;
	RaycastHit hit;

    bool Moving;
    Vector3 MoveToPosition;

	List<Effect> Effects = new List<Effect>();

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        Camera.enabled = true;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (ID == 1)
        {
			var ControlLost = false;
			List<Effect> EffectsToLose = new List<Effect>();
			foreach (var effect in Effects) {
				if (effect.OverrulesControles)
					ControlLost = true;

				switch (effect.Effecttype) {
				case Effect.EffectType.MoveTo:
					float step = 30 * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, effect.GetPosition (), step);
					if (Vector3.Distance (transform.position, effect.GetPosition ()) < 0.1)
						effect.SetInactive();
					break;
				case Effect.EffectType.BlinkTo:
					transform.position = effect.GetPosition();
					effect.SetInactive();
					break;
				}

				effect.FrameLived ();
				if (!effect.Active ()) {
					EffectsToLose.Add (effect);
				}
			}
			foreach (var effect in EffectsToLose) {
				Effects.Remove (effect);
				Destroy (effect);
			}


			if (ControlLost)
				return;


			if (Input.GetMouseButtonDown (0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer ("Ground"))))
				{
					if (hit.collider != null && hit.collider.tag == "Ground")  //NewActivity(ActivityType.Move, null, 0, 0, false, null, );
					{
						OverruleMovement ();
						Moving = true;
						MoveToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
					} 
				}
			}

			if (Input.GetMouseButtonDown (1)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				LayerMask layerMask = (1 << LayerMask.NameToLayer ("Body"));


				if (Physics.Raycast(ray, out hit, 10000, layerMask))
				{
					if (hit.collider != null && hit.collider.tag == "Enemy") {
						//print ("THIS??");
						UseAbility (0, AutoAttack);
						return;
					}
				}
			}
            
			if (Input.GetKeyDown(KeyCode.Alpha1)) UseAbility(1, FirstAbility);
			if (Input.GetKeyDown(KeyCode.Alpha2)) UseAbility(2, SecondAbility);
			if (Input.GetKeyDown(KeyCode.Alpha3)) UseAbility(3, ThirdAbility);
			if (Input.GetKeyDown(KeyCode.Alpha4)) UseAbility(4, ForthAbility);
			if (Input.GetKeyDown(KeyCode.Alpha5)) UseAbility(5, FifthAbility);
			if (Input.GetKeyDown(KeyCode.Alpha6)) UseAbility(6, SixthAbility);
			if (Input.GetKeyDown(KeyCode.Alpha7)) UseAbility(7, SeventhAbility);
			if (Input.GetKeyDown(KeyCode.Alpha8)) UseAbility(8, EighthAbility);
			if (Input.GetKeyDown(KeyCode.Alpha9)) UseAbility(9, NinthAbility);
			if (Input.GetKeyDown(KeyCode.Alpha0)) UseAbility(10, TenthAbility);

			if (Input.GetKey (KeyCode.W))
				PlayerMove (transform.position + (transform.rotation * Vector3.forward));
			if (Input.GetKey (KeyCode.A))
				PlayerMove (transform.position + (transform.rotation * Vector3.left));
			if (Input.GetKey (KeyCode.D))
				PlayerMove (transform.position + (transform.rotation * Vector3.right));
			if (Input.GetKey (KeyCode.S))
				PlayerMove (transform.position + (transform.rotation * Vector3.back));

			if (Input.GetMouseButton (1)) {
				var y = Input.GetAxis ("Mouse X");
				var rotateValue = new Vector3 (0, y * -3, 0);
				transform.eulerAngles = transform.eulerAngles - rotateValue;
			}

			if (Input.GetKey (KeyCode.Q)) {
				transform.eulerAngles = transform.eulerAngles - new Vector3(0,2,0);
			}
			if (Input.GetKey (KeyCode.E)) {
				transform.eulerAngles = transform.eulerAngles - new Vector3(0,-2,0);
			}

			if (Input.GetKey(KeyCode.Space)) {
				var rb = gameObject.GetComponent<Rigidbody>();
				rb.AddForce(new Vector3(0,JumpSpeed,0));
			}
        }

        if (Moving)
        {
			Move(MoveToPosition);
        }
    }

	void PlayerMove(Vector3 position) {
		OverruleMovement ();
		Moving = false;
		Move (position);
	}

	void OverruleMovement()
	{
		foreach (GameObject g in ActiveCasters)
			g.SendMessage ("OverruleMovement");
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
	}

    void Hit(Effect effect)
    {
        print("Body hit with " + effect.Effecttype);
		Effects.Add (effect);
    }

	public void AddCaster(GameObject caster, GameObject ability) {
		Moving = false;
		if (ActiveCasters.Count > 0)
			RemoveCaster (ActiveCasters [0]);
		ActiveCasters.Add(caster);
    }

	void RemoveCaster(GameObject caster) {
        ActiveCasters.Remove (caster);
		Destroy(caster);
	}

    [Command]
    public void CmdCast(int abilityNumber, GameObject targetGameObject, Vector3 targetDirection, bool ignoreCaster, Ability.AbilityType abilityType)
    {
        GameObject.Find("Spawner").GetComponent<Spawner>().CmdSpawn("FireBall", targetGameObject, targetDirection, gameObject);

        /*
        GameObject newAbility = null;
        if (abilityNumber == 1) newAbility = Instantiate(FirstAbility) as GameObject;
        if (abilityNumber == 2) newAbility = Instantiate(SecondAbility) as GameObject;
        if (abilityNumber == 3) newAbility = Instantiate(ThirdAbility) as GameObject;
        if (abilityNumber == 4) newAbility = Instantiate(ForthAbility) as GameObject;


        NetworkServer.Spawn(newAbility);
        var abil = newAbility.GetComponent<Ability>();
        if (targetGameObject != null) abil.Prepare(targetGameObject, transform.position, gameObject);
        else abil.Prepare(targetDirection, transform.position, gameObject);
        if (ignoreCaster) abil.IgnoreCaster(); */







        /*
        newAbility.transform.position = targetDirection;
        var v2 = new Vector2(targetDirection.x, targetDirection.z);
        var v1 = new Vector2(transform.position.x, transform.position.z);
        Vector2 diference = v2 - v1;
        float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
        var Angel = Vector2.Angle(Vector2.right, diference) * sign;
        newAbility.transform.Rotate(new Vector3(0, Angel - 90, 0)); */
        //if(abilityType == Ability.AbilityType.SkillShotAbility) gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 6;
    }

    public void UseAbility(int i, GameObject ability)
    {
        var t = ability.GetComponent<TargetAbility>();
        var a = ability.GetComponent<AoeAbility>();
        var s = ability.GetComponent<SkillShotAbility>();
        if (t != null) UseTargetAbility(i, ability);
        if (a != null) UseAoeAbility(i, ability);
        if (s != null) UseSkillShotAbility(i, ability);
    }

    public void UseAoeAbility(int i, GameObject ability)
    {
        RaycastHit hit = GetRayhit("Ground");
        if (hit.collider == null) return;
        var targetAbility = ability.GetComponent<AoeAbility>();
        Caster.StartCasting(i, ability, targetAbility.CastRange == 0 ? transform.position : new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
        Caster.NewActivity(Ability.AbilityType.Aoe, targetAbility.CastTime, targetAbility.ExecutionTimes, targetAbility.CastRange, targetAbility.CanMove);
    }


    public void UseTargetAbility(int i, GameObject ability)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer("Body"))))
        {
            if (hit.collider == null) return;

            int hitID = hit.collider.gameObject.GetComponent<BodyController>().ID, hitTeam = hit.collider.gameObject.GetComponent<BodyController>().TeamID;
            int senderID = gameObject.GetComponent<BodyController>().ID, senderTeam = gameObject.GetComponent<BodyController>().TeamID;

            var targetAbility = ability.GetComponent<TargetAbility>();

            var IgnoreCaster = false;
            switch (targetAbility.Interraction)
            {
                case Ability.InterractsWith.Enemy:
                    if (hitTeam == senderTeam) return;
                    break;
                case Ability.InterractsWith.Friendly:
                    if (hitTeam != senderTeam || hitID == senderID) return;
                    break;
                case Ability.InterractsWith.Self:
                    if (hitID != senderID) return;
                    break;
                case Ability.InterractsWith.FriendlyAndSelf:
                    if (hitTeam != senderTeam) return;
                    if (hitID != senderID) IgnoreCaster = true;
                    break;
            }

            //GameObject caster = Instantiate(GameObject.Find("Caster"), sender.transform) as GameObject;
            Caster.StartCasting(i, ability, new Vector3(), hit.collider.gameObject);
            Caster.NewActivity(Ability.AbilityType.TargetAbility, targetAbility.CastTime, targetAbility.ExecutionTimes, targetAbility.CastRange, targetAbility.CanMove);
            if (IgnoreCaster) Caster.IgnoreCaster();
        }
    }

    public void UseSkillShotAbility(int i, GameObject ability)
    {
        RaycastHit hit = GetRayhit("Ground");
        if (hit.collider == null) return;
        print(hit.point);
        var targetAbility = ability.GetComponent<SkillShotAbility>();
        Caster.StartCasting(i, ability, new Vector3(hit.point.x, transform.position.y, hit.point.z), null);
        Caster.NewActivity(Ability.AbilityType.SkillShotAbility, targetAbility.CastTime, targetAbility.ExecutionTimes, 0, targetAbility.CanMove);
    }

    RaycastHit GetRayhit(string layer)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer(layer))))
        {
            print("returning hit");
            return hit;
        }
        return hit;
    }
}
