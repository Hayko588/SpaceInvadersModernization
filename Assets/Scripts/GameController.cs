using System;
using System.Collections.Generic;
using SpaceInvaders.Services;
using SpaceInvaders.UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SpaceInvaders {
	public class GameController : MonoBehaviour {
		[SerializeField] private Player _player;
		[SerializeField] private Vector3 _spawnPosition;
		[SerializeField] private Vector3 _spawnOffsets;

		[SerializeField] private float _enemySpawnInterval = 0.5f;


		[Inject] private readonly PoolService _poolService;
		[Inject] private readonly GameplayUI _gameplayUI;
		[Inject] private readonly GameOverUI _gameOverUI;

		private float _enemySpawnTimer = 0.0f;
		bool _running = true;

		private void Awake() {
			Application.targetFrameRate = 60;
		}

		private void Start() {
			_player.Init(UpdateHealth, SpawnProjectile, OnPlayerDie);
			_running = true;
		}

		private void Update() {
			if (!_running) return;
			_enemySpawnTimer += Time.deltaTime;
			if (_enemySpawnTimer >= _enemySpawnInterval) {
				SpawnEnemy();
				_enemySpawnTimer -= _enemySpawnInterval;
			}
		}

		private void UpdateHealth(int health) {
			_gameplayUI.UpdateHealth(health);
		}

		private void SpawnExplosion(Vector3 position) {
			_poolService.SpawnExplosion(position, DespawnExplosion);
		}

		private void DespawnExplosion(Explosion explosion) {
			_poolService.DespawnExplosion(explosion);
		}

		private void SpawnProjectile(ProjectileOwner owner, Vector3 position) {
			_poolService.SpawnProjectile(owner, position, DespawnProjectile);
		}

		private void DespawnProjectile(Projectile projectile) {
			_poolService.DespawnProjectile(projectile);
		}

		private void OnPlayerDie() {
			_running = false;
			SpawnExplosion(_player.transform.position);
			Destroy(_player.gameObject);
			GameOver();
		}

		private void GameOver() {
			_gameOverUI.Open();
			// ClearPool(_enemyPool);
			// _projectiles.ForEach(_ => _projectilePool.Clear());
			// _explosions.ForEach(_ => _explosionPool.Clear());
		}

		private void SpawnEnemy() {
			var position = _spawnPosition + new Vector3(
				Random.Range(-_spawnOffsets.x, _spawnOffsets.x),
				Random.Range(-_spawnOffsets.y, _spawnOffsets.y));
			_poolService.SpawnEnemy(position, AddScore);
		}

		private void AddScore(Enemy enemy) {
			_gameplayUI.AddScore(1);
		}
	}
}
