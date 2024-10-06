using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Gnomes
{
    public class Spoonkin: Gnome
    {
        [SerializeField] private float timeToHoldFlashlight;
        
        private float _remainingTimeToHold;
        public override void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager)
        {
            _screamerSound = soundManager.FMODEvents.SpoonkinScreamer;
            _flashlight = flashlight;
            TrackFlashlightHoldTime(CancellationToken.None).Forget();
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
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
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Shocked);
            ShooGnomeAway();
        }
    }
}