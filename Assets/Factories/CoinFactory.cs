using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CoinFactory : MonoBehaviour
{
    public GameObject CoinPrefab;
    public int instanceCount;
    public float verticalDistance;
    public float recycleOffset;
    public float BaseLine;


    public static List<GameObject> AllCoins;

    private Queue<GameObject> objectQueue;

    private float nextY;

    void Start()
    {
        CreateCoins();

        nextY = BaseLine + verticalDistance;
        objectQueue = new Queue<GameObject>();
        foreach (var item in AllCoins)
            objectQueue.Enqueue(item);

        InvokeRepeating("UpdateCoins", 0f, 0.25f);
    }

    private void CreateCoins()
    {
        toSetPosition = new List<Transform>();
        AllCoins = new List<GameObject>();
        for (int i = 0; i < instanceCount; i++)
            AllCoins.Add(Game.MakePrefabInstance(CoinPrefab));
    }

    void UpdateCoins()
    {
        while (objectQueue.Peek().transform.localPosition.y + recycleOffset < PlayerInfo.maxTraveled)
            Recycle();
    }
    //
    private void Recycle()
    {
        var coin = objectQueue.Dequeue();
        coin.SetActive(true);
        SetPosition(coin.transform);
        objectQueue.Enqueue(coin);
    }
        
    //
    //    private GameObject GenerateCoin()
    //    {
    //        var obj = GetCoin();
    //        SetPosition(obj.transform);
    //        return obj;
    //    }
    //
    //    private GameObject GetCoin()
    //    {
    //        var obj = Reuse(objectQueue);
    //        obj.SetActive(true);
    //        return obj;
    //    }
    //
    //    private GameObject Reuse(Queue<GameObject> queue)
    //    {
    //        var obj = queue.Dequeue();
    //        queue.Enqueue(obj);
    //        return obj;
    //    }
    //
    //
    float xPos;
    List<float> xPoses;
    int coinsLineLeft;
    List<Transform> toSetPosition;
    //
    protected void SetPosition(Transform transform)
    {
        if (PlayerInfo.HasJetPack())
        {
            if (coinsLineLeft == 0)
            {
                var index = Game.positions.IndexOf(xPos);
                if (index == 0)
                    xPos = Game.positions[index + 1];
                else if (index == Game.positions.Count - 1)
                    xPos = Game.positions[index - 1];
                else
                {
                    if (Random.Range(0, 2) == 0)
                        xPos = Game.positions[index + 1];
                    else
                        xPos = Game.positions[index - 1];
                }
    
                coinsLineLeft = Random.Range(3, 10);
            }
    
            transform.localPosition = new Vector2(xPos, nextY);
            nextY += verticalDistance;
            coinsLineLeft--;
        }
        else
        {
            if (coinsLineLeft == 0)
            {
                int lineCount;
                if (Random.Range(0, 10) == 9)
                    lineCount = 3;
                else if (Random.Range(0, 5) == 4)
                    lineCount = 2;
                else
                    lineCount = 1;
    
                var possibleXPoses = Game.positions.ToList();
                xPoses = new List<float>();
                for (int i = 0; i < lineCount; i++)
                {
                    var xPos = possibleXPoses[Random.Range(0, possibleXPoses.Count)];
                    possibleXPoses.Remove(xPos);
                    xPoses.Add(xPos);
                }
                coinsLineLeft = Random.Range(3, 6);
            }
    
            toSetPosition.Add(transform);
            if (xPoses.Count == toSetPosition.Count)
            {
                for (int i = 0; i < xPoses.Count; i++)
                    toSetPosition[i].localPosition = new Vector2(xPoses[i], nextY);
    
                nextY += verticalDistance;
                coinsLineLeft--;
                toSetPosition.Clear();
            }
        }
    }

    public static void GoldBeenUsed(GameObject gold)
    {
        gold.SetActive(false);
    }
}
