using System;
using System.Collections;
using System.Collections.Generic;
using SpaceInvaders.Services;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SpaceInvaders {
	[RequireComponent(typeof(Rigidbody))]
	public class Player : MonoBehaviour {
		[SerializeField] private Transform _projectileSpawnLocation;
		[SerializeField] private Rigidbody _body;

		private int _health = 3;
		private Vector2 _lastInput;
		private bool _hasInput = false;

		private float _fireInterval = 0.4f;
		private float _fireTimer = 0.0f;
		private PoolService _poolService;

		public event Action OnDie;
		public event Action<int> UpdateHealth;

		public void Init(PoolService poolService) {
			_poolService = poolService;
			UpdateHealth?.Invoke(_health);
		}

		private void Update() {
			if (Input.GetMouseButtonDown(0)) _hasInput = true;
			if (Input.GetMouseButtonUp(0)) _hasInput = false;
			if (Input.GetMouseButton(0)) {
				_lastInput = Input.mousePosition;
			}

			_fireTimer += Time.deltaTime;
			if (_fireTimer >= _fireInterval) {
				_poolService.SpawnProjectile(ProjectileOwner.Player, _projectileSpawnLocation.position);
				_fireTimer -= _fireInterval;
			}
		}

		public void Hit() {
			_health--;
			UpdateHealth?.Invoke(_health);

			if (_health <= 0) {
				OnDie?.Invoke();
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
}
