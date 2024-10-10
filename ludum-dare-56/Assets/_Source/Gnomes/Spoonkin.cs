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
        
        [Header("Far")]
        [SerializeField] private GameObject backShadow;
        [SerializeField] private GameObject forwardShadow;

        [Header("Close")] 
        [SerializeField] private GameObject backShadow2;
        [SerializeField] private GameObject forwardShadow2;
        
        private float _remainingTimeToHold;
        private GnomeShadow _shadow;
        public override void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement, SoundManager soundManager)
        {
            _shadow = new GnomeShadow(flashlight);
            _shadow.SetShadows(forwardShadow, backShadow);
            
            if (flashlight.IsOn)
            {
                backShadow.SetActive(true);
            }
            else
            {
                forwardShadow.SetActive(true);
            }
            
            _screamerSound = soundManager.FMODEvents.SpoonkinScreamer;
            _flashlight = flashlight;
            TrackFlashlightHoldTime(CancellationToken.None).Forget();
            base.Initialize(routePointPair, screamer, flashlight, cameraMovement, soundManager);
        }
        protected override void OnDestroy()
        {
            _shadow.CancelShadowTracking();
            base.OnDestroy();
        }
        protected override void GetCloser()
        {
            _shadow.SetShadows(forwardShadow2, backShadow2);
            backShadow.SetActive(false);
            forwardShadow.SetActive(false);
            base.GetCloser();
        }
        protected override UniTask Attack(CancellationToken token)
        {
            _shadow.CancelShadowTracking();
            backShadow2.SetActive(false);
            forwardShadow2.SetActive(false);
            
            return base.Attack(token);
        }
        private async UniTask TrackFlashlightHoldTime(CancellationToken token)
        {
            _remainingTimeToHold = timeToHoldFlashlight;
            while (_remainingTimeToHold > 0)
            {
                if (_flashlight.IsOn)
                {
                    _remainingTimeToHold -= Time.deltaTime;
                }
           
                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
            }
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Shocked);
            ShooGnomeAway();
        }
    }
}