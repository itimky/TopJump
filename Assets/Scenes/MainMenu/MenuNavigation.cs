using UnityEngine;

//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MenuNavigation : MonoBehaviour
{
    public ExitConfirmPanelController confirmPanel;
    private static List<int> LevelChain;

    void Awake()
    {
        Screen.fullScreen = false;
    }

    void Start()
    {       
        LevelChain = new List<int>(){ 0, 2 };
    }

    void Update()
    {
        if (InputManager.Back)
            OnBackPressed();

//        else if ()
//        if (Input.GetKeyDown(KeyCode.Escape))
//            OnBackPressed();
//        if (Input.touchCount > 0)
//        {
           
    }

    public void OnBackPressed()
    {
        var index = LevelChain.IndexOf(Application.loadedLevel);
        if (index == 0) // exit
        {
            if (confirmPanel.IsShown)
                confirmPanel.Hide();
            else
                confirmPanel.Show();
        }
        else
            Application.LoadLevel(LevelChain[index - 1]);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
