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
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCellsList;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public FoodObject[] FoodPrefab;
    public int minFood;
    public int maxFood;

    public void Init()
    {
        m_Grid = GetComponentInChildren<Grid>();
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_EmptyCellsList = new List<Vector2Int>();
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
                    // 비어있는 타일이므로, 빈타일리스트에 넣어준다
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        // 플레이어가 등장하는 위치는 빈타일이 아니므로 빼준다
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
        GenerateFood();
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

    private void GenerateFood()
    {
        int foodCount = Random.Range(minFood, maxFood + 1);
        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            int foodType = Random.Range(0, FoodPrefab.Length);
            FoodObject newFood = Instantiate(FoodPrefab[foodType]);
            newFood.transform.position = CellToWorld(coord);
            data.ContainedObject = newFood;
        }
    }
}
