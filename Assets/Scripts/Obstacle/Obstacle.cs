using UnityEngine;

public class Obstacle : MonoBehaviour
{
	[SerializeField] BoolEventListener[] listeners;
	[SerializeField] IntEvent onScore;
	[SerializeField] protected Collider2D _collider;
	protected bool wasInteracted { get; set; }
	
	private void Start() {
		_collider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			Debug.Log("Lose");
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
