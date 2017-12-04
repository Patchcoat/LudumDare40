﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
	[SerializeField] float health;

	bool isAlive = true;
	public bool IsAlive { get { return isAlive; } }

	CitizenMovement movement = null;
	
    public delegate void OnDeathDelegate(Citizen selfRef);
	public OnDeathDelegate OnDeath { get; set; }

    public enum CitizenType { Innocent, Attacker, VIP };
	[SerializeField] CitizenType type;
	public CitizenType Type { get { return type; } }

	void Awake()
	{
		movement = GetComponent<CitizenMovement>();
	}

	void Start ()
	{
		
	}

	void Update ()
	{
        if (Input.GetButtonDown("Fire3"))
        {
            Die();
        }
    }

    public void OnHit(float damage)
	{
		if (isAlive)
		{
			health -= damage;
			if (health <= 0.0f)
			{
				Die();
			}
		}
	}

    void Die()
    {
		isAlive = false;
		movement.alive = false;

        Component[] Rigidbodies;
        Component[] Spheres;
        Component[] Boxes;
        Component[] Capsules;

        Rigidbodies = GetComponentsInChildren<Rigidbody>();
        Spheres = GetComponentsInChildren<SphereCollider>();
        Boxes = GetComponentsInChildren<BoxCollider>();
        Capsules = GetComponentsInChildren<CapsuleCollider>();

        foreach (Rigidbody body in Rigidbodies)
        {
            body.useGravity = true;
            body.isKinematic = false;
        }
        foreach (SphereCollider collider in Spheres)
        {
            collider.isTrigger = false;
        }
        foreach (BoxCollider collider in Boxes)
        {
            collider.isTrigger = false;
        }
        foreach (CapsuleCollider collider in Capsules)
        {
            collider.isTrigger = false;
        }

        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<SphereCollider>().isTrigger = true;

		if (OnDeath != null)
		{
			OnDeath(this);
		}
	}

	public void OnSpooked(Vector3 spookLocation)
	{
		movement.BecomeSpooked(spookLocation);
	}
}
