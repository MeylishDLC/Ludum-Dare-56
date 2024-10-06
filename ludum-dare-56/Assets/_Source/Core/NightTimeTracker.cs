using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class NightTimeTracker: MonoBehaviour
    {
        public event Action OnNightEnded;
        
        [SerializeField] private float nightDuration;
        [SerializeField] private Image timeIcon;

        private float _timeRemained;
        private void Start()
        {
            timeIcon.fillAmount = 1;
            TrackNightTime(CancellationToken.None).Forget();
        }
        private async UniTask TrackNightTime(CancellationToken token)
        {
            _timeRemained = nightDuration;
            while (_timeRemained > 0)
            {
                _timeRemained -= Time.deltaTime;

                if (timeIcon.fillAmount > 0)
                {
                    timeIcon.fillAmount = _timeRemained / nightDuration;
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            
            Debug.Log("Night Ended");
            OnNightEnded?.Invoke();
            //todo head to the next night
        }
    }
}