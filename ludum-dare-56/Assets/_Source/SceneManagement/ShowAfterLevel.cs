using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneManagement
{
    public class ShowAfterLevel: MonoBehaviour
    {
        [SerializeField] private SceneController sceneController;
        [SerializeField] private Image screenToShowAfterLevel;
        [SerializeField] private SerializedScene nextScene; 
        private Button button;
        private void Start()
        {
            screenToShowAfterLevel.gameObject.SetActive(false);
            button = screenToShowAfterLevel.GetComponent<Button>();
            button.onClick.AddListener(Continue);
            sceneController.OnLevelWon += ShowScreen;
        }
        private void ShowScreen()
        {
            screenToShowAfterLevel.gameObject.SetActive(true);
        }
        private void Continue()
        {
            button.interactable = false;
            SceneManager.LoadScene(nextScene.SceneName);
        }
    }
}