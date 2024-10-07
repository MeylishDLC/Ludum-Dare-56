using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gnomes;
using Items;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment
{
    public class SceneLight: MonoBehaviour
    {
        [SerializeField] private Light2D baseSceneLight;
        [SerializeField] private Light2D[] doorLights;
        [SerializeField] private float minDimValue;
        [SerializeField] private float lightBlinkDuration;
        [SerializeField] private float smoothDuration;
        [SerializeField] private int maxBlinkAmount;
        [SerializeField] private int minBlinkAmount;
        
        private Flashlight _flashlight;
        private float _originalIntensity;
        private float _doorLightOrigIntensity;
        private bool _isBlinking;

        [Inject]
        public void Initialize(Flashlight flashlight)
        {
            _flashlight = flashlight;
            _originalIntensity = baseSceneLight.intensity;
            _doorLightOrigIntensity = doorLights[0].intensity;
            Gnome.OnGnomeChangeState += TriggerBlink;
        }
        private void OnDestroy()
        {
            Gnome.OnGnomeChangeState -= TriggerBlink;
        }
        private void TriggerBlink()
        {
            if (!_isBlinking)
            {
                BlinkAsync().Forget();
            }
        }
        private async UniTask BlinkAsync()
        {
            _isBlinking = true;
            
            _flashlight.DisableFlashlight(true);
            var blinkAmount = Random.Range(minBlinkAmount, maxBlinkAmount + 1);
            var timeForOneBlink = lightBlinkDuration / blinkAmount;
            
            for (var i = 0; i < blinkAmount; i++)
            {
                await SmoothTransition(_originalIntensity, _doorLightOrigIntensity, 
                    minDimValue, minDimValue,
                    timeForOneBlink / 2, CancellationToken.None);
                
                await SmoothTransition(minDimValue, minDimValue, 
                    _originalIntensity, _doorLightOrigIntensity,
                    timeForOneBlink / 2, CancellationToken.None);
            }
            _flashlight.DisableFlashlight(false);
            _isBlinking = false;
        }
        
        private async UniTask SmoothTransition(float baseLightStartIntensity, float doorLightStartIntensity, 
            float baseLightEndIntensity, float doorLightEndIntensity, float duration, CancellationToken token)
        {
            var elapsedTime = 0f;
            while (elapsedTime < smoothDuration)
            {
                baseSceneLight.intensity = Mathf.Lerp(baseLightStartIntensity, baseLightEndIntensity, elapsedTime / smoothDuration);
                foreach (var doorLight in doorLights)
                {
                    doorLight.intensity = Mathf.Lerp(doorLightStartIntensity, doorLightEndIntensity, elapsedTime / smoothDuration);
                }
                
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            var timeLeft = duration - smoothDuration;
            await UniTask.Delay(TimeSpan.FromSeconds(timeLeft), cancellationToken: token);
            baseSceneLight.intensity = baseLightEndIntensity;
            foreach (var doorLight in doorLights)
            {
                doorLight.intensity = doorLightEndIntensity;
            }
        }
    }
}