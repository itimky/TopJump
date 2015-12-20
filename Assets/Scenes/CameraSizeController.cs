using UnityEngine;
using System.Collections;

public class CameraSizeController : MonoBehaviour
{
	void Awake()
	{
		var mainCamera = Camera.main;
		var aspectRatio = 1 / mainCamera.aspect;
		if (aspectRatio < 1.7)
			return;

		mainCamera.transform.localPosition += new Vector3(0, 2, 0);
		mainCamera.orthographicSize += 2;
	}

}
