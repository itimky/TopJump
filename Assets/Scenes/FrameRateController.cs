using UnityEngine;
using System.Collections;

public class FrameRateController : MonoBehaviour
{

    public int TargetFps;

    void Awake()
    {
        Application.targetFrameRate = TargetFps;
    }
}
