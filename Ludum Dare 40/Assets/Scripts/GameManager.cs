using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] GameObject vip;
	public static GameObject VIP { get { return Instance.vip; } }

	[SerializeField] GameObject attackerPrefab;
	[SerializeField] GameObject innocentPrefab;

	[SerializeField] GameObject attackerSpawns;
	[SerializeField] GameObject innocentSpawns;

	enum GameState { Play, GameOver };
	GameState state = GameState.Play;

	Coroutine waitAndRestartRoutine = null;
	[SerializeField] float restartDelay;

	int numAttackersKilled = 0;
	[SerializeField] float minSpawnDelay;
	[SerializeField] float maxSpawnDelay;

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
				numAttackersKilled++;
				// "Kill attackers adds more people"
				SpawnCitizens(Mathf.Max(1, numAttackersKilled / 3), attackerSpawns.transform, attackerPrefab);
				SpawnCitizens(Mathf.Max(1, numAttackersKilled), innocentSpawns.transform, innocentPrefab);
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
		if (state == GameState.Play)
		{
			if (reason == Citizen.CitizenType.VIP)
			{
				Debug.Log("Game over, they can't keep getting away with it!");
			}
			else
			{
				Debug.Log("Game over, an innocents life has been lost.");
			}
			state = GameState.GameOver;
			waitAndRestartRoutine = StartCoroutine(waitThenCallback(restartDelay, () =>
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				waitAndRestartRoutine = null;
			}));
		}
	}

	void SpawnCitizens(int amount, Transform spawnPointsRoot, GameObject prefab)
	{
		for (int i = 0; i < amount; i++)
		{
			StartCoroutine(waitThenCallback(Random.Range(minSpawnDelay, maxSpawnDelay), () =>
			{
				Transform spawnPoint = spawnPointsRoot.GetChild(Random.Range(0, spawnPointsRoot.childCount - 1));
				Citizen citizen = GameObject.Instantiate(prefab, spawnPoint.position, Quaternion.identity).GetComponent<Citizen>();
				citizen.OnDeath += OnCitizenDied;
			}));
		}
	}

	private IEnumerator waitThenCallback(float waitTimeSeconds, System.Action callback)
	{
		yield return new WaitForSeconds(waitTimeSeconds);
		callback();
	}
}
