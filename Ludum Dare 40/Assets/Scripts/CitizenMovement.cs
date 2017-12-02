using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenMovement : MonoBehaviour {

    [SerializeField] private float waitMax;
    [SerializeField] private float waitMin;
    [SerializeField] private float RandomPathSearchDistance;
    Animator animator;
    Vector3 destination;
    bool walking;
    float waitCurrent;
    float movementSpeed;

    NavMeshAgent nav;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        destination = RandomNavSphere(transform.position, RandomPathSearchDistance, -1);
        walking = false;
        waitCurrent = 0;//Random.Range(waitMin, waitMax);
        movementSpeed = 0;
    }
	
	// Update is called once per frame
	void Update () {
        movementSpeed = nav.velocity.magnitude;
        animator.SetFloat("Speed", movementSpeed);
        if (movementSpeed >= 0.1)
        {
            walking = true;
        } else if (movementSpeed < 0.1 && walking == true)
        {
            walking = false;
            waitCurrent = Random.Range(waitMin, waitMax);
        }
        if (walking == false && waitCurrent > 0)
        {
            waitCurrent -= Time.deltaTime;
        } else if (walking == false && waitCurrent <= 0)
        {
            destination = RandomNavSphere(transform.position, RandomPathSearchDistance, -1);
            nav.SetDestination(destination);
            waitCurrent = 0;
            walking = true;
        }
	}

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
