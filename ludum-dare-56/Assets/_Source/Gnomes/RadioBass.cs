using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Items;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Gnomes
{
    public class RadioBass: Gnome
    {
        public static event Action<Gnome> OnSpawnInDoors;
        public static event Action<Gnome> OnDespawnInDoors;
        
        [SerializeField] private int minSoundAmountToShoo;
        [SerializeField] private int maxSoundAmountToShoo;
        [SerializeField] private float timeBeforeLaugh;

        [Header("Shadows")] 
        [SerializeField] private GameObject _forwardShadow;
        [SerializeField] private GameObject _backShadow;
        
        private int _soundAmountToShoo;
        private SoundButton[] _soundButtons;
        private int _currentSoundAmount;
        private bool _isWaiting;

        private GnomeShadow _gnomeShadow;
        protected override void OnDestroy()
        {
            if (GnomeType != GnomeTypes.RadioBass)
            {
                OnDespawnInDoors?.Invoke(this);
            }
            _gnomeShadow?.CancelShadowTracking();
            base.OnDestroy();
            SubscribeOnEvents(false);
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager, SoundButton[] soundButtons)
        {
            _soundAmountToShoo = Random.Range(minSoundAmountToShoo, maxSoundAmountToShoo + 1);

            if (GnomeType == GnomeTypes.RadioBass)
            {
                _forwardShadow.SetActive(false);
                _backShadow.SetActive(false);
            }
            else
            {
                OnSpawnInDoors?.Invoke(this);
            }
            _screamerSound = soundManager.FMODEvents.RadioBassScreamer;
            _soundButtons = soundButtons;
            SubscribeOnEvents(true);
            
            gameObject.transform.rotation = routePointPair.FurtherPoint.rotation;
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
        }
        
        protected override async UniTask Appear(CancellationToken token)
        {
            _currentState = GnomeState.Appeared;
            await _spriteRenderer.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await _spriteRenderer.DOFade(1f, 0.5f).ToUniTask(cancellationToken: token);
            
            CheckCts();
            _soundManager.PlayMusicDuringTime(changeToCloserStateTime, _soundManager.FMODEvents.FarGlitch);
            CountTimeToNextStateAsync(changeToCloserStateTime, _cancelChangeStateCts.Token).Forget();
        }
        protected override void GetCloser()
        {
            gameObject.transform.rotation = _routePointPair.CloserPoint.rotation;
            _soundManager.PlayMusicDuringTime(changeToAttackStateTime, _soundManager.FMODEvents.CloseGlitch);
            
            if (GnomeType == GnomeTypes.RadioBass)
            {
                _gnomeShadow = new GnomeShadow(_flashlight);
                _gnomeShadow.SetShadows(_forwardShadow, _backShadow);
            }
            base.GetCloser();
        }
        protected override UniTask Attack(CancellationToken token)
        {
            if (GnomeType != GnomeTypes.RadioBass)
            {
                OnDespawnInDoors?.Invoke(this);
            }
            
            if (_gnomeShadow != null)
            {
                _gnomeShadow.CancelShadowTracking();
                _backShadow.SetActive(false);
                _forwardShadow.SetActive(false);
            }
            return base.Attack(token);
        }
        private void OnSoundButtonPressed()
        {
            OnSoundButtonPressedAsync(CancellationToken.None).Forget();
        }
        private async UniTask OnSoundButtonPressedAsync(CancellationToken token)
        {
            if (_currentSoundAmount < _soundAmountToShoo)
            {
                _currentSoundAmount++;

                if (_currentSoundAmount < _soundAmountToShoo)
                {
                    return;
                }
            }

            if (_isWaiting)
            {
                return;
            }
            _isWaiting = true;
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeLaugh), cancellationToken: token);
            PlayLaugh();
            
            ShooGnomeAway();
            SubscribeOnEvents(false);
        }
        private void PlayLaugh()
        {
            if (GnomeType == GnomeTypes.RadioBass)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.LaughHall);
            }
            if (GnomeType == GnomeTypes.RadioBassLeft)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.LaughLeft);
            }
            if (GnomeType == GnomeTypes.RadioBassRight)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.LaughRight);
            }
        }
        private void SubscribeOnEvents(bool subscribe)
        {
            if (subscribe)
            {
                foreach (var button in _soundButtons)
                {
                    button.OnSoundPlayed += OnSoundButtonPressed;
                }
            }
            else
            {
                foreach (var button in _soundButtons)
                {
                    button.OnSoundPlayed -= OnSoundButtonPressed;
                }
            }
        }
    }
}