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
			_explosionPool.Spawn(position);
		}

		public void SpawnProjectile(ProjectileOwner owner, Vector3 position) {
			_projectilePool.Spawn(owner, position);
		}

		public void SpawnEnemy(Vector3 position) {
			var e = _enemyPool.Spawn(position);
			e.Init(this);
		}

		public void DespawnEnemy(Enemy enemy) {
			_enemyPool.Despawn(enemy);
		}
	}
}
