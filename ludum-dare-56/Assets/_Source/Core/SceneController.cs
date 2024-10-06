using System;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [Header("End Screen Settings")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Image deathScreen;
        [SerializeField] private Image winScreen;
        [SerializeField] private float winScreenFadeTime;
        
        private Screamer _screamer;
        private NightTimeTracker _nightTimeTracker;
        private CameraMovement _cameraMovement;

        [Inject]
        public void Initialize(Screamer screamer, NightTimeTracker nightTimeTracker, CameraMovement cameraMovement)
        {
            _screamer = screamer;
            _nightTimeTracker = nightTimeTracker;
            _cameraMovement = cameraMovement;
        }
        private void Start()
        {
            deathScreen.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartScene);
            _screamer.OnPlayerDeath += OnGameLose;
            _nightTimeTracker.OnNightEnded += OnGameWin;
        }
        private void OnGameLose()
        {
            PauseGame();
            deathScreen.gameObject.SetActive(true);
        }
        private void OnGameWin()
        {
            _nightTimeTracker.OnNightEnded -= OnGameWin;
            _cameraMovement.EnableCameraMovement(false);
            ShowWinScreenAsync(CancellationToken.None).Forget();
        }
        private async UniTask ShowWinScreenAsync(CancellationToken token)
        {
            winScreen.gameObject.SetActive(true);
            await winScreen.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await winScreen.DOFade(1f, winScreenFadeTime).ToUniTask(cancellationToken: token);
        }
        private void RestartScene()
        {
            _screamer.OnPlayerDeath -= OnGameLose;
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