using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CloudManager : MonoBehaviour
{

	public Transform CloudPrefab_1;
	public Transform CloudPrefab_2;
	public Transform CloudPrefab_3;
	public Transform CloudPrefab_4;
	List<Transform> prefabs;
	List<Cloud> AllClouds;

	int direction;
	public int paddingX;
	public int verticalDistance;
	public int deltaY;
	public int deltaX;

	public int OffsetY;
	private int nextY;

	public int InstanceCount;
	public int FarDistance;

	void Start()
	{
//        nextY = verticalDistance;
		prefabs = new List<Transform>();
		prefabs.Add(CloudPrefab_1);
		prefabs.Add(CloudPrefab_2);
		prefabs.Add(CloudPrefab_3);
		prefabs.Add(CloudPrefab_4);
		if (Random.Range(0, 2) == 1)
			direction = 1;
		else
			direction = -1;

		AllClouds = new List<Cloud>();

		CreateClouds();
		InvokeRepeating("CheckClouds", 1f, 0.2f);
	}

	void CheckClouds()
	{

		List<Cloud> cloudsHorizontalUpdate;
		if (direction > 0)
		{
			var maxX = Game.positions[Game.positions.Count - 1];
			cloudsHorizontalUpdate = AllClouds.Where(c => c.Transform.position.x > maxX + paddingX).ToList();
			foreach (var cloud in cloudsHorizontalUpdate)
				SetPosition(cloud);            
		}
		else
		{
			var minX = Game.positions[0];
			cloudsHorizontalUpdate = AllClouds.Where(c => c.Transform.position.x < minX - paddingX).ToList();
			foreach (var cloud in cloudsHorizontalUpdate)
				SetPosition(cloud);
		}



		var cloudsHeightUpdate = AllClouds.Where(c => PlayerInfo.maxTraveled - c.Height > OffsetY).ToList();
		if (cloudsHeightUpdate.Count == 0)
			return;

		foreach (var cloud in cloudsHeightUpdate)
		{
			cloud.Height = nextY;
			SetPosition(cloud);
		}

		nextY += verticalDistance;
	}

	void CreateClouds()
	{
		AllClouds = new List<Cloud>();

		int i = 0;
		while (AllClouds.Count < InstanceCount)
		{
			var prefab = prefabs[i];
			var gameObject = Game.MakePrefabInstance(prefab);
			var goTransform = gameObject.transform;
			goTransform.localPosition = new Vector3(goTransform.localPosition.x, goTransform.localPosition.y, FarDistance);
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * direction, 0);

			var cloud = new Cloud(gameObject.transform, nextY);
			SetPosition(cloud);
			AllClouds.Add(cloud);
			i++;

			if (i % 2 != 0)
				nextY += verticalDistance;

			if (i == prefabs.Count)
				i = 0;
		}

		var sign = direction > 0 ? -1 : 1;
		foreach (var cloud in AllClouds)
		{
			var scale = Random.Range(1, 4);
			cloud.Transform.localScale = new Vector3(scale, scale, 1);
			cloud.Transform.position += new Vector3(sign * Random.Range(0, deltaX + 1), 0, 0);
		}
		AllClouds.Shuffle();
	}

	void SetPosition(Cloud cloud)
	{
		var padding = 30;
		int yDelta = Random.Range(0, deltaY + 1);
		if (direction > 0)
            //65318.88
            //20105.95
		{
			var minX = Game.positions[0];
			cloud.Transform.position = new Vector3(minX - paddingX, cloud.Height + yDelta, cloud.Transform.position.z);
			if (AllClouds.Any(c => !ReferenceEquals(c, cloud) && c.Height == cloud.Height && c.Transform.position.x == cloud.Transform.position.x))// && Mathf.Abs(c.Transform.position.x - cloud.Transform.position.x) < float.Epsilon))
                cloud.Transform.position -= new Vector3(padding, 0, 0);
		}
		else
		{
			var maxX = Game.positions[Game.positions.Count - 1];
			cloud.Transform.position = new Vector3(maxX + paddingX, cloud.Height + yDelta, cloud.Transform.position.z);
			if (AllClouds.Any(c => !ReferenceEquals(c, cloud) && c.Height == cloud.Height && c.Transform.position.x == cloud.Transform.position.x))//Mathf.Abs(c.Transform.position.x - cloud.Transform.position.x) < float.Epsilon))
                cloud.Transform.position += new Vector3(padding, 0, 0);
		}
	}
}


public class Cloud
{
	public readonly Transform Transform;
	public int Height;

	public Cloud(Transform cloud, int height)
	{
		Transform = cloud;
		Height = height;
	}
}