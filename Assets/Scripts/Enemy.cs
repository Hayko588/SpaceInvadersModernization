using System;
using System.Linq;
using SpaceInvaders.Services;
using UnityEngine;
using Zenject;
using PrimeTween;
using Random = UnityEngine.Random;

namespace SpaceInvaders {
	[RequireComponent(typeof(Rigidbody))]
	public class Enemy : MonoBehaviour {
		public class Pool : MonoMemoryPool<Vector3, Enemy> {
			protected override void OnDespawned(Enemy item) {
				item.Stop();
				base.OnDespawned(item);
			}
			protected override void Reinitialize(Vector3 pos, Enemy item) {
				item.transform.position = pos;
				item.Run();
			}
		}

		private const int TARGET_Y = -3;
		[SerializeField] private PowerUp _prefabPowerUp;
		[SerializeField] private Rigidbody _body;
		[SerializeField] private float _duration = 7.0f;

		private float _powerUpSpawnChance = 0.1f;
		private int _health = 2;
		private bool _canMove = false;
		private bool _canFire = false;
		private float _fireInterval = 2.5f;
		private float _fireTimer = 0.0f;

		private PoolService _poolService;
		private Tween _tween;

		public void Init(PoolService poolService) {
			_poolService = poolService;
		}

		private void Run() {
			_canMove = true;
			_canFire = Random.value < 0.4f;
			_health = 2 + Mathf.Min(Mathf.FloorToInt(Time.time / 15f), 5);
			_tween = Tween.PositionY(transform, TARGET_Y, _duration, Ease.Linear);
		}

		private void Stop() {
			_canMove = false;
			_canFire = false;
			_tween.Stop();
		}

		private void Update() {
			if (_canFire) {
				_fireTimer += Time.deltaTime;
				if (_fireTimer >= _fireInterval) {
					_poolService.SpawnProjectile(ProjectileOwner.Enemy, transform.position);
					_fireTimer -= _fireInterval;
				}
			}
		}

		// private void FixedUpdate() {
		// 	if (!_canMove) return;
		// 	var p = _body.position;
		// 	p += Vector3.down * (_speed * Time.deltaTime);
		// 	_body.MovePosition(p);
		// }

		public void Hit(int damage) {
			_health -= damage;
			if (_health <= 0) {
				_poolService.SpawnExplosion(transform.position);
				if (Random.value < _powerUpSpawnChance) {
					var powerup = Instantiate(_prefabPowerUp);
					var types = Enum.GetValues(typeof(PowerUp.PowerUpType)).Cast<PowerUp.PowerUpType>().ToList();
					powerup.SetType(types[Random.Range(0, types.Count)]);
				}
				_poolService.DespawnEnemy(this);
			}
		}
	}
}
