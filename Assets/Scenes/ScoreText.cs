using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreText : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<TextMesh>().text = PlayerPrefManager.GetMaxScore().ToString();
    }
}
