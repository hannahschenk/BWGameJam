using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for scripts that wish to be notified when a hitbox hits with a collider
/// </summary>
public interface IHitboxResponder
{
	void CollidedWith(Collider collider);
}

public class Hitbox : MonoBehaviour
{

	public enum ColliderState
	{
		Inactive,
		Active,
		Colliding
	}
	private ColliderState _state;

	public LayerMask mask;
	public bool useSphere = false;
	public Vector3 position = Vector3.zero;
	public Vector3 rotation = Vector3.zero;
	public Vector3 hitboxSize = Vector3.one;
	public float radius = 0.5f;
	public Color inactiveColor = Color.gray;
	public Color collisionOpenColor = Color.red;
	public Color collidingColor = Color.magenta;

	private IHitboxResponder _responder = null;

	private Transform _transform;
	public Transform Transform
	{
		get
		{
			if (_transform == null)
				_transform = transform;
			return _transform;
		}
	}

	//local position of the hitbox, offset by half-extents instead of being centered on the transform
	protected Vector3 _lastSize = new Vector3(-1f, -1f, -1f);
	protected Vector3 _lastPos = new Vector3(-1f, -1f, -1f);
	protected Vector3 _offsetLocal = Vector3.zero;
	protected Vector3 offsetLocalPosition
	{
		get
		{
			if (_lastSize != hitboxSize || _lastPos != position) { //If we've changed size or local position...
				if (!useSphere) {
					_offsetLocal.x = position.x;
					_offsetLocal.y = position.y + (hitboxSize.y / 2f);
					_offsetLocal.z = position.z + (hitboxSize.z / 2f);
				} else {
					_offsetLocal.x = position.x;
					_offsetLocal.y = position.y + radius;
					_offsetLocal.z = position.z + radius;
				}
			}
			return _offsetLocal;
		}
	}

	public Vector3 worldPosition
	{
		get
		{
			//return Transform.TransformPoint(position);
			return Transform.TransformPoint(offsetLocalPosition);
		}
	}

	//Hitbox rotates with Transform; apply additional rotation through quaternion multiplication
	public Quaternion worldRotation
	{
		get
		{
			return Transform.rotation * Quaternion.Euler(rotation);
		}
	}

	public void StartCheckingCollision()
	{
		_state = ColliderState.Active;
	}

	public void StopCheckingCollision()
	{
		_state = ColliderState.Inactive;
	}

	//protected void UpdateHitbox()
	protected void Update()
	{
		if (_state == ColliderState.Inactive) 
			return;
		CheckCollision();
	}

	public void CheckCollision()
	{
		Collider[] colliders;

		if (!useSphere) {
			colliders = Physics.OverlapBox(worldPosition, hitboxSize, worldRotation, mask);
		} else {
			colliders = Physics.OverlapSphere(worldPosition, radius);
		}

		for (int i = 0; i < colliders.Length; i++) {
			Collider aCollider = colliders[i];
			_responder?.CollidedWith(aCollider);
		}

		if (_state != ColliderState.Inactive)
			_state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Active;

	}

	//e.g., the script that gets notified when this hitbox collides with anything.
	public void Register(IHitboxResponder responder)
	{
		_responder = responder;
	}

	public void Die()
	{
		_state = ColliderState.Inactive;
	}

#if UNITY_EDITOR
	protected void CheckGizmoColor()
	{
		switch (_state) {
			case ColliderState.Inactive:
				Gizmos.color = inactiveColor;
				break;
			case ColliderState.Active:
				Gizmos.color = collisionOpenColor;
				break;
			case ColliderState.Colliding:
				Gizmos.color = collidingColor;
				break;
		}
	}

	public void OnDrawGizmosSelected()
	{
		CheckGizmoColor();
		//Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		Gizmos.matrix = Matrix4x4.TRS(transform.position, worldRotation, transform.localScale);
		//Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2)); // Because size is halfExtents
		if (!useSphere) {
			Gizmos.DrawWireCube(offsetLocalPosition, hitboxSize);
		} else {
			Gizmos.DrawSphere(offsetLocalPosition, radius);
		}
	}
#endif

}
