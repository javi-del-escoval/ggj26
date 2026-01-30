using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { running, paused, menu, lost, won }
    private GameState gameState;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        if (gameState == 0) {
            setGameState(GameState.menu);
        }
    }

    void Update(){}

    public static void setGameState(GameState newState)
    {
        switch(newState) {
            case GameState.running:
                break;
            case GameState.paused:
                break;
            case GameState.menu:
                break;
            case GameState.lost:
                break;
            case GameState.won:
                break;
        }
    }
}
