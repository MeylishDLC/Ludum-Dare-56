using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class Flashlight: MonoBehaviour
    {
        public bool IsOn { get; private set; }

        [SerializeField] private Image[] chargeBars;
        [SerializeField] private GameObject lightObject;
        [SerializeField] private float chargeTime;

        private float _remainingChargeTime;
        private bool _canTurnOn = true;
        
        private int _currentBarIndex;
        private float _timeToWasteOneBar;
        private float _barTimeRemaining;
        private CancellationTokenSource _cancelTrackBarsChargeCts = new();
        private void Start()
        {
            SetBars();
            lightObject.gameObject.SetActive(false);
            TrackFlashlightCharge(CancellationToken.None).Forget();
            TrackBarsChange(_cancelTrackBarsChargeCts.Token).Forget();
        }
        private void Update()
        {
            if (!_canTurnOn)
            {
                return;
            }
            
            if (Input.GetMouseButton(0))
            {
                var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    TurnOnFlashlight(true);
                }
                else
                {
                    TurnOnFlashlight(false);
                }
            }
            else
            {
                TurnOnFlashlight(false);
            }
        }
        private void SetBars()
        {
            _currentBarIndex = chargeBars.Length - 1;
            var barAmount = chargeBars.Length;
            _timeToWasteOneBar = chargeTime / barAmount;
        }
        private void TurnOnFlashlight(bool on)
        {
            lightObject.SetActive(on);
            IsOn = on;
        }
        private async UniTask TrackFlashlightCharge(CancellationToken token)
        {
            _remainingChargeTime = chargeTime;
            while (_remainingChargeTime > 0)
            {
                if (IsOn)
                {
                    _remainingChargeTime -= Time.deltaTime;
                }
                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);
            }
            _canTurnOn = false;
            TurnOnFlashlight(false);
            if (_cancelTrackBarsChargeCts != null)
            {
                _cancelTrackBarsChargeCts.Cancel();
                _cancelTrackBarsChargeCts.Dispose();
                if (chargeBars[0].gameObject.activeSelf)
                {
                    chargeBars[0].gameObject.SetActive(false);
                }
            }
        }
        private async UniTask TrackBarsChange(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _barTimeRemaining = _timeToWasteOneBar;
                while (_barTimeRemaining > 0)
                {
                    if (IsOn)
                    {
                        _barTimeRemaining -= Time.deltaTime;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update);
                }

                chargeBars[_currentBarIndex].gameObject.SetActive(false);
                _currentBarIndex--;
                if (_currentBarIndex >= 0)
                {
                    continue;
                }
                break;
            }
        }
    }
}