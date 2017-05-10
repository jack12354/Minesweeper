using UnityEngine;
using System.Collections;

public class ResetButton : ClickableObject
{
    public Grid GridToReset;
    private Collider collider;
    // Use this for initialization
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ResetCoroutine()
    {
        float time = 2.0f;
        GridToReset.Clear(time, Grid.Result.Reset);

        yield return new WaitForSeconds(time);
        GridToReset.Init();

    }

    IEnumerator ReenableButton()
    {
        while (GridToReset.Cleared)
        {
            yield return new WaitForSeconds(0.5f);
        }
        collider.enabled = true;
    }

    public override void OnClick()
    {
        collider.enabled = false;
        if (GridToReset.Cleared)
        {
            GridToReset.Init();
        }
        else
        {
            StartCoroutine(ResetCoroutine());
        }
        StartCoroutine(ReenableButton());
    }

    public override void OnAltClick() { }
}
