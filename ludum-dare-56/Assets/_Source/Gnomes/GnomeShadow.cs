using System.Threading;
using Cysharp.Threading.Tasks;
using Items;
using UnityEngine;

namespace Gnomes
{
    public class GnomeShadow
    {
        public GameObject ForwardShadow { get; set; }
        public GameObject BackShadow { get; set; }
        public Flashlight _flashlight { get; set; }

        private CancellationTokenSource _cancelFlashlightTrackingCts = new();
        public GnomeShadow(Flashlight flashlight)
        {
            _flashlight = flashlight;
            SetShadowsCycle(_cancelFlashlightTrackingCts.Token).Forget();
        }
        public void SetShadows(GameObject forwardShadow, GameObject backShadow)
        {
            ForwardShadow = forwardShadow;
            BackShadow = backShadow;
        }
        public void CancelShadowTracking()
        {
            if (_cancelFlashlightTrackingCts != null)
            {
                _cancelFlashlightTrackingCts.Cancel();
                _cancelFlashlightTrackingCts.Dispose();
            }
        }
        private async UniTask SetShadowsCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SwitchShadow();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void SwitchShadow()
        {
            if (ForwardShadow == null || BackShadow == null)
            {
                return;
            }
            
            if (!_flashlight.IsOn)
            {
                TurnOnForwardShadow();
            }
            else
            {
                TurnOnBackShadow();
            }
        }
        private void TurnOnBackShadow()
        {
            BackShadow.SetActive(true);
            ForwardShadow.SetActive(false);
        }
        private void TurnOnForwardShadow()
        {
            BackShadow.SetActive(false);
            ForwardShadow.SetActive(true);
        }
    }
}