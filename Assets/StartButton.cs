using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : ClickableObject
{

    public override void OnClick()
    {
        PlayerPrefs.SetInt("Width", 8);
        PlayerPrefs.SetInt("Height", 8);
        PlayerPrefs.SetInt("NumBombs", 10);

        SceneManager.LoadScene("sandbox");
    }

    public override void OnAltClick() { }
}
