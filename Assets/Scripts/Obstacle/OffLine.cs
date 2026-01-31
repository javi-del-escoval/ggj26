using UnityEngine;

public class OffLine : MonoBehaviour
{
	public IntEvent onScore;
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			Obstacle obs = other.gameObject.GetComponent<Obstacle>();
			if (obs) {
				obs.SetOffLine(true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			ObstacleSpawner.Instance.RemoveObstacle(gameObject);
		}
	}
}
