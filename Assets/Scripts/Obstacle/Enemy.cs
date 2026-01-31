using UnityEngine;

public class Enemy : Obstacle
{
	public void TakeDamage(bool destroy)
	{
		if(destroy) {
			wasInteracted = true;
			//play dead
		}
	}
	public void BeDodged(bool dodge)
	{
		wasInteracted = true;
		_collider.enabled = dodge;
	}
}
