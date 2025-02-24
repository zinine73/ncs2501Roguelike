using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public PlayerController Player;

    void Start()
    {
        m_Grid = GetComponentInChildren<Grid>();
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    // Wall tile
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    // ground tile
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;
                }
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        Player.Spawn(this, new Vector2Int(1, 1));
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }
}
