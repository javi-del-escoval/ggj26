using UnityEngine;

public class Furniture : Obstacle
{
	public void BePhased(bool phase)
	{
		wasInteracted = true;
		_collider.enabled = phase;
	}

	public void BeDodged(bool dodge)
	{
		wasInteracted = true;
		_collider.enabled = dodge;
	}
}
