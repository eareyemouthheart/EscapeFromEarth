using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController {
	
	float maxClimbAngle = 80;
	float maxDescendAngle = 80;
	float checkGroundAheadLength = 0.35f;
	
	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
	[HideInInspector]
	public bool HandlePhysic = true;
	[HideInInspector]
	public bool inverseGravity = false;

	[ReadOnly] public bool ownerIsPlayer = false;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;
	}
	
	public void Move(Vector3 velocity, bool standingOnPlatform) {
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false, bool isPlayer = false) {
		ownerIsPlayer = isPlayer;
		CalculateRaySpacing ();
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;
		playerInput = input;

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope (ref velocity);
		}

		if (HandlePhysic) {
			HorizontalCollisions (ref velocity);
			if (velocity.y != 0) {
				VerticalCollisions (ref velocity);
			}
		}

		CheckGroundedAhead (velocity);

        transform.Translate (velocity,Space.World);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		if (Mathf.Abs(velocity.x) < skinWidth) {
			rayLength = 5*skinWidth;
		}
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += (Vector2) transform.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLength, collisionMaskHorizontal);

			Debug.DrawRay(rayOrigin, transform.right * directionX * rayLength,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

                if (ownerIsPlayer && hit.collider.gameObject.CompareTag("Through"))
                {
					continue;
                }

				float slopeAngle = Vector2.Angle (hit.normal, transform.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;

					collisions.ClosestHit = hit;
				}

                if (playerInput.x == 0 && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    velocity.x = 0;
			}
		}
	}
	
	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += (Vector2) transform.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, transform.up * directionY * rayLength,Color.red);

			if (hit) {
				if (hit.collider.tag == "Through")
				{
					if(ownerIsPlayer)
					if ((directionY == (inverseGravity ? -1 : 1) || hit.distance == 0) && (!ownerIsPlayer
							||( ownerIsPlayer && velocity.y > 0 && GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
							 || (ownerIsPlayer && !GameManager.Instance.Player.playerCheckLadderZone.isClimbing)))
					{
						continue;
					}

					if (collisions.fallingThroughPlatform)
					{
						continue;
					}

					if (playerInput.y == -1)
					{
						if (ownerIsPlayer)
						{
							if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
							{
								if (!GameManager.Instance.Player.playerCheckLadderZone.isLadderBelow)
								{
									
									GameManager.Instance.Player.input.y = 0;
									collisions.below = true;
									GameManager.Instance.Player.transform.position += Vector3.up * 0.03f;
									GameManager.Instance.Player.playerCheckLadderZone.isClimbing = false;
									Debug.LogError("1");
									isFallingOutTheLadder = true;
									Invoke("ResetFallOutLadder", .1f);
								}
								continue;
							}
							else if(!isFallingOutTheLadder)
							{
								collisions.fallingThroughPlatform = true;
								Invoke("ResetFallingThroughPlatform", .25f);
								continue;
							}
						}
					}
				}
				else if ((GameManager.Instance.Player.playerCheckLadderZone.isClimbing && GameManager.Instance.Player.playerCheckLadderZone.currentLadder
					&& !GameManager.Instance.Player.playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>()) && GetComponent<Player>() != null)
				{
                    if (ownerIsPlayer)
					{
						if (playerInput.y == 1 && GameManager.Instance.Player.playerCheckLadderZone.isLadderAbove)
                        {
							continue;
						}else if (playerInput.y == -1 && GameManager.Instance.Player.playerCheckLadderZone.isLadderBelow)
						{
							continue;
						}else if (playerInput.y == -1 && !GameManager.Instance.Player.playerCheckLadderZone.isLadderBelow)
                        {
                            GameManager.Instance.Player.input.y = 0;
                            collisions.below = true;
                            GameManager.Instance.Player.transform.position += Vector3.up * 0.03f;
							GameManager.Instance.Player.playerCheckLadderZone.isClimbing = false;
						}
					}
				}

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				if (!inverseGravity) {
					collisions.below = directionY == -1;
					collisions.above = directionY == 1;
				} else {
					collisions.below = directionY == 1;
					collisions.above = directionY == -1;
				}


				collisions.ClosestHit = hit;
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + (Vector2) transform.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,transform.right * directionX,rayLength,collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,transform.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void CheckGroundedAhead(Vector3 velocity){
		float directionX = collisions.faceDir;

		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.down, checkGroundAheadLength, collisionMask);

		Debug.DrawRay (rayOrigin, Vector2.down * checkGroundAheadLength, Color.green);


	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	bool isFallingOutTheLadder = false;
	void ResetFallOutLadder()
    {
		isFallingOutTheLadder = false;

	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public bool isGrounedAhead(bool isFacingRight)
	{
		float directionX = isFacingRight ? 1 : -1;

		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

		RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, checkGroundAheadLength, collisionMask);

		Debug.DrawRay(rayOrigin, Vector2.down * checkGroundAheadLength, Color.blue);

		if (hits.Length > 0)
		{
			bool hitGround = false;
			foreach (var hit in hits)
			{
				if (hit && hit.collider.gameObject != gameObject)
					hitGround = true;
			}
			return hitGround;
		}
		else
			return false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public RaycastHit2D ClosestHit;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
