using System;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Zenject;
using IPoolable = Zenject.IPoolable;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IPoolable {
	public class Pool : MemoryPool<Enemy> {
	}

	[SerializeField] private PowerUp _prefabPowerUp;
	[SerializeField] private Rigidbody _body;

	private float _powerUpSpawnChance = 0.1f;
	private int _health = 2;
	private float _speed = 2.0f;
	private bool _canMove = false;
	private bool _canFire = false;
	private float _fireInterval = 2.5f;
	private float _fireTimer = 0.0f;

	public Action<ProjectileOwner, Vector3> LaunchProjectile;
	public Action<Vector3> SpawnExplosion;
	public Action<Enemy> OnDestroy;

	private void Run() {
		_canMove = true;
		_canFire = Random.value < 0.4f;
		_health = 2 + Mathf.Min(Mathf.FloorToInt(Time.time / 15f), 5);
	}

	private void Stop() {
		_canMove = false;
		_canFire = false;
	}

	private void Update() {
		if (_canFire) {
			_fireTimer += Time.deltaTime;
			if (_fireTimer >= _fireInterval) {
				LaunchProjectile?.Invoke(ProjectileOwner.Enemy, transform.position);
				_fireTimer -= _fireInterval;
			}
		}
	}

	private void FixedUpdate() {
		if (!_canMove) return;
		var p = _body.position;
		p += Vector3.down * (_speed * Time.deltaTime);
		_body.MovePosition(p);
	}

	public void Hit(int damage) {
		_health -= damage;
		if (_health <= 0) {
			SpawnExplosion?.Invoke(transform.position);
			if (Random.value < _powerUpSpawnChance) {
				var powerup = Instantiate(_prefabPowerUp);
				var types = Enum.GetValues(typeof(PowerUp.PowerUpType)).Cast<PowerUp.PowerUpType>().ToList();
				powerup.SetType(types[Random.Range(0, types.Count)]);
			}
			OnDestroy?.Invoke(this);
		}
	}

	public void OnDespawned() {
		Stop();
		gameObject.SetActive(false);
	}
	public void OnSpawned() {
		Run();
		gameObject.SetActive(true);
	}
}
