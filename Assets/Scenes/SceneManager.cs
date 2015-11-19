using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{

    public void Load(int index)
    {
        Application.LoadLevel(index);
    }
}
