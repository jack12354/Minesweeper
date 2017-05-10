using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    private int width, height, numBombs;
    public bool Cleared;
    private Tile[,] tiles;

    public enum Result
    {
        Win, Bomb, Reset
    }

    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int NumBombs { get { return numBombs; } }

    // Use this for initialization
    void Start()
    {
        width = PlayerPrefs.GetInt("Width");
        height = PlayerPrefs.GetInt("Height");
        numBombs = PlayerPrefs.GetInt("NumBombs");
        Reset();
    }

    public void Reset()
    {
        Clear(0, Result.Reset);
        Init();
        GameObject.FindObjectOfType<SmartCamera>().Init();
    }

    public void Init()
    {
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileGO = Instantiate(Resources.Load("TilePrefab"), GetPosForGrid(x, y), Quaternion.identity) as GameObject;
                Debug.Assert(tileGO != null, "tileGO != null");
                tileGO.transform.parent = transform;
                tileGO.name = "Tile (" + x + ", " + y + ")";
                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(this, x, y);
                tiles[x, y] = tile;
            }
        }



        HashSet<Vector2> bombs = new HashSet<Vector2>();
        while (bombs.Count < numBombs)
        {
            bombs.Add(new Vector2(Random.Range(0, width), Random.Range(0, height)));
        }
        foreach (var vector2 in bombs)
        {
            tiles[(int)vector2.x, (int)vector2.y].HasBomb = true;
        }

        foreach (var tile in tiles)
        {
            tile.LateInit();
        }
        Cleared = false;
    }

    public void Clear(float inTime, Result result, params object[] values)
    {
        Cleared = true;
        if (tiles == null) return;
        switch (result)
        {
            case Result.Win:
                const float liftPower = 100.0f, spinPower = 100.0f;
                foreach (var tile in tiles)
                {
                    if (!tile) continue;
                    if (!tile.IsRevealed || tile.NumBombNeighbors > 0)
                    {
                        tile.gameObject.AddComponent<Rigidbody>();
                        var tileRigidbody = tile.GetComponent<Rigidbody>();
                        tileRigidbody.useGravity = false;
                        tileRigidbody.AddForce(
                            Random.Range(-liftPower/4.0f, liftPower/4.0f),
                            Random.Range(liftPower/2.0f, liftPower),
                            Random.Range(-liftPower/4.0f, liftPower/4.0f));
                        tileRigidbody.AddRelativeTorque(
                            Random.Range(-spinPower, spinPower),
                            Random.Range(-spinPower, spinPower),
                            Random.Range(-spinPower, spinPower));
                    }
                    Destroy(tile.gameObject, Random.value * (inTime - 1.5f) + 1.5f);
                }
                break;
            case Result.Bomb:
                Vector3 bombPos = (Vector3)values[0];
                const float bombPower = 300.0f, twistPower = 20.0f;
                foreach (var tile in tiles)
                {
                    if (!tile) continue;
                    if (!tile.IsRevealed || tile.NumBombNeighbors > 0)
                    {
                        tile.gameObject.AddComponent<Rigidbody>();
                        tile.GetComponent<Rigidbody>()
                            .AddExplosionForce(bombPower, bombPos + new Vector3(0, 0, 2.0f), width);
                        tile.GetComponent<Rigidbody>()
                            .AddRelativeTorque(new Vector3(Random.Range(-twistPower, twistPower),
                                Random.Range(-twistPower, twistPower),
                                Random.Range(-twistPower, twistPower)));
                    }
                    Destroy(tile.gameObject, Random.value * (inTime - 1.5f) + 1.5f);
                }
                break;
            case Result.Reset:
                const float power = 100.0f;

                foreach (var tile in tiles)
                {
                    if (!tile) continue;
                    if (!tile.IsRevealed || tile.NumBombNeighbors > 0)
                    {
                        tile.gameObject.AddComponent<Rigidbody>();
                        tile.GetComponent<Rigidbody>()
                            .AddRelativeForce(new Vector3(Random.Range(-power, power), Random.Range(-power, power),
                                Random.Range(-power, power)));
                        tile.GetComponent<Rigidbody>()
                            .AddRelativeTorque(new Vector3(Random.Range(-power, power), Random.Range(-power, power),
                                Random.Range(-power, power)));
                    }
                    Destroy(tile.gameObject, Random.value * (inTime - 1.5f) + 1.5f);
                }
                break;
        }

    }

    private Vector3 GetPosForGrid(int x, int y)
    {
        float CellSize = 1.0f;
        return transform.position + new Vector3(x - (width / 2.0f) * CellSize, y - (height / 2.0f) * CellSize) + Vector3.one *(CellSize/2.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Tile> GetTileNeighbors(Tile inTile)
    {
        return GetTileNeighbors(inTile.X, inTile.Y);
    }

    public List<Tile> GetTileNeighbors(int inX, int inY)
    {
        List<Tile> outList = new List<Tile>();

        if (inX - 1 > -1)
        {
            outList.Add(tiles[inX - 1, inY]);
            if (inY - 1 > 0)
                outList.Add(tiles[inX - 1, inY - 1]);
            if (inY + 1 < height)
                outList.Add(tiles[inX - 1, inY + 1]);
        }

        if (inX + 1 < width)
        {
            outList.Add(tiles[inX + 1, inY]);
            if (inY - 1 > 0)
                outList.Add(tiles[inX + 1, inY - 1]);
            if (inY + 1 < height)
                outList.Add(tiles[inX + 1, inY + 1]);
        }

        if (inY - 1 > -1)
            outList.Add(tiles[inX, inY - 1]);
        if (inY + 1 < height)
            outList.Add(tiles[inX, inY + 1]);

        return outList;
    }

    public void CheckWinner()
    {
        Debug.Log("dig");
        bool anyLeft = false;
        foreach (var tile in tiles)
        {
            if (tile.HasBomb && tile.IsRevealed)
            {
                Debug.Log("YOU LOSE");
                Clear(5, Result.Bomb, tile.transform.position);
                return;
            }
            if (!tile.HasBomb && !tile.IsRevealed)
            {
                anyLeft = true;
            }
        }
        if (!anyLeft)
        {
            Clear(5, Result.Win);
            return;
        }
    }
}