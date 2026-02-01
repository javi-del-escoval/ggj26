using UnityEngine;

public class Wall : Obstacle
{
	public void TakeDamage(bool strike)
	{
		if(strike && isInteractable) {
			wasInteracted = true;
			_collider.enabled = false;
			_sprite.color = Color.red;
			_sprite.flipY = true;
		}
	}
	public void BePhased(bool phase)
	{
		if(isInteractable)
		{
			wasInteracted = true;
			_collider.enabled = false;
			_sprite.color = new Color(1f,1f,1f,0.5f);
		}
	}
}
