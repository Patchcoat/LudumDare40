using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
	[SerializeField] float health;
    [SerializeField] float despawnTime;
    [SerializeField] float rigorMortisTime;

    bool isAlive = true;
    bool frozen = false;
    float dissolve = 0;
    float timeDead = 0;
    //Material material;
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
        //material = GetComponentInChildren<Renderer>().material;
	}

	void Update ()
	{
        if (Input.GetButtonDown("Fire3"))
        {
            Die();
        }
        if (!isAlive)
        {
            timeDead += Time.deltaTime;
            if (timeDead >= despawnTime)
            {
                Destroy(gameObject);
            }
            /*if (despawnTime - timeDead <= 1)
            {
                dissolve += Time.deltaTime;
                material.SetFloat("Slice Amount", dissolve);
            }*/
            if (timeDead > rigorMortisTime && !frozen)
            {
                Freeze();
            }
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
            collider.enabled = true;
        }
        foreach (BoxCollider collider in Boxes)
        {
            collider.enabled = true;
        }
        foreach (CapsuleCollider collider in Capsules)
        {
            collider.enabled = true;
        }

        //GetComponent<CapsuleCollider>().enabled = false;
        //GetComponent<SphereCollider>().enabled = false;

		if (OnDeath != null)
		{
			OnDeath(this);
		}
	}

    void Freeze()
    {
        Component[] Rigidbodies;
        Rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in Rigidbodies)
        {
            body.useGravity = false;
            body.isKinematic = true;
        }
    }

	public void OnSpooked(Vector3 spookLocation)
	{
		// 2spooky
		if (type != CitizenType.Attacker)
		{
			movement.BecomeSpooked(spookLocation);
		}
	}
}
