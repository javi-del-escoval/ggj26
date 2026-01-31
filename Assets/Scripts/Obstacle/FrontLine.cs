using UnityEngine;

public class FrontLine : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			Obstacle obs = other.gameObject.GetComponent<Obstacle>();
			if (obs) {
				obs.SetFrontLine(true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Obstacle")) {
			Obstacle obs = other.gameObject.GetComponent<Obstacle>();
			if (obs) {
				obs.SetOffLine(true);
			}
		}
	}
}
