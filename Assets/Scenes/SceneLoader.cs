using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

	public void Load(int index)
	{
		SceneManager.LoadScene(index);
	}
}
