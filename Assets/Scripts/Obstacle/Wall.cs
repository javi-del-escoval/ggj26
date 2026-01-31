using UnityEngine;

public class Wall : Obstacle
{
	public void TakeDamage(bool destroy)
	{
		if(destroy) {
			wasInteracted = true;
			//play dead
		}
	}
	public void BePhased(bool phase)
	{
		wasInteracted = true;
		_collider.enabled = phase;
	}
}
