using UnityEngine;
using System.Collections;

public class SpikePlatformBehavior : MonoBehaviour
{
    public bool IsHarmful { get { return Spikes.AreSpikesUp; } }

    private SpikesBehavior Spikes;

    void Start()
    {
        Spikes = transform.Find("Spikes").GetComponent<SpikesBehavior>();
    }
}
