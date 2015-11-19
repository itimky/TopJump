using UnityEngine;
using System.Collections;

public class ExitConfirmPanelController : MonoBehaviour
{
    public bool IsShown { get; private set; }

    public void Show()
    {
        IsShown = true;
        transform.localScale = new Vector2(1, 1);
    }

    public void Hide()
    {
        IsShown = false;
        transform.localScale = Vector2.zero;
    }
}
