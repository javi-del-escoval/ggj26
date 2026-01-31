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


	[Header("Jump")]
	public float jumpForce = 0.5f;
	[SerializeField] bool isGrounded;
	[SerializeField] LayerMask whatIsGround;
	
	[Header("Slide")]
	[SerializeField] Collider2D headCollider;

	[Header("Phase")]
	[SerializeField] float phaseDuration = 1f;
	[SerializeField] Collider2D feetCollider;
	bool phasing = false;
	float timeElapsedPhasing = 0f;

	//Mask
	[Header("Mask")]
	[SerializeField] Mask mask;
	public enum Mask { none, red, blue, yellow, black }

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
		// Jump
		bool wasGrounded = isGrounded;
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(ground.position, moveThreshold, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
			}
		}
		// Phase
		if(phasing) {
			timeElapsedPhasing += Time.fixedDeltaTime;
			if(timeElapsedPhasing >= phaseDuration) {
				phasing = false;
				timeElapsedPhasing = 0f;
				headCollider.enabled = true;
				feetCollider.enabled = true;
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
	}

	public void Jump(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed && (mask == Mask.red || mask == Mask.yellow) && isGrounded){
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        	isGrounded = false;
		}
	}
	public void Slide(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed && mask == Mask.red){
			headCollider.enabled = false;
			Debug.Log("Slide");
		}
		else if (callbackContext.canceled) {
			headCollider.enabled = true;
		}
	}
	public void Phase(InputAction.CallbackContext callbackContext) {
		if(callbackContext.performed && (mask == Mask.blue || mask == Mask.yellow) && !phasing){
			Debug.Log("Phase");
			headCollider.enabled = false;
			feetCollider.enabled = false;
			phasing = true;
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
		void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
		}
	}
}
