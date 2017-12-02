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
	Transform targetTransform = null;
	Citizen.CitizenType citizenType;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        destination = DirectionalNavSphere(transform.position, UnityEngine.Random.insideUnitSphere, RandomPathSearchDistance, -1);
        walking = false;
        waitCurrent = 0;//Random.Range(waitMin, waitMax);
        movementSpeed = 0;
		citizenType = GetComponent<Citizen>().Type;
		if (citizenType == Citizen.CitizenType.Attacker)
		{
			targetTransform = GameManager.VIP.transform;
		}
    }
	
	// Update is called once per frame
	void Update () {
        movementSpeed = nav.velocity.magnitude;
        animator.SetFloat("Speed", movementSpeed);
        if (movementSpeed >= 0.1)
        {
            walking = true;
        } else if (movementSpeed < 0.1 && walking == true && citizenType != Citizen.CitizenType.Attacker)
        {
            walking = false;
            waitCurrent = Random.Range(waitMin, waitMax);
        }
        if (walking == false && waitCurrent > 0)
        {
            waitCurrent -= Time.deltaTime;
        } else if ((walking == false && waitCurrent <= 0) || citizenType == Citizen.CitizenType.Attacker)
        {
			Vector3 direction = targetTransform != null ? (targetTransform.position - transform.position).normalized  : UnityEngine.Random.insideUnitSphere;
			destination = DirectionalNavSphere(transform.position, direction, RandomPathSearchDistance, -1);
            nav.SetDestination(destination);
            waitCurrent = 0;
            walking = true;
        }

		if (targetTransform)
		{
			if (Vector3.Distance(targetTransform.position, transform.position) < 1.0f)
			{
				OnTargetReached();
			}
		}
	}

	void OnTargetReached()
	{
		if (citizenType == Citizen.CitizenType.Attacker)
		{
			if (targetTransform.gameObject == GameManager.VIP)
			{
				targetTransform.SendMessage("OnHit", 100);
			}
		}
	}

	public static Vector3 DirectionalNavSphere(Vector3 origin, Vector3 direction, float distance, int layermask)
	{
		Vector3 walkTargetPosition = (direction * distance) + origin;
		NavMeshHit navHit;
		NavMesh.SamplePosition(walkTargetPosition, out navHit, distance, layermask);
		return navHit.position;
	}
}
