using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Reign;

/// <summary>
/// InputEx service, used for enhancing Unitys Input API.
/// </summary>
public class InputExService : MonoBehaviour
{
	private static InputExService singleton;

	void Start()
	{
		if (singleton != null)
		{
			Destroy(gameObject);
			return;
		}

		singleton = this;
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		foreach (var map in InputEx.ButtonMappings) map.update();
	}
}