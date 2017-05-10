using UnityEngine;
using System.Collections;

public class ScalableFloor : MonoBehaviour
{
    public Grid grid;
    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(grid.Width + 5, transform.localScale.y, 2.0f * Mathf.Log(grid.Width));
        transform.position = -new Vector3(0, grid.Height / 2.0f + 1, 0);
        Destroy(this);
    }
}
