using UnityEngine;

public class GameManager : MonoBehaviour
{
	int currentScore, highScore;
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
	private void Start() {
		highScore = PlayerPrefs.GetInt("HighScore", 0); 
	}
}
