using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public Transform ground;
	Vector2 offset;
	public Transform[] lanes;
	[SerializeField] int laneIndex;
	Rigidbody2D rb;
	Vector2 targetPosition;
	public float moveThreshold = 0.02f;
	bool moving = false;

	public enum Mask { none, red, blue, yellow, black }

	[SerializeField] Mask mask;

	public float speed = 1f;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		offset = new Vector2(0f, Vector2.Distance(transform.position, ground.position));
		laneIndex = 1;
		targetPosition = lanes[laneIndex].position;
		for(int i = 0; i < lanes.Length; i++) {
			if(i != laneIndex) {
				lanes[i].gameObject.SetActive(false);
			}
		}
	}
	void Update() { }
    void FixedUpdate() {
		if(Vector2.Distance(targetPosition, ground.position) > moveThreshold && moving) {
			Vector2 newPos = Vector2.MoveTowards(rb.position+offset, targetPosition, speed * Time.fixedDeltaTime);
			rb.MovePosition(newPos);
		}
		else if(moving) {
			moving = false;
			lanes[laneIndex].gameObject.SetActive(true);
			for(int i = 0; i < lanes.Length; i++) {
				if(i != laneIndex) {
					lanes[i].gameObject.SetActive(false);
				}
			}
		}
    }

	public void Jump(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed){
			Debug.Log("Jump");
		}
	}
	public void Slide(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed){
			Debug.Log("Slide");
		}
	}
	public void ActivateAbility(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed){
			Debug.Log("Ability");
		}
	}
	public void ChangeLane(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed){
			laneIndex += (int)callbackContext.ReadValue<float>();
			laneIndex = (laneIndex % lanes.Length + lanes.Length) % lanes.Length;
			targetPosition = lanes[laneIndex].position;
			for(int i = 0; i < lanes.Length; i++) {
				lanes[i].gameObject.SetActive(false);
			}
			moving = true;
		}
	}
	public void ChangeMask(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed) {
			Vector2 vec = callbackContext.ReadValue<Vector2>();
			mask = (vec.x, vec.y) switch
			{
				(0, 1) => Mask.red, // <-- up
				(1, 0) => Mask.blue, // <-- right
				//(0, -1) => Mask.black, // <-- down
				(-1, 0) => Mask.yellow, // <-- left
				_      => Mask.none
			};
		}
	}
}
