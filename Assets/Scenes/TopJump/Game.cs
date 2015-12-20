using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;

public class Game : MonoBehaviour
{
	public static bool IsPaused { get; private set; }

	public static bool IsGameOver { get; private set; }

	private static List<Pausable> PausableObjects;
	private static List<IGameOverHandler> GameOverHandlers;
	public List<float> Positions;
	//	public static readonly List<float> positions = new List<float> { -4f, 0f, 4f };
	public static List<float> positions { get; private set; }

	void Awake()
	{
		positions = Positions;
		Screen.fullScreen = false;
		PausableObjects = new List<Pausable>();
		GameOverHandlers = new List<IGameOverHandler>();
	}

	void Start()
	{
		Resume();
	}


	public static GameObject MakePrefabInstance(GameObject prefab)
	{
		var instance = Instantiate(prefab);
		instance.transform.localPosition = new Vector2(0, -60);
		return instance;
	}

	public static Transform MakePrefabInstance(Transform prefab)
	{
		var instance = Instantiate(prefab);
		instance.localPosition = new Vector2(0, -60);
		return instance;
	}

	public static RectTransform MakePrefabInstance(RectTransform prefab)
	{
		return Instantiate(prefab);
	}


	public static void RegisterPausableObject(Pausable pausable)
	{
		if (!PausableObjects.Contains(pausable))
			PausableObjects.Add(pausable);
	}

	public static void RegisterGameOverHandler(IGameOverHandler handler)
	{
		if (!GameOverHandlers.Contains(handler))
			GameOverHandlers.Add(handler);
	}


	public static void Pause()
	{
		if (IsPaused)
			return;

		IsPaused = true;
		MenuPanelController.ShowAsPause();
		PausableObjects.ForEach(po => po.Pause());
	}

	public void PauseNonStatic()
	{
		Pause();
	}

	public static void Resume()
	{
		if (!IsPaused)
			return;

		IsPaused = false;
		IsGameOver = false;
		PausableObjects.ForEach(po => po.Resume());
	}


	public static void GameOver()
	{
		IsPaused = true;
		IsGameOver = true;
		MenuPanelController.ShowAsGameOver();
		PausableObjects.ForEach(po => po.Pause());

		GameOverHandlers.ForEach(goh => goh.OnGameOver());
		PlayerPrefs.Save();
	}



}


public abstract class Pausable : MonoBehaviour
{
	public virtual void Pause()
	{		
		enabled = false;
	}

	public virtual void Resume()
	{
		enabled = true;
	}
}

public interface IGameOverHandler
{
	void OnGameOver();
}