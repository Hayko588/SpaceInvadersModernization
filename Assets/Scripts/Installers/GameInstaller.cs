using SpaceInvaders.Services;
using SpaceInvaders.UI;
using Zenject;
using UnityEngine;

namespace SpaceInvaders.Installers {
	public class GameInstaller : MonoInstaller {
		[SerializeField] private GameplayUI _gamePlayUi;
		[SerializeField] private GameOverUI _gameOverUi;
		[SerializeField] private GameController _gameController;
		[SerializeField] private Player _player;
		[SerializeField] private Enemy _enemyPrefab;
		[SerializeField] private Explosion _explosionPrefab;
		[SerializeField] private Projectile _projectilePrefab;

		public override void InstallBindings() {
			Container
				.Bind<GameplayUI>()
				.FromInstance(_gamePlayUi)
				.AsSingle()
				.NonLazy();
			Container
				.Bind<GameOverUI>()
				.FromInstance(_gameOverUi)
				.AsSingle()
				.NonLazy();
			Container.Bind<GameController>()
				.FromInstance(_gameController)
				.AsSingle()
				.NonLazy();
			Container
				.Bind<Player>()
				.FromInstance(_player)
				.AsSingle()
				.NonLazy();
			Container
				.BindMemoryPool<Enemy, Enemy.Pool>()
				.FromComponentInNewPrefab(_enemyPrefab)
				.UnderTransformGroup("Enemies");
			Container
				.BindMemoryPool<Explosion, Explosion.Pool>()
				.FromComponentInNewPrefab(_explosionPrefab)
				.UnderTransformGroup("Explosions");
			Container
				.BindMemoryPool<Projectile, Projectile.Pool>()
				.FromComponentInNewPrefab(_projectilePrefab)
				.UnderTransformGroup("Projectiles");
			Container
				.BindInterfacesAndSelfTo<PoolService>()
				.AsSingle();
		}
	}
}
