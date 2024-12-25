using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour {
	[SerializeField] private Player _player;
	[SerializeField] private Vector3 _spawnPosition;
	[SerializeField] private Vector3 _spawnOffsets;
	[SerializeField] private float _enemySpawnInterval = 0.5f;

	[Inject] private readonly Enemy.Pool _enemyPool;

	private List<Enemy> _enemies = new();

	private float _enemySpawnTimer = 0.0f;
	bool _running = true;

	private void Awake() {
		Application.targetFrameRate = 60;
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

	private void OnPlayerDie() {
		_running = false;
	}

	private Enemy CreateEnemy() {
		var e = _enemyPool.Spawn();
		_enemies.Add(e);
		return e;
	}

	public void RemoveEnemy(Enemy enemy) {
		_enemyPool.Despawn(enemy);
		_enemies.Remove(enemy);
	}
}
