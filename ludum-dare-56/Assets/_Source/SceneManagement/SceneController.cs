using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gnomes;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagement
{
    public class SceneController: MonoBehaviour
    {
        public event Action OnLevelWon;
        public bool IsPaused { get; private set; }
        
        [SerializeField] private Transform gnomeContainer;
        [SerializeField] private GnomeSpawner gnomeSpawner;
        
        [Header("End Screen Settings")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Image deathScreen;
        [SerializeField] private Image winScreen;
        [SerializeField] private float winScreenFadeTime;
        [SerializeField] private float timeToMoveToNextLevel;
        
        private Screamer _screamer;
        private NightTimeTracker _nightTimeTracker;
        private CameraMovement _cameraMovement;
        private SoundManager _soundManager;

        [Inject]
        public void Initialize(Screamer screamer, NightTimeTracker nightTimeTracker, CameraMovement cameraMovement, 
            SoundManager soundManager)
        {
            _screamer = screamer;
            _nightTimeTracker = nightTimeTracker;
            _cameraMovement = cameraMovement;
            _soundManager = soundManager;
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
            ClearScene();
            _cameraMovement.EnableCameraMovement(false);
            deathScreen.gameObject.SetActive(true);
            _soundManager.SetMusicArea(MusicAct.GameLose);
        }
        private void OnGameWin()
        {
            ClearScene();
            _soundManager.SetMusicArea(MusicAct.Menu);
            
            _nightTimeTracker.OnNightEnded -= OnGameWin;
            _cameraMovement.EnableCameraMovement(false);
            ShowWinScreenAsync(CancellationToken.None).Forget();
        }
        private void ClearScene()
        {
            Destroy(gnomeContainer.gameObject);
            gnomeSpawner.StopSpawning();
        }
        private async UniTask ShowWinScreenAsync(CancellationToken token)
        {
            winScreen.gameObject.SetActive(true);
            await winScreen.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await winScreen.DOFade(1f, winScreenFadeTime).ToUniTask(cancellationToken: token);
            await UniTask.Delay(TimeSpan.FromSeconds(timeToMoveToNextLevel), cancellationToken: token);
            OnLevelWon?.Invoke();
        }
        private void RestartScene()
        {
            _screamer.OnPlayerDeath -= OnGameLose;
            restartButton.interactable = false;
            var scene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(scene);
            _soundManager.SetMusicArea(MusicAct.Game);
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