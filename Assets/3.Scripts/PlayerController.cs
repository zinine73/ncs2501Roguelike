using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;
    public Vector2Int Cell { 
        get
        {
            return m_CellPosition;
        }
        private set{}
    }
    
    private readonly int hashMoving = Animator.StringToHash("Moving");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    private Animator m_Animator;
    private Vector2Int newCellTarget;
    private bool hasMoved;

    //public Vector2Int GetPlayerPosition()
    //{
    //    return m_CellPosition;
    //}

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();    
    }

    public void Init()
    {
        m_IsMoving = false;
        m_IsGameOver = false;
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate = false)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        m_Animator.SetBool(hashMoving, m_IsMoving);
    }

    private void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                m_MoveTarget, 
                MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool(hashMoving, false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }
            return;
        }

        newCellTarget = m_CellPosition;
        hasMoved = false;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MoveSkip();
        }
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            MoveUp();
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            MoveDown();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveRight();
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveLeft();
        }
    }

    private void UpdatePlayer()
    {
        if (hasMoved)
        {
            //셀이 움직일 수 있으면 움직여라
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget);
                }
                else
                { 
                    if (cellData.ContainedObject.PlayerWantsToEnter())
                    {
                        MoveTo(newCellTarget);
                    }
                    else
                    {
                        m_Animator.SetTrigger(hashAttack);
                    }
                }
            }
        }
    }

    public void MoveSkip()
    {
        if (m_IsGameOver)
        {
            GameManager.Instance.StartNewGame();
            return;
        }
        
        if (m_IsMoving) return;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveUp()
    {
        if (m_IsMoving) return;
        newCellTarget.y++;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveDown()
    {
        if (m_IsMoving) return;
        newCellTarget.y--;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveLeft()
    {
        if (m_IsMoving) return;
        newCellTarget.x--;
        hasMoved = true;
        UpdatePlayer();
    }

    public void MoveRight()
    {
        if (m_IsMoving) return;
        newCellTarget.x++;
        hasMoved = true;
        UpdatePlayer();
    }
}
