using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuPanelController : MonoBehaviour
{
    public Button mainMenuButton;
    public int mainMenuScene;
    public Button playButton;
    public int playScene;

    private PanelMode panelMode;
    private static MenuPanelController panel;

    void Start()
    {
        panel = this;
    }


    public static void ShowAsPause()
    {
        Show();
        panel.panelMode = PanelMode.Pause;
        panel.playButton.GetComponentInChildren<Text>().text = "Resume";
//        Game.Pause();
    }


    public static void ShowAsGameOver()
    {
        Show();
        panel.panelMode = PanelMode.GameOver;
        panel.playButton.GetComponentInChildren<Text>().text = "Retry";
//        Game.Pause();
    }



    public void Play()
    {        
        if (panelMode == PanelMode.GameOver)
        {
            Hide();
            Game.Resume();
            LoadScene(playScene);
        }
        else if (panelMode == PanelMode.Pause)
            StartCoroutine(Resume());
    }

    private IEnumerator Resume()
    {
        var second = 2;
        playButton.GetComponentInChildren<Text>().text = "3...";
        yield return new WaitForSeconds(second);
        playButton.GetComponentInChildren<Text>().text = "2...";
        yield return new WaitForSeconds(second);
        playButton.GetComponentInChildren<Text>().text = "1...";
        yield return new WaitForSeconds(second);

        Game.Resume();
        Hide();
    }


    public void MainMenu()
    {
        LoadScene(mainMenuScene);
    }

    private static void Show()
    {
        panel.transform.localScale = new Vector2(1, 1);
    }

    private void Hide()
    {        
        panel.transform.localScale = new Vector2(0, 0);
    }

    private void LoadScene(int level)
    {        
        Application.LoadLevel(level);
    }
}

enum PanelMode
{
    Pause,
    GameOver
}