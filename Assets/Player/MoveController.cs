using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MoveDirection
{
    Up,
    Down,
    Right,
    Left,
    None
}

public class MoveController : MonoBehaviour
{
    public float speed;

    public static int currentTileNum { get; private set; }

    private Queue<float> targets;
    private float startPos;
    private float startTime;

    public static bool Paused { get; set; }

    void Start()
    {
        currentTileNum = Game.positions.Count / 2;
        targets = new Queue<float>();
        startPos = transform.position.x;
    }

    public void QueueMove(MoveDirection direction)
    {
        if (targets.Count > 1)
            return;

        var positions = Game.positions;

        int inc;
        if (direction == MoveDirection.Right)
            inc = 1;
        else
            inc = -1;

        var newTileNum = currentTileNum + inc;
        if (newTileNum < 0 || newTileNum >= positions.Count)
            return;

        currentTileNum = newTileNum;
        var target = positions[newTileNum];
        targets.Enqueue(target);
        if (targets.Count == 1)
            StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
//        var traveled = 0f;
        while (targets.Count != 0 && !Game.IsGameOver)
        {
            if (!Game.IsPaused)
            {
                if (startPos == transform.position.x)
                {
//                    traveled = 0;
                    startTime = Time.time;
                    yield return null;
                }
     
                var target = targets.Peek();
                if (transform.position.x == target)
                {
                    transform.position = new Vector2(target, transform.position.y);
                    targets.Dequeue();
                    startPos = transform.position.x;
                }
                else
                {
//                    traveled += speed;
                    float newXPos = Mathf.MoveTowards(startPos, target, speed * (Time.time - startTime));
                    transform.position = new Vector2(newXPos, transform.position.y);
                }

            }
            else
                startTime += Time.deltaTime;
            yield return null;//WaitForSeconds(0.1f * Time.timeScale);
        }
    }
}