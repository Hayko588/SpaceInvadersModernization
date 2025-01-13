using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SpaceInvaders {
	public class Explosion : MonoBehaviour {
		public class Pool : MonoMemoryPool<Vector3, Explosion> {
			protected override void Reinitialize(Vector3 pos, Explosion item) {
				item.Init(pos, Despawn);
			}
		}

		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private int _delay = 1000;

		private Action<Explosion> _despawn;

		public void Init(Vector3 position, Action<Explosion> despawn) {
			transform.position = position;
			_despawn = despawn;
			Explode();
		}

		private async void Explode() {
			_particleSystem.Play();
			await Task.Delay(_delay);
			_despawn?.Invoke(this);
			Stop();
		}

		private void Stop() {
			if (_particleSystem) {
				_particleSystem.Stop();
				_particleSystem.Clear();
			}
		}
	}
}
