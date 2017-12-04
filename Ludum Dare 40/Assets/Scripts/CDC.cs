using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDC : MonoBehaviour {

    public AudioClip CDCClip;
    private AudioSource audioSource;
    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = CDCClip;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        audioSource.Play();
    }
}
