using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gnomes
{
    public class Screamer: MonoBehaviour
    {
        public event Action OnPlayerDeath;

        [SerializeField] private float timeForScreamer;
        
        private Image _screamerUIImage;
        private Animator _screamerAnimator;
        private void Start()
        {
            _screamerUIImage = GetComponent<Image>();
            _screamerAnimator = GetComponent<Animator>();
            _screamerUIImage.gameObject.SetActive(false);
        }
        public void ShowScreamer(Sprite screamerSprite)
        {
            ShowScreamerAsync(screamerSprite, CancellationToken.None).Forget();
        }
        private async UniTask ShowScreamerAsync(Sprite screamerSprite, CancellationToken token)
        {
            _screamerUIImage.gameObject.SetActive(true);
            _screamerUIImage.sprite = screamerSprite;
            await UniTask.Delay(TimeSpan.FromSeconds(timeForScreamer), cancellationToken: token);
            OnPlayerDeath?.Invoke();
        }
    }
}