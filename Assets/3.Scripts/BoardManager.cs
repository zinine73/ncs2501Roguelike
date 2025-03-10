using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Tile[] WallTiles; // 테두리
    public FoodObject[] FoodPrefab;
    public WallObject[] WallPrefab; // 벽
    public ExitCellObject ExitPrefab;
    public EnemyObject[] EnemyPrefab;
    public int minFood;
    public int maxFood;
    public int minWall;
    public int maxWall;
    public int minEnemy;
    public int maxEnemy;

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
        
        // Exit
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        // 레벨에 따른 오브젝트 수 조정
        GenerateWall(AdjustObjectCount(true, 3, minWall, maxWall));
        GenerateFood(AdjustObjectCount(false, 4, minFood, maxFood));
        GenerateEnemy(AdjustObjectCount(true, 5, minEnemy, maxEnemy));
    }

    private int AdjustObjectCount(bool isAdd, int ratio, int min, int max)
    {
        int val = GameManager.Instance.Level / ratio;
        int tmp, rv;
        if (isAdd)
        {
            tmp = min + val;
            rv = (tmp > max) ? max : tmp;
        }
        else
        {
            tmp = max - val;
            rv = (tmp < min) ? min : tmp;
        }
        return rv;
    }

    public void Clean()
    {
        if (m_BoardData == null) return;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var cellData = m_BoardData[x, y];
                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }
                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile); 
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
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

    private void GenerateFood(int val)
    {
        int foodCount = Random.Range(val, val + 2);
        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            int foodType = Random.Range(0, FoodPrefab.Length);
            FoodObject newFood = Instantiate(FoodPrefab[foodType]);
            AddObject(newFood, coord);
        }
    }

    private void GenerateWall(int val)
    {
        int wallCount = Random.Range(val, val + 2);
        for (int i = 0; i < wallCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            int wallType = Random.Range(0, WallPrefab.Length);
            WallObject newWall = Instantiate(WallPrefab[wallType]);
            AddObject(newWall, coord);
        }
    }

    private void GenerateEnemy(int val)
    {
        int enemyCount = Random.Range(val, val + 2);
        for (int i = 0; i < enemyCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            int enemyType = Random.Range(0, EnemyPrefab.Length);
            EnemyObject newEnemy = Instantiate(EnemyPrefab[enemyType]);
            AddObject(newEnemy, coord);
        }
    }

    private void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }
}
