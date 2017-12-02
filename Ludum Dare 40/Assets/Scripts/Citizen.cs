using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
	[SerializeField] private float health;

	bool isAlive = true;
	public bool IsAlive { get { return isAlive; } }

	CitizenMovement movement = null;

	public delegate void MultiDelegate(Citizen selfRef);
	public MultiDelegate OnDeath { get; set; }

	public enum CitizenType
	{
		Innocent,
		Attacker,
		VIP
	};

	[SerializeField]
	private CitizenType type;
	public CitizenType Type { get { return type; } }

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
			OnDeath.Invoke(this);
		}
	}
}
