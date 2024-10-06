using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gnomes
{
    public class RadioBass: Gnome
    {
        [SerializeField] private int soundAmountToShoo;

        private SoundButton[] _soundButtons;
        private int _currentSoundAmount;
        private void OnDestroy()
        {
            SubscribeOnEvents(false);
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, SoundButton[] soundButtons)
        {
            _soundButtons = soundButtons;
            SubscribeOnEvents(true);
            
            gameObject.transform.rotation = routePointPair.FurtherPoint.rotation;
            base.Initialize(routePointPair, screamer, flashlight);
        }
        protected override void GetCloser()
        {
            gameObject.transform.rotation = _routePointPair.CloserPoint.rotation;
            base.GetCloser();
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
            ShooGnomeAway();
            SubscribeOnEvents(false);
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