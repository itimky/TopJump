using UnityEngine;
using System.Collections;

public abstract class TogglePlatform : MonoBehaviour
{
    public float ToggleFrequency;
    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Toggle", 0, ToggleFrequency);
    }

    protected abstract void Toggle();
}
