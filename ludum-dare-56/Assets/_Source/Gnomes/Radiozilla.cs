using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using Sound;
using UnityEngine;

namespace Gnomes
{
    public class Radiozilla: Gnome
    {
        [SerializeField] private int soundAmountToShoo;
        [SerializeField] private int tomatoesAmountToShoo;
        
        private int _currentSoundAmount;
        private int _currentTomatoAmount;
        private bool _allConditionsDone;
        
        private SoundButton[] _soundButtons;
        private Tomato _tomato;
        private void OnDestroy()
        {
            SubscribeOnButtons(false);
            _tomato.OnTomatoClicked -= OnTomatoClicked;
        }
        
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, CameraMovement cameraMovement,
            SoundManager soundManager, Tomato tomato, SoundButton[] soundButtons)
        {
            PlayAppearSound(soundManager);
            _screamerSound = soundManager.FMODEvents.TomatozillaScreamer;
            _tomato = tomato;
            _soundButtons = soundButtons;
            SubscribeOnButtons(true);
            _tomato.OnTomatoClicked += OnTomatoClicked;
            
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
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
            if (_currentState != GnomeState.Closer)
            {
                return;
            }
            
            if (_currentTomatoAmount < tomatoesAmountToShoo)
            {
                _currentTomatoAmount++;
                if (_currentTomatoAmount < tomatoesAmountToShoo)
                {
                    return;
                }
            }
            TryShoo();
        }
        private void TryShoo()
        {
            if (_currentTomatoAmount >= tomatoesAmountToShoo
                && _currentSoundAmount >= soundAmountToShoo)
            {
                ShooGnomeAway();
                _tomato.OnTomatoClicked -= OnTomatoClicked;
                SubscribeOnButtons(false);
            }
        }
        private void OnSoundButtonPressed()
        {
            if (_currentSoundAmount < soundAmountToShoo)
            {
                _currentSoundAmount++;

                if (_currentSoundAmount < soundAmountToShoo)
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