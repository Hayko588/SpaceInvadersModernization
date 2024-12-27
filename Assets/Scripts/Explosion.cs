using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DefaultNamespace {
	public class Explosion : MonoBehaviour, IPoolable {
		public class Pool : MemoryPool<Explosion> {
		}

		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private int _delay = 1000;

		private Action<Explosion> _despawn;

		public void Init(Action<Explosion> despawn) {
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

		public void OnSpawned() {
			Explode();
			gameObject.SetActive(true);
		}
		public void OnDespawned() {
			Stop();
			gameObject.SetActive(false);
		}
	}
}
