using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
	public Image maskMarker;
	public TMP_Text scoreText;

	public void SetMask(int index)
	{
		Color[] colors = {new Color(1,0,0,1), new Color(0,0,1,1), new Color(1,1,0,1),new Color(0,0,0,0),  new Color(1,1,1,0)};
		if(index>=colors.Length || index < 0) {
			index = 0;
		}
		maskMarker.color = colors[index];
	}

	public void AddPoints(int points)
	{
		GameManager.Instance.AddPoints(points);
		scoreText.text = $"Score: {GameManager.Instance.GetCurrentScore()}";
	}
}
