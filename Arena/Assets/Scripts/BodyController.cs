using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BodyController : MonoBehaviour {
	
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

	public List<GameObject> ActiveCasters;
    public List<string> ActiveCasterNames;
	public int JumpSpeed;
	Ray ray;
	RaycastHit hit;

    bool Moving;
    Vector3 MoveToPosition;

	List<Effect> Effects = new List<Effect>();

    void Update()
    {
        if (gameObject.tag == "Player")
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
						AutoAttack.GetComponent<Ability> ().UseAbility (gameObject);
						return;
					}
					/*
					else if (hit.collider != null && hit.collider.tag == "Ground")  //NewActivity(ActivityType.Move, null, 0, 0, false, null, );
					{
						//print ("OR DAT??");
						//gameObject.SendMessage ("OverruleMovement");
						Moving = true;
						MoveToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
					} */
				}
			}
            
			if (Input.GetKeyDown(KeyCode.Alpha1)) FirstAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha2)) SecondAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha3)) ThirdAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha4)) ForthAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha5)) FifthAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha6)) SixthAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha7)) SeventhAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha8)) EighthAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha9)) NinthAbility.GetComponent<Ability>().UseAbility(gameObject);
			if (Input.GetKeyDown(KeyCode.Alpha0)) TenthAbility.GetComponent<Ability>().UseAbility(gameObject);

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

        //if (GlobalCooldown > 0) GlobalCooldown--;


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

		//print (ActiveCasters.Count + " index: " + ActiveCasters.IndexOf (caster));
		if (ActiveCasters.Count > 1 && ActiveCasters.IndexOf(caster) != ActiveCasters.Count-1) {
			caster.SendMessage("Stop");
		}
	}

    void Hit(Effect effect)
    {
		print(gameObject.tag + " is hit!!!");
		Effects.Add (effect);
		//effect.SetPosition (effect.transform.position);
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
        //print("index of: " + ActiveCasters.IndexOf(caster));
        ActiveCasterNames.RemoveAt(ActiveCasters.IndexOf(caster));
        ActiveCasters.Remove (caster);
		Destroy(caster);
	}

	public bool FirstInqueue(GameObject caster) {
		return ActiveCasters.IndexOf (caster) == 0;
	}
}
