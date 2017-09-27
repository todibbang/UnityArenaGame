using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Stats Stats;
    public Text Text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var hp = Stats.GetHealth();
        Text.text = hp[1] + "/" + hp[0];
	}
}
