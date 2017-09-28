using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelector : MonoBehaviour {

	public Stats Stats;
	public GameObject[] Abilities;



	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ground")
			return; 
		other.gameObject.GetComponent<AbilityCaster> ().Abilities = Abilities;
		other.gameObject.GetComponent<Stats> ().SetStats(Stats);
	}
}
