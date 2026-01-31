using UnityEngine;

public class Obstacle : MonoBehaviour
{
	[SerializeField] BoolEventListener[] listeners;
	[SerializeField] IntEvent onScore;
	[SerializeField] protected Collider2D _collider;
	protected bool wasInteracted { get; set; }
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start() {
		_collider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			GameManager.Instance.SetGameState(GameManager.GameState.lose);
		}
	}

	public void SetFrontLine(bool frontLine) {
		foreach(BoolEventListener listener in listeners) {
			listener.enabled = frontLine;
		}
	}

	public void SetOffLine(bool offLine) {
		foreach(BoolEventListener listener in listeners) {
			listener.enabled = offLine;
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
	
}
