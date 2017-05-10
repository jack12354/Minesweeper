using UnityEngine;
using System.Collections;

public class SmartCamera : MonoBehaviour
{
    public Grid grid;
    // Use this for initialization
    void Start()
    {
        
    }

    public void Init()
    {
        transform.position = new Vector3(0, 0, -(Mathf.Max(grid.Height, grid.Width)));
    }
    // Update is called once per frame
    void Update()
    {

    }
}
