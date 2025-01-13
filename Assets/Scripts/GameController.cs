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
			_player.Init(_poolService);
			_player.OnDie += OnPlayerDie;
			_player.UpdateHealth += UpdateHealth;
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

		private void OnDestroy() {
			_player.OnDie -= OnPlayerDie;
		}

		private void UpdateHealth(int health) {
			_gameplayUI.UpdateHealth(health);
		}

		private void SpawnExplosion(Vector3 position) {
			_poolService.SpawnExplosion(position);
		}

		private void SpawnProjectile(ProjectileOwner owner, Vector3 position) {
			_poolService.SpawnProjectile(owner, position);
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
			var position = _spawnPosition + Random.Range(-_spawnOffsets.x, _spawnOffsets.x) * Vector3.right;
			_poolService.SpawnEnemy(position, AddScore);
		}

		private void AddScore(Enemy enemy) {
			_gameplayUI.AddScore(1);
		}
	}
}
