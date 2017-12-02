using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Debug.Log("Found multiple instances of GameManager.");
			Destroy(gameObject);
		}

		Instance = this;
	}

	void Start ()
	{
		GameObject[] citizenObjects = GameObject.FindGameObjectsWithTag("Citizen");
		for (int i = 0; i < citizenObjects.Length; i++)
		{
			Citizen citizen = citizenObjects[i].GetComponent<Citizen>();
			citizen.OnDeath += OnCitizenDied;
		}
	}
	
	void Update ()
	{
		
	}

	void OnCitizenDied(Citizen citizen)
	{
		switch (citizen.Type)
		{
			case Citizen.CitizenType.Innocent:
			{
				GameOver(Citizen.CitizenType.Innocent);
				break;
			}
			case Citizen.CitizenType.Attacker:
			{
				// "Kill attackers adds more people"
				break;
			}
			case Citizen.CitizenType.VIP:
			{
				GameOver(Citizen.CitizenType.VIP);
				break;
			}
		}
	}

	void GameOver(Citizen.CitizenType reason)
	{
		if (reason == Citizen.CitizenType.VIP)
		{
			Debug.Log("Game over, they can't keep getting away with it!");
		}
		else
		{
			Debug.Log("Game over, an innocents life has been lost.");
		}
	}
}
