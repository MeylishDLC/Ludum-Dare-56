using System;
using Gnomes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Core
{
    public class SceneController: MonoBehaviour
    {
        public bool IsPaused { get; private set; }
        
        [SerializeField] private Button restartButton;
        [SerializeField] private Image deathScreen;
        private Screamer _screamer;

        [Inject]
        public void Initialize(Screamer screamer)
        {
            _screamer = screamer;
        }
        private void Start()
        {
            deathScreen.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartScene);
            _screamer.OnPlayerDeath += OnGameOver;
        }
        private void OnGameOver()
        {
            PauseGame();
            deathScreen.gameObject.SetActive(true);
        }
        private void RestartScene()
        {
            _screamer.OnPlayerDeath -= OnGameOver;
            restartButton.interactable = false;
            var scene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(scene);
            ResumeGame();
        }
        private void PauseGame()
        {
            Time.timeScale = 0f;
            IsPaused = true;
        }
        private void ResumeGame()
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }
}