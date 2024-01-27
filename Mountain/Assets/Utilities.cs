using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
	public static T GetRootComponent<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.Select(a => a.GetComponent<T>())
			.Single(a => a != null);
	}

	public static void DestroyAllChildren(Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}
}
