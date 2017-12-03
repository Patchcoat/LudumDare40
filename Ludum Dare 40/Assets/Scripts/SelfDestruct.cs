using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
	[SerializeField] bool waitForParticleSystems;
	[SerializeField] float waitTimeSeconds = 0.0f;

	void Start()
	{
		if (waitForParticleSystems)
		{
			ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < particleSystems.Length; ++i)
			{
				waitTimeSeconds = Mathf.Max(waitTimeSeconds, particleSystems[i].main.duration);
			}
		}

		if (waitTimeSeconds > 0.0f)
		{
			StartCoroutine("WaitAndDestroy");
		}
		else
		{
			Destroy(gameObject);
		}
	}

	IEnumerator WaitAndDestroy()
	{
		yield return new WaitForSeconds(waitTimeSeconds);
		Destroy(gameObject);
	}

}
