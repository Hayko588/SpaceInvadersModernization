using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
	[SerializeField] private Transform _projectileSpawnLocation;
	[SerializeField] private Rigidbody _body;

	private int _health = 3;
	private Vector2 _lastInput;
	private bool _hasInput = false;

	float _fireInterval = 0.4f;
	private float _fireTimer = 0.0f;

	private Action<int> _updateHealth;
	private Action<ProjectileOwner, Vector3> _launchProjectile;
	private Action _onDie;

	public void Init(Action<int> updateHealth,
		Action<ProjectileOwner, Vector3> launchProjectile,
		Action onDie) {
		_updateHealth = updateHealth;
		_launchProjectile = launchProjectile;
		_onDie = onDie;
		
		_updateHealth?.Invoke(_health);
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) _hasInput = true;
		if (Input.GetMouseButtonUp(0)) _hasInput = false;
		if (Input.GetMouseButton(0)) {
			_lastInput = Input.mousePosition;
		}


		_fireTimer += Time.deltaTime;
		if (_fireTimer >= _fireInterval) {
			_launchProjectile?.Invoke(ProjectileOwner.Player, _projectileSpawnLocation.position);
			_fireTimer -= _fireInterval;
		}
	}

	public void Hit() {
		_health--;
		_updateHealth?.Invoke(_health);

		if (_health <= 0) {
			_onDie?.Invoke();
		}
	}

	private void FixedUpdate() {
		if (_hasInput) {
			Vector2 pos = _lastInput;
			const float playAreaMin = -3f;
			const float playAreaMax = 3f;

			var p = pos.x / Screen.width;
			_body.MovePosition(new Vector3(Mathf.Lerp(playAreaMin, playAreaMax, p), 0.0f, 0.0f));
		}
	}

	public void AddPowerUp(PowerUp.PowerUpType type) {

		if (type == PowerUp.PowerUpType.FIRE_RATE) {
			_fireInterval *= 0.9f;
		}
	}
}
