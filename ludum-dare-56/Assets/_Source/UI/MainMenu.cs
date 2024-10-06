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

        [Header("Settings")] 
        [SerializeField] private Image settingsScreen;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button returnButton;

        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            settingsScreen.gameObject.SetActive(false);
            _soundManager.InitializeMusic(_soundManager.FMODEvents.GameMusic);
            startGameButton.onClick.AddListener(StartGame);
            settingsButton.onClick.AddListener(OpenSettings);
            returnButton.onClick.AddListener(CloseSettings);
        }
        private void StartGame()
        {
            //TODO show guide and newspapers
            startGameButton.interactable = false;
            SceneManager.LoadScene("1stLevel");
            _soundManager.SetMusicArea(MusicAct.Game);
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
