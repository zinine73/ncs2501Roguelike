using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;
    public Tile HP1Tile;
    public int MaxHealth = 3;
    public AudioClip Clip;
    public AudioClip Clip2;

    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public override void Init(Vector2Int cell)
    {
        //m_Cell = cell;
        base.Init(cell);
        m_HealthPoint = MaxHealth;
        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
    }

    public override void PlayerEntered()
    {
        
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint -= 1;
        if (m_HealthPoint > 0)
        {
            if (m_HealthPoint == 1)
            {
                GameManager.Instance.BoardManager.SetCellTile(m_Cell, HP1Tile);
            }
            GameManager.Instance.PlaySound(Clip);
            return false;
        }
        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
        Destroy(gameObject);
        GameManager.Instance.PlaySound(Clip2);
        return true;
    }
}
