using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gnomes
{
    public class Radiozilla: Gnome
    {
        public static event Action<Gnome> OnSpawnInDoors;
        public static event Action<Gnome> OnDespawnInDoors;
        
        [SerializeField] private int minSoundAmountToShoo;
        [SerializeField] private int maxSoundAmountToShoo;
        [SerializeField] private int minTomatoesAmountToShoo;
        [SerializeField] private int maxTomatoesAmountToShoo;
        [SerializeField] private float timeBeforeEating;

        private int _soundAmountToShoo;
        private int _tomatoesAmountToShoo;
        
        private int _currentSoundAmount;
        private int _currentTomatoAmount;
        private bool _allConditionsDone;
        private bool _isWaiting;
        
        private SoundButton[] _soundButtons;
        private Tomato _tomato;

        protected override void OnDestroy()
        {
            SubscribeOnButtons(false);
            _tomato.OnTomatoClicked -= OnTomatoClicked;
            OnDespawnInDoors?.Invoke(this);
            base.OnDestroy();
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, CameraMovement cameraMovement,
            SoundManager soundManager, Tomato tomato, SoundButton[] soundButtons)
        {
            OnSpawnInDoors?.Invoke(this);
            
            _soundAmountToShoo = Random.Range(minSoundAmountToShoo, maxSoundAmountToShoo + 1);
            _tomatoesAmountToShoo = Random.Range(minTomatoesAmountToShoo, maxTomatoesAmountToShoo + 1);
            
            PlayAppearSound(soundManager);
            _screamerSound = soundManager.FMODEvents.TomatozillaScreamer;
            _tomato = tomato;
            _soundButtons = soundButtons;
            SubscribeOnButtons(true);
            _tomato.OnTomatoClicked += OnTomatoClicked;
            
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
        }

        protected override UniTask Attack(CancellationToken token)
        {
            OnDespawnInDoors?.Invoke(this);
            return base.Attack(token);
        }

        private void PlayAppearSound(SoundManager soundManager)
        {
            if (GnomeType == GnomeTypes.RadiozillaRight)
            {
                soundManager.PlayOneShot(soundManager.FMODEvents.PairAppearRight);
            }
            if (GnomeType == GnomeTypes.RadiozillaLeft)
            {
                soundManager.PlayOneShot(soundManager.FMODEvents.PairAppearLeft);
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
            
            if (_currentTomatoAmount < _tomatoesAmountToShoo)
            {
                _currentTomatoAmount++;
                if (_currentTomatoAmount < _tomatoesAmountToShoo)
                {
                    return;
                }
            }
            TryShoo();
        }
        private void PlayEatSound()
        {
            if (GnomeType == GnomeTypes.RadiozillaLeft)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.EatingLeft);
            }
            if (GnomeType == GnomeTypes.RadiozillaRight)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.EatingRight);
            }
        }
        private void TryShoo()
        {
            if (_currentTomatoAmount >= _tomatoesAmountToShoo
                && _currentSoundAmount >= _soundAmountToShoo)
            {
                ShooGnomeAway();
                _tomato.OnTomatoClicked -= OnTomatoClicked;
                SubscribeOnButtons(false);
            }
        }
        private void OnSoundButtonPressed()
        {
            if (_currentSoundAmount < _soundAmountToShoo)
            {
                _currentSoundAmount++;

                if (_currentSoundAmount < _soundAmountToShoo)
                {
                    return;
                }
            }
            TryShoo();
        }
        private void SubscribeOnButtons(bool subscribe)
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