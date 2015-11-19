using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvailableGoldManager : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = string.Format("Gold: {0}", PlayerPrefs.GetInt(PlayerPrefKeys.Gold));
    }
}
