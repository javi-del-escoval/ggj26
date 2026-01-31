using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] Collider2D _collider;
	[SerializeField] BoolEventListener[] listeners;

	private void Start() {
		_collider = GetComponent<Collider2D>();
	}
	public void TakeDamage(bool destroy)
	{
		if(destroy) {
			Destroy(gameObject);
		}
	}
	public void BeDodged(bool dodge)
	{
		_collider.enabled = dodge;
	}
	public void SetFrontLine(bool frontLine)
	{
		foreach(BoolEventListener listener in listeners)
		{
			listener.enabled = frontLine;
		}
	}
}
