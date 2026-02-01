using UnityEngine;

public class Obstacle : MonoBehaviour
{
	[SerializeField] IntEvent onScore;
	[SerializeField] protected Collider2D _collider;
	public SpriteRenderer _sprite;
	protected bool wasInteracted { get; set; }
	[field: SerializeField]
	protected bool isInteractable { get; private set; }
	
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player") && !wasInteracted) {
			GameManager.Instance.SetGameState(GameManager.GameState.lose);
		}
	}

	public void SetFrontLine(bool frontLine) {
		isInteractable = frontLine;
	}

	public void SetOffLine() {
		int score = 1+(int)GameManager.Instance.difficulty;
		if (wasInteracted) {
			score *= 2;
			GameManager.Instance.ComboInteraction();
		}
		if (GameManager.Instance.comboCounter > 0)
		{
			score *= GameManager.Instance.comboCounter;
		}
		onScore.Invoke(score);
	}
	
}
