using System;
using System.Collections.Generic;
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

	private List<Enemy> _enemies = new();
	private List<Projectile> _projectiles = new();

	private float _enemySpawnTimer = 0.0f;
	bool _running = true;

	private void Awake() {
		Application.targetFrameRate = 60;

		_player.OnLaunchProjectile += LaunchProjectile;
	}

	private void Start() {
		_player = Object.FindObjectOfType<Player>(true);
		_player.OnDie += OnPlayerDie;

		_running = true;
	}

	private void Update() {
		if (!_running) return;
		_enemySpawnTimer += Time.deltaTime;
		if (_enemySpawnTimer >= _enemySpawnInterval) {
			var e = CreateEnemy();

			var p = _spawnPosition + new Vector3(
				Random.Range(-_spawnOffsets.x, _spawnOffsets.x),
				Random.Range(-_spawnOffsets.y, _spawnOffsets.y));

			e.transform.position = p;
			_enemySpawnTimer -= _enemySpawnInterval;
		}
	}

	private void OnDestroy() {
		_player.OnLaunchProjectile -= LaunchProjectile;
		foreach (Enemy enemy in _enemies) {
			enemy.OnLaunchProjectile -= LaunchProjectile;
		}
	}

	private void LaunchProjectile(ProjectileOwner owner, Vector3 position) {
		if (owner == ProjectileOwner.Enemy) {
			Debug.Log("ProjectileOwner.Enemy");
		}
		var p = _projectilePool.Spawn();
		p.Init(owner, position, RemoveProjectile);
		_projectiles.Add(p);
	}

	private void RemoveProjectile(Projectile projectile) {
		_projectilePool.Despawn(projectile);
		_projectiles.Remove(projectile);
	}

	private void OnPlayerDie() {
		_running = false;
	}

	private Enemy CreateEnemy() {
		var e = _enemyPool.Spawn();
		e.OnLaunchProjectile += LaunchProjectile;
		_enemies.Add(e);
		return e;
	}

	public void RemoveEnemy(Enemy enemy) {
		enemy.OnLaunchProjectile -= LaunchProjectile;
		_enemyPool.Despawn(enemy);
		_enemies.Remove(enemy);
	}
}
