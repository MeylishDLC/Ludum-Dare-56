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

        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            _soundManager.InitializeMusic(_soundManager.FMODEvents.GameMusic);
            startGameButton.onClick.AddListener(StartGame);
        }
        private void StartGame()
        {
            startGameButton.interactable = false;
            SceneManager.LoadScene("1stLevel");
            _soundManager.SetMusicArea(MusicAct.Game);
        }
    }
}
