using UnityEngine;
using System.Collections;

public class CameraUpdater : MonoBehaviour
{
    private float startY;

    void Start()
    {
        enabled = false;
        startY = transform.position.y;
        enabled = true;
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var y = transform.localPosition.y;
        if (y < PlayerInfo.maxTraveled + startY)
            Camera.main.gameObject.transform.localPosition +=
                new Vector3(0, PlayerInfo.maxTraveled - y + startY, 0);
    }
}
