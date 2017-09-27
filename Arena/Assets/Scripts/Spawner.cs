using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
	/*
    public GameObject[] Abilities;

	GameObject NewAbility(string name, out Ability abil) {
		int i = Abilities.ToList().FindIndex(a => a.name == name);
		GameObject newAbility = Instantiate(Abilities[i]);
		abil = newAbility.GetComponent<Ability>();
		return newAbility;
	}

	[Command]
	public void CmdSpawnSkillShot(string name, Vector3 position, GameObject sender)
	{
		print("Spawner casting : " + name);

		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(position, sender);
		abil.IgnoreCaster();

		var StartPosition = sender.transform.position;
		newAbility.transform.position = StartPosition;
		var v2 = new Vector2(position.x * 10000 + StartPosition.x, position.z * 10000 + StartPosition.z);
		var v1 = new Vector2(StartPosition.x, StartPosition.z);
		Vector2 diference = v2 - v1;
		float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
		var Angel = Vector2.Angle(Vector2.right, diference) * sign;
		newAbility.transform.Rotate(new Vector3(0, Angel + 90, 0));
		newAbility.GetComponent<Rigidbody>().velocity = newAbility.transform.forward * 6;

		NetworkServer.Spawn(newAbility);
	}

	[Command]
	public void CmdSpawnAoe(string name, Vector3 position, GameObject sender)
	{
		print("Spawner casting : " + name);

		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(position, sender);
		abil.IgnoreCaster();


		var TargetPosition = new Vector3(position.x, newAbility.transform.position.y, position.z);

		newAbility.transform.position = TargetPosition;
		var v2 = new Vector2(TargetPosition.x, TargetPosition.z);
		var v1 = new Vector2(sender.transform.position.x, sender.transform.position.z);
		Vector2 diference = v2 - v1;
		float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
		var Angel = Vector2.Angle(Vector2.right, diference) * sign;
		newAbility.transform.Rotate(new Vector3(0, Angel - 90, 0));  

		NetworkServer.Spawn(newAbility);
	}

	[Command]
	public void CmdSpawnTarget(string name, GameObject target, GameObject sender)
	{
		print("Spawner casting : " + name);

		Ability abil;
		GameObject newAbility = NewAbility(name, out abil);
		abil.Prepare(target, sender);
		abil.IgnoreCaster();
		newAbility.transform.position = sender.transform.position;

		NetworkServer.Spawn(newAbility);
	}
	*/
}
