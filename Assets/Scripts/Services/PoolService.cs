using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceInvaders.Services {
	public class PoolService {
		private readonly HashSet<Enemy> _enemies = new();
		private readonly HashSet<Projectile> _projectiles = new();
		private readonly HashSet<Explosion> _explosions = new();

		[Inject] private readonly Enemy.Pool _enemyPool;
		[Inject] private readonly Projectile.Pool _projectilePool;
		[Inject] private readonly Explosion.Pool _explosionPool;

		public void SpawnExplosion(Vector3 position, Action<Explosion> _onDestroy) {
			var ex = _explosionPool.Spawn();
			ex.Init(position, _onDestroy + DespawnExplosion);
			_explosions.Add(ex);
		}

		public void DespawnExplosion(Explosion explosion) {
			_explosionPool.Despawn(explosion);
		}

		public void SpawnProjectile(ProjectileOwner owner, Vector3 position, Action<Projectile> onDestroy) {
			var p = _projectilePool.Spawn();
			p.Init(owner, position, onDestroy + DespawnProjectile);
			_projectiles.Add(p);
		}

		public void DespawnProjectile(Projectile projectile) {
			_projectilePool.Despawn(projectile);
		}

		public void SpawnEnemy(Vector3 position, Action<Enemy> onDestroy) {
			var e = _enemyPool.Spawn();
			e.transform.position = position;
			if (!_enemies.Contains(e)) {
				e.LaunchProjectile = SpawnProjectile;
				e.SpawnExplosion = SpawnExplosion;
				e.OnDestroy = enemy => {
					onDestroy?.Invoke(enemy); 
					DespawnEnemy(enemy);
				};
			}
			_enemies.Add(e);
		}

		private void DespawnEnemy(Enemy enemy) {
			_enemyPool.Despawn(enemy);
		}
	}
}
