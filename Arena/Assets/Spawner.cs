using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{

    public GameObject[] Abilities;

    [Command]
	public void CmdSpawn(string name, GameObject target, Vector3 position, GameObject sender)
    {
        int i = Abilities.ToList().FindIndex(a => a.name == name);

        GameObject newAbility = Instantiate(Abilities[i]);

        NetworkServer.Spawn(newAbility);



        var abil = newAbility.GetComponent<Ability>();
        if (target != null) abil.Prepare(target, sender.transform.position, sender);
        else abil.Prepare(position, sender.transform.position, sender);
        abil.IgnoreCaster();

        /*
        var StartPosition = sender.transform.position;
        var TargetPosition = new Vector3(position.x * 10000 + StartPosition.x, newAbility.transform.position.y, position.z * 10000 + StartPosition.z);

        newAbility.transform.position = sender.transform.position;
        //var TargetPosition = new Vector3(position.x * 10000 + sender.transform.position.x, newAbility.transform.position.y, position.z * 10000 + sender.transform.position.z);

        var v2 = new Vector2(position.x * 10000 + sender.transform.position.x, position.z * 10000 + sender.transform.position.z);
        var v1 = new Vector2(sender.transform.position.x, sender.transform.position.z);
        Vector2 diference = v2 - v1;
        float sign = (v2.y > v1.y) ? -1.0f : 1.0f;
        var Angel = Vector2.Angle(Vector2.right, diference) * sign;
        newAbility.transform.Rotate(new Vector3(0, Angel + 90, 0));
        newAbility.gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 6; */
    }
}
