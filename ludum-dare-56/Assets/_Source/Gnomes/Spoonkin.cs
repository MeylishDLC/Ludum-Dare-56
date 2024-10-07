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
            _shadow = new GnomeShadow(forwardShadow, backShadow, flashlight);
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
            if (_shadow != null)
            {
                _shadow.Unsubscribe();
            }
            base.OnDestroy();
        }
        protected override void GetCloser()
        {
            _shadow.Unsubscribe();
            backShadow.SetActive(false);
            forwardShadow.SetActive(false);
            
            _shadow = new GnomeShadow(forwardShadow2, backShadow2, _flashlight);
            if (_flashlight.IsOn)
            {
                backShadow2.SetActive(true);
            }
            else
            {
                forwardShadow2.SetActive(true);
            }
            base.GetCloser();
        }
        protected override UniTask Attack(CancellationToken token)
        {
            backShadow2.SetActive(false);
            forwardShadow2.SetActive(false);
            _shadow.Unsubscribe();
            
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
                    Debug.Log($"{_remainingTimeToHold}");
                }
           
                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
            }
            _soundManager.PlayOneShot(_soundManager.FMODEvents.Shocked);
            ShooGnomeAway();
        }
    }
}