using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public GameObject[] SpawnLocations;
    int playersJoined;

    public void JoinMap(GameObject player)
    {
        player.transform.position = SpawnLocations[playersJoined++].transform.position;
    }

}
