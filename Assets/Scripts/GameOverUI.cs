using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceInvaders.UI {
	public class GameOverUI : MonoBehaviour {
		public void Open() {
			gameObject.SetActive(true);
		}

		public void Close() {
			gameObject.SetActive(false);
		}

		public void OnRetryButton() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
