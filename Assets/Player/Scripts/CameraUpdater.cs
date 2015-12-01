using UnityEngine;
using System.Collections;

public class CameraUpdater : MonoBehaviour
{
    //    private float startY;
    //
    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
//        GetComponent<Rigidbody2D>().inertia = 0;
        //GetComponent<Rigidbody2D> = 0;
//            enabled = false;
//            startY = transform.position.y;
//            enabled = true;
    }

    void FixedUpdate()
    {
        rigid.velocity = Vector2.zero;
    }
    //
    //    void Update()
    //    {
    //        UpdatePosition();
    //    }
    //
    //    private void UpdatePosition()
    //    {
    //        var y = transform.localPosition.y;
    //        if (y < PlayerInfo.maxTraveled + startY)
    //            Camera.main.gameObject.transform.localPosition +=
    //                new Vector3(0, PlayerInfo.maxTraveled - y + startY, 0);
    //    }
}
