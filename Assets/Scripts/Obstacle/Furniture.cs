using UnityEngine;

public class Furniture : Obstacle
{
	public void BePhased(bool phase)
	{
		if(isInteractable){
			wasInteracted = true;
			_collider.enabled = false;
			_sprite.color = new Color(1f,1f,1f,0.5f);
		}
	}

	public void BeDodged(bool dodge)
	{
		if(isInteractable){
			wasInteracted = true;
			_collider.enabled = false;
		}
	}
}
