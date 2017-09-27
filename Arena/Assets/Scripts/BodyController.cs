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

    public Camera Camera;
    public AbilityCaster Caster;
	public int JumpSpeed;

    bool Moving;
    Vector3 MoveToPosition;
	List<Effect> Effects = new List<Effect>();

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        Camera.enabled = true;
    }

	void Start() {
		ID = UnityEngine.Random.Range(0, 100000);
		TeamID = UnityEngine.Random.Range(0, 100000);
	}

    void Update()
    {
        if (!isLocalPlayer) return;
        
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
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 10000, (1 << LayerMask.NameToLayer ("Ground"))))
			{
				if (hit.collider != null && hit.collider.tag == "Ground")  //NewActivity(ActivityType.Move, null, 0, 0, false, null, );
				{
					Moving = true;
					MoveToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
				} 
			}
		}

		if (Input.GetKey (KeyCode.W))
			PlayerMove (transform.position + (transform.rotation * Vector3.forward));
		if (Input.GetKey (KeyCode.A))
			PlayerMove (transform.position + (transform.rotation * Vector3.left));
		if (Input.GetKey (KeyCode.D))
			PlayerMove (transform.position + (transform.rotation * Vector3.right));
		if (Input.GetKey (KeyCode.S))
			PlayerMove (transform.position + (transform.rotation * Vector3.back));
		if (Input.GetKey (KeyCode.Q)) 
			transform.eulerAngles = transform.eulerAngles - new Vector3(0,2,0);
		if (Input.GetKey (KeyCode.E)) 
			transform.eulerAngles = transform.eulerAngles - new Vector3(0,-2,0);
		if (Input.GetMouseButton (1)) 
			transform.eulerAngles = transform.eulerAngles - new Vector3 (0, Input.GetAxis ("Mouse X") * -3, 0);
		if (Input.GetKey(KeyCode.Space)) 
			gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,JumpSpeed,0));

        if (Moving)
			Move(MoveToPosition);
    }

	void PlayerMove(Vector3 position) {
		Caster.OverruleMovement();
		Moving = false;
		Move (position);
	}

    void Move(Vector3 position)
    {
        float step = Stats.GetSpeed() * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(position.x, transform.position.y, position.z), step);
        if (Vector3.Distance(transform.position, position) < 0.1)
            Moving = false;
    }
		
	void AttemptToContinue(GameObject caster) {
		if (Moving) {
			Caster.OverruleMovement();
		}
	}

    void Hit(Effect effect)
    {
        print("Body hit with " + effect.Effecttype);
		Effects.Add (effect);
    }
}
