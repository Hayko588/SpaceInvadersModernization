using System;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {
	public class Pool : MemoryPool<Enemy> {
		protected override void OnSpawned(Enemy item) {
			item.Run();
		}
		protected override void OnDespawned(Enemy item) {
			item.Stop();
		}
	}

	[SerializeField] private PowerUp _prefabPowerUp;
	[SerializeField] private Projectile _prefabProjectile;
	[SerializeField] private Rigidbody _body;

	[Inject] private readonly GameController _gameController;
	[Inject] private readonly Explosion.Pool _explosionPool;
	[Inject] private readonly GameplayUI _gameplayUI;

	private float _powerUpSpawnChance = 0.1f;
	private int _health = 2;
	private float _speed = 2.0f;
	private bool _canMove = false;
	private bool _canFire = false;
	private float _fireInterval = 2.5f;
	private float _fireTimer = 0.0f;

	public void Run() {
		_canMove = true;
		_canFire = Random.value < 0.4f;
		_health = 2 + Mathf.Min(Mathf.FloorToInt(Time.time / 15f), 5);
		gameObject.SetActive(true);
	}

	public void Stop() {
		_canMove = false;
		_canFire = false;
		gameObject.SetActive(false);
	}

	private void Update() {
		if (_canFire) {
			_fireTimer += Time.deltaTime;
			if (_fireTimer >= _fireInterval) {
				var go = Instantiate(_prefabProjectile);
				go.transform.position = transform.position;
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
			var fx = _explosionPool.Spawn();
			fx.transform.position = transform.position;

			if (Random.value < _powerUpSpawnChance) {
				var powerup = Instantiate(_prefabPowerUp);
				var types = Enum.GetValues(typeof(PowerUp.PowerUpType)).Cast<PowerUp.PowerUpType>().ToList();
				powerup.SetType(types[Random.Range(0, types.Count)]);
			}

			_gameplayUI.AddScore(1);
			_gameController.RemoveEnemy(this);
		}
	}

}
