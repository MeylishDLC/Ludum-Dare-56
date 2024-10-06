using Camera;
using Core;
using Items;
using Sound;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gnomes
{
    public class Tomatozilla: Gnome
    {
        [SerializeField] private int tomatoesAmountToShoo;

        private Tomato _tomato;
        private int _currentTomatoAmount;
        private void OnDestroy()
        {
            _tomato.OnTomatoClicked -= OnTomatoClicked;
        }
        public void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager, Tomato tomato)
        {
            _tomato = tomato;
            _tomato.OnTomatoClicked += OnTomatoClicked;
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
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
            ShooGnomeAway();
            _tomato.OnTomatoClicked -= OnTomatoClicked;
        }
        
    }
}