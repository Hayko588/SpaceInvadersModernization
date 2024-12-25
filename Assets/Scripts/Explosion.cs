using UnityEngine;
using Zenject;

namespace DefaultNamespace {
	public class Explosion : MonoBehaviour {
		public class Pool : MemoryPool<Explosion> {
			protected override void OnSpawned(Explosion item) {
				item.Explode();
			}
			protected override void OnDespawned(Explosion item) {
				item.Stop();
			}
		}

		[SerializeField] private ParticleSystem _particleSystem;

		private void Explode() {
			_particleSystem.Play();
		}

		private void Stop() {
			_particleSystem.Stop();
			_particleSystem.Clear();
		}
	}
}
