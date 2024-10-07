using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gnomes
{
    public class Screamer: MonoBehaviour
    {
        public event Action OnPlayerDeath;

        [SerializeField] private float timeForScreamer;
        
        private Image _screamerUIImage;
        private Animator _screamerAnimator;
        private SoundManager _soundManager;
        private static readonly int Shake = Animator.StringToHash("shake");

        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            _screamerUIImage = GetComponent<Image>();
            _screamerAnimator = GetComponent<Animator>();
            _screamerUIImage.gameObject.SetActive(false);
        }
        public void ShowScreamer(Gnome gnome, Sprite screamerSprite, EventReference sound)
        {
            ShowScreamerAsync(gnome, screamerSprite, sound, CancellationToken.None).Forget();
        }
        private async UniTask ShowScreamerAsync(Gnome gnome, Sprite screamerSprite, EventReference sound, CancellationToken token)
        {
            _screamerUIImage.gameObject.SetActive(true);
            _screamerUIImage.sprite = screamerSprite;
            _soundManager.PlayOneShot(sound);
            
            if (gnome.GnomeType == GnomeTypes.RadioBass)
            {
                _screamerAnimator.SetTrigger(Shake);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(timeForScreamer), cancellationToken: token);
            OnPlayerDeath?.Invoke();
        }
    }
}