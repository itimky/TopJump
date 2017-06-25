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
    private Transform tr;
    private PlayerInfo playerInfo;

    public static bool Paused { get; set; }

    void Start()
    {
        tr = transform;
        this.playerInfo = GetComponent<PlayerInfo>();
        currentTileNum = Game.positions.Count / 2;
        targets = new Queue<float>();
        startPos = tr.position.x;
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
                // pre move start
                if (startPos == tr.position.x)
                {
//                    traveled = 0;
                    var trg = targets.Peek();
                    MoveDirection dir;
                    if (trg > tr.position.x)
                        dir = MoveDirection.Right;
                    else
                        dir = MoveDirection.Left;
                    playerInfo.MoveTo(dir);
                    startTime = Time.time;
                    yield return null;
                }

                var target = targets.Peek();
                // move ending
                if (tr.position.x == target)
                {
                    tr.position = new Vector2(target, tr.position.y);
                    targets.Dequeue();
                    startPos = tr.position.x;
                }
                // move process
                else
                {
//                    traveled += speed;
                    float newXPos = Mathf.MoveTowards(startPos, target, speed * (Time.time - startTime));
                    tr.position = new Vector2(newXPos, tr.position.y);
                }

            }
            else
                startTime += Time.deltaTime;
            yield return null;//WaitForSeconds(0.1f * Time.timeScale);
        }
    }
}