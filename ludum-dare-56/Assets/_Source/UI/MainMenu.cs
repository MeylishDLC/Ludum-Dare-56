using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private GameObject soundManagerPrefab;

        [Header("Settings")] 
        [SerializeField] private Image settingsScreen;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button returnButton;

        [Header("Guide")] 
        [SerializeField] private Image guideScreen;
        [SerializeField] private GameObject pageOne;
        [SerializeField] private Button continueButtonOne;
        [SerializeField] private GameObject pageTwo;
        [SerializeField] private Button continueButtonTwo;

        [Header("Newspaper")] 
        [SerializeField] private Image newspaper;
        [SerializeField] private float newspaperFadeTime;
        
        private Button _newspaperButton;
        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            settingsScreen.gameObject.SetActive(false);
            startGameButton.onClick.AddListener(StartGuide);
            settingsButton.onClick.AddListener(OpenSettings);
            returnButton.onClick.AddListener(CloseSettings);
            
            guideScreen.gameObject.SetActive(false);
            pageTwo.SetActive(false);
            continueButtonOne.onClick.AddListener(MoveToNextPage);
            continueButtonTwo.onClick.AddListener(ShowNewspaper);

            newspaper.gameObject.SetActive(false);
            _newspaperButton = newspaper.GetComponent<Button>();
            _newspaperButton.onClick.AddListener(StartGame);
        }
        private void StartGuide()
        {
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Newspaper);
            guideScreen.gameObject.SetActive(true);
        }
        private void MoveToNextPage()
        {
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Newspaper);
            pageOne.SetActive(false);
            pageTwo.SetActive(true);
        }
        private void ShowNewspaper()
        {
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Newspaper);
            ShowNewspaperAsync(CancellationToken.None).Forget();
        }
        private async UniTask ShowNewspaperAsync(CancellationToken token)
        {
            _newspaperButton.interactable = false;
            newspaper.gameObject.SetActive(true);
            await newspaper.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await newspaper.DOFade(1f, newspaperFadeTime).ToUniTask(cancellationToken: token);
            _newspaperButton.interactable = true;
        }
        private void StartGame()
        {
            StartGameAsync(CancellationToken.None).Forget();
        }
        private async UniTask StartGameAsync(CancellationToken token)
        {
            await newspaper.DOColor(Color.black, newspaperFadeTime).ToUniTask(cancellationToken: token);
            startGameButton.interactable = false;
            SceneManager.LoadScene("1stLevel");
        }
        private void OpenSettings()
        {
            settingsScreen.gameObject.SetActive(true);
        }
        private void CloseSettings()
        {
            settingsScreen.gameObject.SetActive(false);
        }
    }
}
