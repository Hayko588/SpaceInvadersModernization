using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SpaceInvaders {
	public class Explosion : MonoBehaviour {
		public class Pool : MonoMemoryPool<Explosion> {
			protected override void OnSpawned(Explosion item) {
				item.Explode();
			}
			protected override void OnDespawned(Explosion item) {
				item.Stop();
			}
		}

		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private int _delay = 1000;

		private Action<Explosion> _despawn;

		public void Init(Vector3 position, Action<Explosion> despawn) {
			transform.position = position;
			_despawn = despawn;
		}

		private async void Explode() {
			_particleSystem.Play();
			await Task.Delay(_delay);
			_despawn?.Invoke(this);
		}

		private void Stop() {
			if (_particleSystem) {
				_particleSystem.Stop();
				_particleSystem.Clear();
			}
		}
	}
}
