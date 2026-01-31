using UnityEngine;

public class Wall : MonoBehaviour
{
	[SerializeField] Collider2D _collider;

	private void Start() {
		_collider = GetComponent<Collider2D>();
	}
	public void TakeDamage(bool destroy)
	{
		if(destroy) {
			Destroy(gameObject);
		}
	}
	public void BePhased(bool phase)
	{
		_collider.enabled = phase;
	}
}
