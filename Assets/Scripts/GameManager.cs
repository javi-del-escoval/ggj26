using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameState { running, paused, lose }
	GameState gameState;
	int currentScore, highScore;
	public int comboCounter { get; private set; }
	float comboThreshold, timeSinceLastComboInteraction = 0f;
	bool wasInteracted;
	public float speed { get; private set; }
	public float difficulty=1, difficultyFactor=1/6000, runTime = 0;
	public Material backgroundMaterial;
	public PlayerMovement player;
	public static GameManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			// If an instance already exists and it's not this one, destroy this duplicate
			Destroy(this.gameObject);
		}
		else
		{
			// Otherwise, set the instance to this object and ensure it persists across scenes
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}
	private void Start() {
		comboCounter = 0;
		comboThreshold = ObstacleSpawner.Instance.cooldown;
		highScore = PlayerPrefs.GetInt("HighScore", 0); 
	}
	private void Update() {
		difficulty += Time.deltaTime * difficultyFactor;
		speed += difficulty/600;
		runTime += Time.deltaTime;
		if (backgroundMaterial)
		{
			backgroundMaterial.SetVector("Direction",new Vector2(speed/10, 0f));
		}
		comboThreshold = ObstacleSpawner.Instance.cooldown;
		timeSinceLastComboInteraction += Time.deltaTime;
		if (wasInteracted) {
			timeSinceLastComboInteraction = 0f;
			comboCounter++;
		}
		else if(timeSinceLastComboInteraction>=comboThreshold) {
			comboCounter = 0;
			timeSinceLastComboInteraction = 0f;
		}
	}

	public void ComboInteraction() { wasInteracted = true; }

	public void AddPoints(int points)
	{
		currentScore += points;
		if(currentScore > highScore)
		{
			PlayerPrefs.SetInt("HighScore", currentScore);
			highScore = currentScore;
		}
	}
	public int GetCurrentScore()
	{
		return currentScore;
	}

	public void SetGameState(GameState state)
	{
		switch (state)
		{
			case GameState.running:
				if(gameState != GameState.paused && gameState != GameState.running)
				{
					difficulty = 1;
					runTime = 0;
					comboCounter = 0;
					comboThreshold = ObstacleSpawner.Instance.cooldown;
					highScore = PlayerPrefs.GetInt("HighScore", 0); 
				}
				gameState = GameState.running;
				break;
			case GameState.paused:
				break;
			case GameState.lose:
				Time.timeScale = 0;
				if (player)
				{
					// 
				}
				break;
		}
	}
}
