using UnityEngine;

public class Furniture : MonoBehaviour
{
	[SerializeField] Collider2D _collider;

	private void Start() {
		_collider = GetComponent<Collider2D>();
	}

	public void BePhased(bool phase)
	{
		_collider.enabled = phase;
	}

	public void BeDodged(bool dodge)
	{
		_collider.enabled = dodge;
	}
}
