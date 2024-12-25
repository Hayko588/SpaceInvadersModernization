using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum ProjectileOwner {
	Player,
	Enemy
}

public class Projectile : MonoBehaviour {
	public class Pool : MemoryPool<Projectile> {
		protected override void OnSpawned(Projectile item) {
			item.gameObject.SetActive(true);
		}
		protected override void OnDespawned(Projectile item) {
			item.gameObject.SetActive(false);
		}
	}

	[SerializeField] private TrailRenderer _trailRenderer;
	[SerializeField] private float _playerProjectileSpeed = 5.0f;
	[SerializeField] private float _enemyProjectileSpeed = 3.5f;
	[SerializeField] private Gradient _playerProjectileColor;
	[SerializeField] private Gradient _enemyProjectileColor;

	private int _damage = 1;
	private float _speed = 0.0f;
	private Vector3 _direction;
	private Action<Projectile> _onExplosion;

	public ProjectileOwner Owner { get; private set; }

	public void Init(ProjectileOwner projectileOwner, Vector3 position, Action<Projectile> onExplosion) {
		Owner = projectileOwner;
		_onExplosion = onExplosion;
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
		gameObject.SetActive(true);
	}

	void Update() {

		var p = transform.position;
		p += _direction * (_speed * Time.deltaTime);
		transform.position = p;
	}

	private void OnTriggerEnter(Collider other) {

		bool destroy = false;
		var enemy = other.GetComponent<Enemy>();
		if (enemy != null) {

			enemy.Hit(_damage);
			destroy = true;
		}
		else {
			var player = other.GetComponent<Player>();
			if (player != null) {

				player.Hit();
				destroy = true;
			}
		}

		if (destroy) {
			gameObject.SetActive(false);
			_onExplosion?.Invoke(this);
		}
	}
}
