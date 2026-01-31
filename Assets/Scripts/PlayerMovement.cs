using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("Lane Switch")]
	public Transform ground;
	public Transform[] lanes;
	public float speed = 1f;
	public float moveThreshold = 0.02f;
	Vector2 groundOffset;
	int laneIndex;
	[SerializeField] Rigidbody2D rb;
	Vector2 targetPosition;
	bool moving = false;


	[Header("Abilities")]
	[SerializeField] float abilityDuration = .2f;
	[SerializeField] BoolEvent onDodge;
	[SerializeField] BoolEvent onStrike;
	[SerializeField] BoolEvent onPhase;
	bool isAbilityActive = false;
	float timeElapsedOnAbility = 0f;

	//Mask
	[Header("Mask")]
	[SerializeField] IntEvent onMaskChanged;
	[SerializeField] Mask mask;
	[SerializeField] float maskCooldown = .2f, timeElapsedMaskCooldown = 0f;
	bool canChangeMask = true;
	enum Mask { agile, strong, phase, harmony }

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		groundOffset = new Vector2(0f, Vector2.Distance(rb.position, ground.position));
		laneIndex = 1;
		targetPosition = lanes[laneIndex].position;
		for(int i = 0; i < lanes.Length; i++) {
			if(i != laneIndex) {
				lanes[i].gameObject.SetActive(false);
			}
		}
	}
	void FixedUpdate() {
		// Ability
		if(isAbilityActive) {
			timeElapsedOnAbility += Time.fixedDeltaTime;
			if(timeElapsedOnAbility >= abilityDuration) {
				isAbilityActive = false;
				timeElapsedOnAbility = 0f;
			}
		}
		// Lane
		if(Vector2.Distance(rb.position, targetPosition) > moveThreshold && moving) {
			Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
			rb.MovePosition(newPos);
		}
		else if(moving) {
			moving = false;
			lanes[laneIndex].gameObject.SetActive(true);
		}
		// Mask
		if(!canChangeMask) {
			timeElapsedMaskCooldown += Time.fixedDeltaTime;
			if(timeElapsedMaskCooldown >= maskCooldown) {
				canChangeMask = true;
				timeElapsedMaskCooldown = 0f;
			}
		}
	}
	public void Ability(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed && !isAbilityActive){
			Debug.Log("Ability");
			isAbilityActive = true;
			if(mask == Mask.agile) { onDodge.Invoke(true); }
			if(mask == Mask.strong) { onStrike.Invoke(true); }
			if(mask == Mask.phase) { onPhase.Invoke(true); }
		}
	}
	public void ChangeLane(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed){
			laneIndex += (int)callbackContext.ReadValue<float>();
			laneIndex = Math.Clamp(laneIndex, 0, lanes.Length-1);
			targetPosition = (Vector2)(lanes[laneIndex].position)+groundOffset;
			moving = true;
			for(int i = 0; i < lanes.Length; i++) {
				lanes[i].gameObject.SetActive(false);
			}
		}
	}
	public void ChangeMask(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed && canChangeMask) {
			Vector2 vec = callbackContext.ReadValue<Vector2>();
			mask = (vec.x, vec.y) switch
			{
				(0, 1)	=> Mask.agile, // <-- up
				(1, 0)	=> Mask.strong, // <-- right
				//(0, -1) => Mask.black, // <-- down
				(-1, 0)	=> Mask.phase, // <-- left
				_		=> 0
			};
			canChangeMask = false;
			onMaskChanged.Invoke((int)mask);
		}
	}
}
