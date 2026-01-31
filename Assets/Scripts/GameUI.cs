using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
	public TMP_Text scoreText, timerText, lostScoreText;
	public GameObject panelLost;

	public static GameUI Instance { get; private set; }
	private void Awake() {
		if (Instance != null && Instance != this) {
			// If an instance already exists and it's not this one, destroy this duplicate
			Destroy(this.gameObject);
		}
		else {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}
	private void Update() {
		TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.runTime);
		timerText.text = timeSpan.ToString(@"mm\:ss");
	}
	public void AddPoints(int points)
	{
		GameManager.Instance.AddPoints(points);
		scoreText.text = $"Score: {GameManager.Instance.GetCurrentScore()}";
	}

	public void Lose(bool lost)
	{
		panelLost.SetActive(lost);
		lostScoreText.text = $"Score: {GameManager.Instance.GetCurrentScore()}";
	}
}
