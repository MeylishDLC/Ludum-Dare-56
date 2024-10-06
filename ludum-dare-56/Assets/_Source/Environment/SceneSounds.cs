using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Sound;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment
{
    public class SceneSounds: MonoBehaviour
    {
        [SerializeField] private float minTimeToKnock;
        [SerializeField] private float maxTimeToKnock;

        private CancellationTokenSource cancelKnockingSoundCts = new ();
        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            StartKnockingSoundsCycle(cancelKnockingSoundCts.Token).Forget();
        }

        public void CancelKnockingSounds()
        {
            cancelKnockingSoundCts.Cancel();
            cancelKnockingSoundCts.Dispose();
        }
        private async UniTask StartKnockingSoundsCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var randomTime = Random.Range(minTimeToKnock, maxTimeToKnock);
                await UniTask.Delay(TimeSpan.FromSeconds(randomTime), cancellationToken: token);

                var randomKnockingSoundIndex = Random.Range(0, 2);
                if (randomKnockingSoundIndex == 0)
                {
                    _soundManager.PlayOneShot(_soundManager.FMODEvents.KnockingLeft);
                }
                else
                {
                    _soundManager.PlayOneShot(_soundManager.FMODEvents.KnockingRight);
                }
            }
        }
    }
}