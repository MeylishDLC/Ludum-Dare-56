using Items;
using UnityEngine;

namespace Gnomes
{
    public class GnomeShadow
    {
        private GameObject _forwardShadow;
        private GameObject _backShadow;
        private Flashlight _flashlight;

        public GnomeShadow(GameObject forwardShadow, GameObject backShadow, Flashlight flashlight)
        {
            _forwardShadow = forwardShadow;
            _backShadow = backShadow;

            _flashlight = flashlight;
            _flashlight.OnFlashlightSwitch += SwitchShadow;
        }
        public void Unsubscribe()
        {
            _flashlight.OnFlashlightSwitch -= SwitchShadow;
        }
        private void SwitchShadow(bool lightOn)
        {
            if (lightOn)
            {
                TurnOnBackShadow();
            }
            else
            {
                TurnOnForwardShadow();
            }
        }
        private void TurnOnBackShadow()
        {
            _backShadow.SetActive(true);
            _forwardShadow.SetActive(false);
        }
        private void TurnOnForwardShadow()
        {
            _backShadow.SetActive(false);
            _forwardShadow.SetActive(true);
        }
    }
}