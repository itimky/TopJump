using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FpsCalc : MonoBehaviour
{
    public float updateInterval;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    private Text FpsText;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        FpsText = GetComponent<Text>();
    }
    //
    //    void OnGUI()
    //    {
    //        GUILayout.Label("" + fps.ToString("f2"));
    //    }

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }

        FpsText.text = fps.ToString("f2");
    }
}
