using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelector : MonoBehaviour {

    public GameObject Map;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        Map.GetComponent<MapController>().JoinMap(other.gameObject);
    }
}
