using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Public
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public UIDocument UIDoc;
    #endregion

    // property
    public TurnManager TurnManager { get; private set; }
    public int Level { 
        get { return m_CurrentLevel; }
        private set{}
    }

    #region private
    private const int START_FOOD_AMOUNT = 100;
    private const string GOS1 = "Game Over!\n\nYou traveled\nthrough";
    private const string GOS2 = "levels\n\n(Press Enter to\nstart New game)";
    private int m_FoodAmount;
    private Label m_FoodLabel;
    private int m_CurrentLevel;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    #endregion

    #region Singleton
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }    
        Instance = this;
    }
    #endregion

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_CurrentLevel = 0;
        m_FoodAmount = START_FOOD_AMOUNT;
        m_FoodLabel.text = $"Food : {m_FoodAmount:000}";

        PlayerController.Init();
        //BoardManager.Clean();
        //BoardManager.Init();
        //PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        NewLevel();
    }

    public void NewLevel()
    {
        m_CurrentLevel++;
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = $"Food : {m_FoodAmount:000}";

        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = $"{GOS1} {m_CurrentLevel} {GOS2}";
        }
    }
}
