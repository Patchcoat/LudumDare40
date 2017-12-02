using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
	[SerializeField] private float health;

	bool isAlive = true;

	CitizenMovement movement = null;

	private void Awake()
	{
		movement = GetComponent<CitizenMovement>();
	}

	void Start ()
	{
		
	}

	void Update ()
	{
		
	}

	public void OnHit(float damage)
	{
		health -= damage;
		if (health <= 0.0f)
		{
			isAlive = false;
			movement.enabled = false;
		}
	}
}
