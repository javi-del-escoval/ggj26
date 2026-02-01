using UnityEngine;

public class Enemy : Obstacle
{
	public void TakeDamage(bool strike)
	{
		Debug.Log($"Destroy {strike} & isInteractable {isInteractable}");
		if(strike && isInteractable) {
			Debug.Log($"Striked {gameObject.name}");
			wasInteracted = true;
			_collider.enabled = false;
			_sprite.color = Color.red;
			_sprite.flipY = true;
		}
	}
	public void BeDodged(bool dodge)
	{
		if(isInteractable){
			wasInteracted = true;
			_collider.enabled = false;
			_sprite.flipX = true;
		}
	}
}
