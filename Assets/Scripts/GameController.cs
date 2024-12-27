using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
	[SerializeField] private Player _player;
	[SerializeField] private Vector3 _spawnPosition;
	[SerializeField] private Vector3 _spawnOffsets;

	[SerializeField] private float _enemySpawnInterval = 0.5f;

	[Inject] private readonly Enemy.Pool _enemyPool;
	[Inject] private readonly Projectile.Pool _projectilePool;
	[Inject] private readonly Explosion.Pool _explosionPool;
	[Inject] private readonly GameplayUI _gameplayUI;
	[Inject] private readonly GameOverUI _gameOverUI;

	private List<Enemy> _enemies = new();
	private List<Projectile> _projectiles = new();
	private List<Explosion> _explosions = new();

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
		var ex = _explosionPool.Spawn();
		ex.transform.position = position;
		ex.Init(DespawnExplosion);
		_explosions.Add(ex);
	}

	private void DespawnExplosion(Explosion explosion) {
		_explosionPool.Despawn(explosion);
		_explosions.Remove(explosion);
	}

	private void SpawnProjectile(ProjectileOwner owner, Vector3 position) {
		if (owner == ProjectileOwner.Enemy) {
			Debug.Log("ProjectileOwner.Enemy");
		}
		var p = _projectilePool.Spawn();
		p.Init(owner, position, DespawnProjectile);
		_projectiles.Add(p);
	}

	private void DespawnProjectile(Projectile projectile) {
		_projectilePool.Despawn(projectile);
		_projectiles.Remove(projectile);
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
		var e = _enemyPool.Spawn();
		e.LaunchProjectile = SpawnProjectile;
		e.SpawnExplosion = SpawnExplosion;
		e.OnDestroy = DestroyEnemy;

		var p = _spawnPosition + new Vector3(
			Random.Range(-_spawnOffsets.x, _spawnOffsets.x),
			Random.Range(-_spawnOffsets.y, _spawnOffsets.y));
		e.transform.position = p;
		_enemies.Add(e);
	}

	private void DestroyEnemy(Enemy enemy) {
		_gameplayUI.AddScore(1);
		DespawnEnemy(enemy);
	}

	private void DespawnEnemy(Enemy enemy) {
		_enemyPool.Despawn(enemy);
		_enemies.Remove(enemy);
	}

	private void ClearPool<T>(T pool) where T : MemoryPool<IPoolable> {
		pool.Clear();
	}
}
