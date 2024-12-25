using Zenject;
using UnityEngine;

namespace Installers {
	public class GameInstaller : MonoInstaller {
		[SerializeField] private GameplayUI _gamePlayUi;
		[SerializeField] private GameOverUI _gameOverUi;
		[SerializeField] private GameController _gameController;
		[SerializeField] private Player _player;

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
		}
	}
}
