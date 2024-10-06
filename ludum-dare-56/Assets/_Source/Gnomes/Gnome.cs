using System;
using System.Threading;
using Camera;
using Core;
using Cysharp.Threading.Tasks;
using Items;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gnomes
{
    public abstract class Gnome: MonoBehaviour
    {
        public static Action OnGnomeChangeState;
        [field:SerializeField] public GnomeTypes GnomeType { get; protected set; }

        [Header("Visuals")] 
        [SerializeField] protected Sprite furtherSprite; 
        [SerializeField] protected Sprite closerSprite;
        [SerializeField] protected Sprite screamerImageSprite;
        
        [Header("Timers")]
        [SerializeField] protected float changeToCloserStateTime;
        [SerializeField] protected float changeToAttackStateTime;
        [SerializeField] protected float disappearTime;
        [SerializeField] protected float timeBeforeScreamer;
        
        protected RoutePointPair _routePointPair;
        protected GnomeState _currentState;
        protected Flashlight _flashlight;
        
        private Screamer _screamer;
        private SpriteRenderer _spriteRenderer;
        private CameraMovement _cameraMovement;
        
        private float _timeRemaining;
        private CancellationTokenSource _cancelChangeStateCts = new();
        protected virtual void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = furtherSprite;
        }
        private void OnDestroy()
        {
            if (_cancelChangeStateCts != null)
            {
                _cancelChangeStateCts.Cancel();
                _cancelChangeStateCts.Dispose();
            }
        }

        public virtual void Initialize(RoutePointPair routePointPair, Screamer screamer, Flashlight flashlight, 
            CameraMovement cameraMovement)
        {
            _flashlight = flashlight;
            _cameraMovement = cameraMovement;
            
            _screamer = screamer;
            _routePointPair = routePointPair;
            _routePointPair.IsReserved = true;
            
            _currentState = GnomeState.Appeared;
            
            CheckCts();
            CountTimeToNextStateAsync(changeToCloserStateTime, _cancelChangeStateCts.Token).Forget();
        }
        protected virtual void GetCloser()
        {
            _currentState = GnomeState.Closer;
            
            OnGnomeChangeState?.Invoke();
            
            gameObject.transform.position = _routePointPair.CloserPoint.position;
            _spriteRenderer.sprite = closerSprite;
            
            CheckCts();
            CountTimeToNextStateAsync(changeToAttackStateTime, _cancelChangeStateCts.Token).Forget();
        }
        private async UniTask Attack(CancellationToken token)
        {
            _currentState = GnomeState.Attack;
            OnGnomeChangeState?.Invoke();
            gameObject.SetActive(false);
            
            Debug.Log("Counting time to screamer");
            _flashlight.DisableFlashlight(true);
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeScreamer), cancellationToken: token);

            _cameraMovement.EnableCameraMovement(false);
            _screamer.ShowScreamer(screamerImageSprite);
            Debug.Log("Boo game over");
        }
      
        protected void ShooGnomeAway()
        {
            if (_currentState == GnomeState.Attack)
            {
                return;
            }
            _cancelChangeStateCts?.Cancel();
            _cancelChangeStateCts?.Dispose();
            DisappearAsync(CancellationToken.None).Forget();
        }
        private async UniTask DisappearAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disappearTime), cancellationToken: token);
            
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

            if (!token.IsCancellationRequested)
            {
                GoToNextState();
            }
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
        private void CheckCts()
        {
            _cancelChangeStateCts ??= new CancellationTokenSource();
        }
    }
}