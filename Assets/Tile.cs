using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class Tile : ClickableObject
{
    private Grid _parentGrid;
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsRevealed, HasBomb, IsFlagged;
    public int NumBombNeighbors;

    private TextMesh _numberTextMesh;
    private MeshRenderer _meshRenderer;
    private Collider _collider;
    // Use this for initialization
    void Start()
    {
        _numberTextMesh = GetComponentInChildren<TextMesh>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    public void Init(Grid inGrid, int inX, int inY)
    {
        _parentGrid = inGrid;
        X = inX;
        Y = inY;
    }

    public void LateInit()
    {
        NumBombNeighbors = _parentGrid.GetTileNeighbors(X, Y).Count(x => x.HasBomb);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RevealNumber()
    {
        float eighth = 1f / 8f;
        Color[] colors = new[]
        {
            Color.white,
            Color.yellow,
            new Color(1f,0.54f,0.3f),
            Color.red,
            new Color(0.5f,0f,0.5f),
            Color.blue,
            Color.cyan,
            Color.black,
            new Color(1f*eighth,0f,0f),//1f*eighth,1f*eighth),
            new Color(2f*eighth,0f,0f),//2f*eighth,2f*eighth),
            new Color(3f*eighth,0f,0f),//3f*eighth,3f*eighth),
            new Color(4f*eighth,0f,0f),//4f*eighth,4f*eighth),
            new Color(5f*eighth,0f,0f),//5f*eighth,5f*eighth),
            new Color(6f*eighth,0f,0f),//6f*eighth,6f*eighth),
            new Color(7f*eighth,0f,0f),//7f*eighth,7f*eighth),
            new Color(8f*eighth,0f,0f),//8f*eighth,8f*eighth),


        };

        IsRevealed = true;
        string text = "#";
        if (NumBombNeighbors == 0)
            text = "";
        else
        {
            text = NumBombNeighbors.ToString();
            _numberTextMesh.color = colors[NumBombNeighbors - 1];
        }

        if (HasBomb)
        {
            text = "*";
            _numberTextMesh.color = Color.red;
        }
        _numberTextMesh.text = text;
        _meshRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void ClickAction()
    {
        RevealNumber();
        if (HasBomb)
        {
            return;
        }
        if (NumBombNeighbors == 0)
        {
            foreach (var emptyTile in _parentGrid.GetTileNeighbors(X, Y).Where(t => !t.IsRevealed && !t.HasBomb))
            {
                emptyTile.ClickAction();
            }
        }
    }

    private void ToggleFlag()
    {
        IsFlagged = !IsFlagged;
        _numberTextMesh.text = IsFlagged ? "F" : "";
    }

    public override void OnClick()
    {
        if (!IsFlagged)
        {
            ClickAction();
            _parentGrid.CheckWinner();
        }
    }

    public override void OnAltClick()
    {
        ToggleFlag();
    }

    void OnDrawGizmos()
    {
        if (HasBomb)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position, Vector3.one * 0.9f);
        }
        if (IsRevealed)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(transform.position, Vector3.one * 0.9f);
        }
    }
}
