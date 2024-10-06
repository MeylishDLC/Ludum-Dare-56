using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Gnomes
{
    public class Spoonkin: Gnome
    {
        [SerializeField] private float timeToHoldFlashlight;
        
        private float _remainingTimeToHold;
        private Flashlight _flashlight;
        public void Initialize(RoutePointPair routePointPair, Image screamerUIImage, Flashlight flashlight)
        {
            _flashlight = flashlight;
            TrackFlashlightHoldTime(CancellationToken.None).Forget();
            base.Initialize(routePointPair, screamerUIImage);
        }
        private async UniTask TrackFlashlightHoldTime(CancellationToken token)
        {
            _remainingTimeToHold = timeToHoldFlashlight;
            while (_remainingTimeToHold > 0)
            {
                if (_flashlight.IsOn)
                {
                    _remainingTimeToHold -= Time.deltaTime;
                    Debug.Log($"{_remainingTimeToHold}");
                }
           
                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
            }
            ShooGnomeAway();
        }
    }
}