using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackToMenuButton : ClickableObject
{
    public override void OnClick()
    {
        SceneManager.UnloadScene("sandbox");
    }

    public override void OnAltClick() { }
}
