using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {

    [SerializeField] GameObject ObjectToFollow;

    private Vector3 newPosition;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        newPosition = new Vector3(ObjectToFollow.transform.position.x, transform.position.y, ObjectToFollow.transform.position.z);
        transform.position = newPosition;
    }
}
