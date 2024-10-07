using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagement
{
    public class ShowAfterLevel: MonoBehaviour
    {
        [SerializeField] private SceneController sceneController;
        [SerializeField] private int nextSceneIndex;
        
        [Header("Screens")]
        [SerializeField] private Image screenToShowAfterLevel;
        [SerializeField] private Image nextLevelScreen;
        [SerializeField] private float nexlLevelScreenStayTime;
        [SerializeField] private float screenFadeDuration;

        private bool isLastLevel;
        private Button screenButton;
        private SoundManager _soundManager;

        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            screenToShowAfterLevel.gameObject.SetActive(false);
            screenButton = screenToShowAfterLevel.GetComponent<Button>();

            if (nextLevelScreen != null)
            {
                nextLevelScreen.gameObject.SetActive(false);
                
                screenButton.onClick.AddListener(ShowNextLevelScreen);
            }
            else
            {
                isLastLevel = true;
                screenButton.onClick.AddListener(Continue);
            }
            sceneController.OnLevelWon += ShowScreen;
        }
        private void ShowScreen()
        {
            if (!isLastLevel)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.Newspaper);
            }
            ShowScreenAsync(CancellationToken.None).Forget();
        }
        private async UniTask ShowScreenAsync(CancellationToken token)
        {
            screenToShowAfterLevel.gameObject.SetActive(true);
            screenButton.interactable = false;
            await screenToShowAfterLevel.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await screenToShowAfterLevel.DOFade(1f, screenFadeDuration).ToUniTask(cancellationToken: token);
            screenButton.interactable = true;
        }
        private void ShowNextLevelScreen()
        {
            ShowNextLevelScreenAsync(CancellationToken.None).Forget();
        }
        private async UniTask ShowNextLevelScreenAsync(CancellationToken token)
        {
            nextLevelScreen.gameObject.SetActive(true);
            await nextLevelScreen.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await nextLevelScreen.DOFade(1f, screenFadeDuration).ToUniTask(cancellationToken: token);

            await UniTask.Delay(TimeSpan.FromSeconds(nexlLevelScreenStayTime), cancellationToken: token);
            Continue();
        }
        private void Continue()
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}