
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


static class Util
{
    public static T GetRootComponent<T>() where T : Component
    {
        return SceneManager.GetActiveScene().GetRootGameObjects().Select(a => a.GetComponent<T>()).Single(a => a != null);
    }

}