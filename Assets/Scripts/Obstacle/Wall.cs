using UnityEngine;

public class Wall : Obstacle
{
	public void TakeDamage(bool destroy)
	{
		if(destroy) {
			wasInteracted = true;
			_collider.enabled = false;
			Debug.Log($"rammed {gameObject.name}");
		}
	}
	public void BePhased(bool phase)
	{
		wasInteracted = true;
		_collider.enabled = phase;
	}
}
