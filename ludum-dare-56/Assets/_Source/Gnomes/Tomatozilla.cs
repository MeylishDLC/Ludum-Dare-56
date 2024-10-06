using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Items;
using Sound;
using UnityEngine;

namespace Gnomes
{
    public class Tomatozilla: Gnome
    {
        [SerializeField] private int tomatoesAmountToShoo;
        [SerializeField] private float timeBeforeEating;
        
        private Tomato _tomato;
        private int _currentTomatoAmount;
        private bool _isWaiting;
        private void OnDestroy()
        {
            _tomato.OnTomatoClicked -= OnTomatoClicked;
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager, Tomato tomato)
        {
            PlayAppearSound(soundManager);
            
            _screamerSound = soundManager.FMODEvents.TomatozillaScreamer;
            _tomato = tomato;
            _tomato.OnTomatoClicked += OnTomatoClicked;
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
        }

        private void PlayAppearSound(SoundManager soundManager)
        {
            if (GnomeType == GnomeTypes.TomatozillaLeft)
            {
                soundManager.PlayOneShot(soundManager.FMODEvents.AppearLeft);
            }
            if (GnomeType == GnomeTypes.TomatozillaRight)
            {
                soundManager.PlayOneShot(soundManager.FMODEvents.AppearRight);
            }
        }

        private void PlayEatSound()
        {
            if (GnomeType == GnomeTypes.TomatozillaLeft)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.EatingLeft);
            }
            if (GnomeType == GnomeTypes.TomatozillaRight)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.EatingRight);
            }
        }
        private void OnTomatoClicked()
        {
            OnTomatoClickedAsync(CancellationToken.None).Forget();
        }
        private async UniTask OnTomatoClickedAsync(CancellationToken token)
        {
            if (_currentState != GnomeState.Closer)
            {
                return;
            }
            if (_isWaiting)
            {
                return;
            }
            
            _isWaiting = true;
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeEating), cancellationToken: token);
            PlayEatSound();
            _isWaiting = false;
            
            if (_currentTomatoAmount < tomatoesAmountToShoo)
            {
                _currentTomatoAmount++;
                if (_currentTomatoAmount < tomatoesAmountToShoo)
                {
                    return;
                }
            }
            ShooGnomeAway();
            _tomato.OnTomatoClicked -= OnTomatoClicked;
        }
        
    }
}