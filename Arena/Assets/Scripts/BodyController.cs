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

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        Camera.enabled = true;
        GameObject.Find("LoadCamera").SetActive(false);
    }

	void Start() {
		ID = UnityEngine.Random.Range(0, 100000);
		TeamID = UnityEngine.Random.Range(0, 100000);
	}

    void Update()
    {
        if (!isLocalPlayer) return;

        //Stats.ProcessEffects();
        
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

        if (Stats.HasOverruleBlinkTo())
        {
            transform.position = Stats.GetMoveToPosition();
            Moving = false;
            //Stats.CurrentHealth = 0;
        }
        else if(Stats.HasOverruleMoveTo())
        {
            Move(Stats.GetMoveToPosition());
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
                PlayerMove(transform.position + (transform.rotation * Vector3.forward));
            if (Input.GetKey(KeyCode.A))
                PlayerMove(transform.position + (transform.rotation * Vector3.left));
            if (Input.GetKey(KeyCode.D))
                PlayerMove(transform.position + (transform.rotation * Vector3.right));
            if (Input.GetKey(KeyCode.S))
                PlayerMove(transform.position + (transform.rotation * Vector3.back));
            if (Input.GetKey(KeyCode.Q))
                transform.eulerAngles = transform.eulerAngles - new Vector3(0, 2, 0);
            if (Input.GetKey(KeyCode.E))
                transform.eulerAngles = transform.eulerAngles - new Vector3(0, -2, 0);
            if (Input.GetMouseButton(1))
                transform.eulerAngles = transform.eulerAngles - new Vector3(0, Input.GetAxis("Mouse X") * -3, 0);
            if (Input.GetKey(KeyCode.Space))
                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpSpeed, 0));
        }
        
        if (Moving)
			Move(MoveToPosition);


        print(Stats.GetSpeed());
        //Stats.ResetTemporaryValues();
    }

	void PlayerMove(Vector3 position) {
		Caster.OverruleMovement();
		Moving = false;
        Move (position);
	}

    void Move(Vector3 position)
    {
        

        float step = Stats.GetSpeed() * Time.deltaTime;

        print("Moving " + Stats.GetSpeed() +" - - "+ Time.deltaTime);

        //float step = 4 * Time.deltaTime;
        print("before " + transform.position);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(position.x, transform.position.y, position.z), step);
        print("after " + transform.position);
        if (Vector3.Distance(transform.position, position) < 0.1)
            Moving = false;
    }
		
	void AttemptToContinue(GameObject caster) {
		if (Moving) {
			Caster.OverruleMovement();
		}
	}
}
