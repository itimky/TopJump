using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaxScoreManager : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = string.Format("Max Score: {0}", PlayerPrefs.GetInt(PlayerPrefKeys.MaxScore));
    }
}
