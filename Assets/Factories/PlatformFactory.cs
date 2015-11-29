using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlatformFactory : MonoBehaviour
{
    public GameObject simplePrefab;
    public GameObject superJumpPrefab;
    public GameObject astralPrefab;
    public GameObject spikePrefab;

    public GameObject floorPrefab;

    public Vector2 PlatformScale;

    protected Queue<GameObject> objectQueue;

    protected float nextY;


    public int instanceCount;
    public float recycleOffset;
    public float verticalDistance;
    public float BaseLine;


    public static float VerticalDistance;

    public static List<GameObject> AllPlatforms { get; private set; }

    private static Queue<GameObject> simpleQueue;
    private static Queue<GameObject> updatingQueue;
    private static Queue<GameObject> bonusPlatformQueue;


    void Start()
    {
        CreatePlatforms();

        nextY = BaseLine + verticalDistance;
        objectQueue = new Queue<GameObject>();
        foreach (var item in AllPlatforms.Take(instanceCount))
            objectQueue.Enqueue(item);
    }

    private void CreatePlatforms()
    {
        VerticalDistance = verticalDistance;
        simpleQueue = new Queue<GameObject>();
        updatingQueue = new Queue<GameObject>();
        bonusPlatformQueue = new Queue<GameObject>();

        for (int i = 0; i < instanceCount; i++)
            simpleQueue.Enqueue(Game.MakePrefabInstance(simplePrefab));        

        for (int i = 0; i < instanceCount / 3; i++)
        {
            updatingQueue.Enqueue(Game.MakePrefabInstance(astralPrefab));
            updatingQueue.Enqueue(Game.MakePrefabInstance(spikePrefab));
        }

        for (int i = 0; i < instanceCount / 3; i++)
            bonusPlatformQueue.Enqueue(Game.MakePrefabInstance(superJumpPrefab));

        var grass = Game.MakePrefabInstance(floorPrefab);
        grass.transform.localPosition = new Vector2(0, -5);
        AllPlatforms = simpleQueue.Concat(updatingQueue).Concat(bonusPlatformQueue).ToList();
    }

    void Update()
    {
        if (objectQueue.Peek().transform.localPosition.y + recycleOffset < PlayerInfo.maxTraveled)
            Recycle();
    }

    private void Recycle()
    {
        objectQueue.Dequeue();
        objectQueue.Enqueue(GeneratePlatform());
    }



    private GameObject GeneratePlatform()
    {
        var platform = GetPlatform();
        SetPosition(platform.transform);
        PlatformBonusManager.OnPlatformCreated(platform);
        return platform;
    }


    private GameObject GetPlatform()
    {
        var random = Random.Range(0, 10);
        var anySpecialPlatformLower = AllPlatforms.Any(p => !p.CompareTag("SimplePlatform") && p.transform.localPosition.y == nextY - verticalDistance);

        GameObject platform = null;

        if (!anySpecialPlatformLower)
        {
            if (random == 0)
                platform = Reuse(updatingQueue);
            if (random == 1)
                platform = Reuse(bonusPlatformQueue);
        }

        if (platform == null)
            platform = Reuse(simpleQueue);
        

        return platform;
    }


    private GameObject Reuse(Queue<GameObject> queue)
    {
        var platform = queue.Dequeue();
        queue.Enqueue(platform);
        return platform;
    }

    private void SetPosition(Transform transform)
    {
        var currentLinePlatforms = objectQueue.Where(p => p.transform.localPosition.y == nextY).ToList();
        var notUsedPositions = Game.positions.Except(currentLinePlatforms.Select(c => c.transform.localPosition.x)).ToList();

        var platformX = notUsedPositions[Random.Range(0, notUsedPositions.Count)];

        transform.localPosition = new Vector2(platformX, nextY);

        var currentCount = currentLinePlatforms.Count + 1;
        bool addHeight = currentCount == Game.positions.Count - 1 || Random.Range(0, Game.positions.Count - 1) < currentCount;

        if (addHeight)
            nextY += verticalDistance;
    }
}