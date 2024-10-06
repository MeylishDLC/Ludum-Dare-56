using Core;
using Items;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gnomes
{
    public class Tomatozilla: Gnome
    {
        [SerializeField] private int tomatoesAmountToShoo;

        private Tomato[] _allTomatoes;
        private int _currentTomatoAmount;
        private void OnDestroy()
        {
            SubscribeOnEvents(false);
        }
        public void Initialize(RoutePointPair routePointPair, Image screamerUIImage, Tomato[] tomatoes)
        {
            _allTomatoes = tomatoes;
            SubscribeOnEvents(true);
            base.Initialize(routePointPair, screamerUIImage);
        }
        private void OnTomatoClicked()
        {
            if (_currentTomatoAmount < tomatoesAmountToShoo)
            {
                _currentTomatoAmount++;
                if (_currentTomatoAmount < tomatoesAmountToShoo)
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
                foreach (var tomato in _allTomatoes)
                {
                    tomato.OnTomatoClicked += OnTomatoClicked;
                }
            }
            else
            {
                foreach (var tomato in _allTomatoes)
                {
                    tomato.OnTomatoClicked -= OnTomatoClicked;
                }
            }
        }
    }
}