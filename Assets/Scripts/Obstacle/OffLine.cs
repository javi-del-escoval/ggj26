using UnityEngine;

public class OffLine : MonoBehaviour
{
	public IntEvent onScore;
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			Obstacle obs = other.transform.parent.gameObject.GetComponent<Obstacle>();
			if (obs) {
				obs.SetOffLine();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			ObstacleSpawner.Instance.RemoveObstacle(other.gameObject);
		}
	}
}
