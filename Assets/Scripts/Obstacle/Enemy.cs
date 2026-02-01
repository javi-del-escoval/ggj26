using UnityEngine;

public class Enemy : Obstacle
{
	public void TakeDamage(bool destroy)
	{
		if(destroy && isInteractable) {
			wasInteracted = true;
			_collider.enabled = false;
			Debug.Log($"rammed {gameObject.name}");
		}
	}
	public void BeDodged(bool dodge)
	{
		if(isInteractable){
			wasInteracted = true;
			_collider.enabled = dodge;
		}
	}
}
