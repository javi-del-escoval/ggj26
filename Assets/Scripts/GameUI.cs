using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
	public TMP_Text scoreText, timerText;

	private void Update() {
		TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.runTime);
		timerText.text = timeSpan.ToString(@"mm\:ss");
	}
	public void AddPoints(int points)
	{
		GameManager.Instance.AddPoints(points);
		scoreText.text = $"Score: {GameManager.Instance.GetCurrentScore()}";
	}
}
