using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceInvaders.Services {
	public class PoolService {
		[Inject] private readonly Enemy.Pool _enemyPool;
		[Inject] private readonly Projectile.Pool _projectilePool;
		[Inject] private readonly Explosion.Pool _explosionPool;

		public void SpawnExplosion(Vector3 position) {
			var ex = _explosionPool.Spawn();
			ex.Init(position, DespawnExplosion);
		}

		public void DespawnExplosion(Explosion explosion) {
			_explosionPool.Despawn(explosion);
		}

		public void SpawnProjectile(ProjectileOwner owner, Vector3 position) {
			var p = _projectilePool.Spawn(owner, position);
			p.Init(owner, position, DespawnProjectile);
		}

		public void DespawnProjectile(Projectile projectile) {
			_projectilePool.Despawn(projectile);
		}

		public void SpawnEnemy(Vector3 position, Action<Enemy> onDestroy) {
			var e = _enemyPool.Spawn(position);
			e.Init(this);
		}

		public void DespawnEnemy(Enemy enemy) {
			_enemyPool.Despawn(enemy);
		}
	}
}
