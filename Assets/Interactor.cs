using UnityEngine;
using System.Collections;

public class Interactor : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                var clickableObject = hit.transform.gameObject.GetComponent<ClickableObject>();
                if (clickableObject)
                {
                    clickableObject.OnClick();
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                var clickableObject = hit.transform.gameObject.GetComponent<ClickableObject>();
                if (clickableObject)
                {
                    clickableObject.OnAltClick();
                }
            }
        }
    }
}

public abstract class ClickableObject : MonoBehaviour
{
    public abstract void OnClick();
    public abstract void OnAltClick();
}
