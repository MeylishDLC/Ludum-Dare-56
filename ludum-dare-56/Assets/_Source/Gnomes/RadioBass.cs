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

namespace Gnomes
{
    public class RadioBass: Gnome
    {
        [SerializeField] private int soundAmountToShoo;
        [SerializeField] private float timeBeforeLaugh;

        private SoundButton[] _soundButtons;
        private int _currentSoundAmount;
        private bool _isWaiting;
        private void OnDestroy()
        {
            SubscribeOnEvents(false);
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager, SoundButton[] soundButtons)
        {
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
            base.GetCloser();
        }
        private void OnSoundButtonPressed()
        {
            OnSoundButtonPressedAsync(CancellationToken.None).Forget();
        }
        private async UniTask OnSoundButtonPressedAsync(CancellationToken token)
        {
            if (_currentSoundAmount < soundAmountToShoo)
            {
                _currentSoundAmount++;

                if (_currentSoundAmount < soundAmountToShoo)
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