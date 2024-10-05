﻿using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gnomes
{
    public abstract class Gnome: MonoBehaviour
    {
        public event Action OnGnomeChangeState;

        [field:SerializeField] public GnomeTypes GnomeType { get; protected set; }

        [Header("Visuals")] 
        [SerializeField] private Sprite furtherSprite; 
        [SerializeField] private Sprite closerSprite;
        [SerializeField] private Sprite screamerImageSprite;
        
        [Header("Timers")]
        [SerializeField] private float changeToCloserStateTime;
        [SerializeField] private float changeToAttackStateTime;
        [SerializeField] private float disappearTime;
        [SerializeField] private float timeBeforeScreamer;
        
        protected RoutePointPair _routePointPair;
        private GnomeState _currentState;

        private float _timeRemaining;
        private CancellationTokenSource _cancelChangeStateCts = new();

        private Image _screamerUIImage;
        private Animator _screamerAnimator;
        protected SpriteRenderer _spriteRenderer;
        protected virtual void Start()
        {
            _screamerUIImage.gameObject.SetActive(false);
            _screamerAnimator = _screamerUIImage.GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = furtherSprite;
        }
        //should be in a spawner
        public void Initialize(RoutePointPair routePointPair, Image screamerUIImage)
        {
            _screamerUIImage = screamerUIImage;
            _routePointPair = routePointPair;
            _routePointPair.IsReserved = true;
            
            _currentState = GnomeState.Appeared;
            
            CountTimeToNextStateAsync(changeToCloserStateTime, _cancelChangeStateCts.Token).Forget();
        }
        private void GetCloser()
        {
            _currentState = GnomeState.Closer;
            //todo screen blinking
            OnGnomeChangeState?.Invoke();
            
            gameObject.transform.position = _routePointPair.CloserPoint.position;
            _spriteRenderer.sprite = closerSprite;
            
            CountTimeToNextStateAsync(changeToAttackStateTime, _cancelChangeStateCts.Token).Forget();
        }
        private async UniTask Attack(CancellationToken token)
        {
            _currentState = GnomeState.Attack;
            OnGnomeChangeState?.Invoke();
            gameObject.SetActive(false);
            
            Debug.Log("Counting time to screamer");
            //todo disable flashlight
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeScreamer), cancellationToken: token);

            _screamerUIImage.gameObject.SetActive(true);
            _screamerUIImage.sprite = screamerImageSprite;
            Debug.Log("Boo game over");
            Time.timeScale = 0f;
        }
      
        private void ShooGnomeAway()
        {
            //should be invoked
            
            _cancelChangeStateCts.Cancel();
            _cancelChangeStateCts.Dispose();
            DisappearAsync(CancellationToken.None).Forget();
        }
        private async UniTask DisappearAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disappearTime), cancellationToken: token);
            
            //todo light blinking 
            OnGnomeChangeState?.Invoke();
            _routePointPair.IsReserved = false;
            Destroy(gameObject);
        }
        private async UniTask CountTimeToNextStateAsync(float changeStateTime, CancellationToken token)
        {
            var startTime = Time.time;
            _timeRemaining = changeStateTime;
            while (_timeRemaining > 0)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                _timeRemaining = changeStateTime - (Time.time - startTime);
                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
            }
            GoToNextState();
        }

        private void GoToNextState()
        {
            if (_currentState == GnomeState.Appeared)
            {
                GetCloser();
                return;
            }

            if (_currentState == GnomeState.Closer)
            {
                Attack(CancellationToken.None).Forget();
            }
        }
    }
}