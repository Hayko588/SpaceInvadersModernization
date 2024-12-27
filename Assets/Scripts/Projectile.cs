using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceInvaders {
	public enum ProjectileOwner {
		Player,
		Enemy
	}

	public class Projectile : MonoBehaviour, IPoolable {
		public class Pool : MemoryPool<Projectile> {
		}

		[SerializeField] private TrailRenderer _trailRenderer;
		[SerializeField] private float _playerProjectileSpeed = 5.0f;
		[SerializeField] private float _enemyProjectileSpeed = 3.5f;
		[SerializeField] private Gradient _playerProjectileColor;
		[SerializeField] private Gradient _enemyProjectileColor;
		[SerializeField] private float _lifetime = 5f;

		private int _damage = 1;
		private float _speed = 0.0f;
		private float _currentLifetime;
		private Vector3 _direction;
		private Action<Projectile> _despawn;
		private bool _isStopped;

		public ProjectileOwner Owner { get; private set; }

		public void Init(ProjectileOwner projectileOwner, Vector3 position, Action<Projectile> despawn) {
			Owner = projectileOwner;
			_despawn = despawn;
			_currentLifetime = _lifetime;
			switch (Owner) {
				case ProjectileOwner.Player:
					_direction = Vector3.up;
					_speed = _playerProjectileSpeed;
					_trailRenderer.colorGradient = _playerProjectileColor;
					break;
				case ProjectileOwner.Enemy:
					_direction = Vector3.down;
					_speed = _enemyProjectileSpeed;
					_trailRenderer.colorGradient = _enemyProjectileColor;
					break;
			}
			transform.position = position;
			_isStopped = false;
		}


		private void Update() {
			if (_isStopped) {
				return;
			}
			var p = transform.position;
			p += _direction * (_speed * Time.deltaTime);
			transform.position = p;
			_currentLifetime -= Time.deltaTime;
			if (_currentLifetime <= 0f) {
				_despawn?.Invoke(this);
				_isStopped = true;
			}
		}

		private void OnTriggerEnter(Collider other) {
			bool destroy = false;
			switch (Owner) {
				case ProjectileOwner.Player:
					var enemy = other.GetComponent<Enemy>();
					if (enemy != null) {
						enemy.Hit(_damage);
						destroy = true;
					}
					break;
				case ProjectileOwner.Enemy:
					var player = other.GetComponent<Player>();
					if (player != null) {
						player.Hit();
						destroy = true;
					}
					break;
			}

			if (destroy) {
				_despawn?.Invoke(this);
			}
		}

		public void OnDespawned() {
			gameObject.SetActive(false);
		}
		public void OnSpawned() {
			gameObject.SetActive(true);
		}
	}
}
