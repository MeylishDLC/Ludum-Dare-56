using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Sound
{
    public class VolumeSlider: MonoBehaviour
    {
        private enum VolumeType
        {
            MASTER,
            MUSIC,
            SFX
        }

        [Header("Type")] 
        [SerializeField] private VolumeType volumeType;

        private Slider _volumeSlider;
        private SoundManager _soundManager;

        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            _volumeSlider = GetComponent<Slider>();
            _volumeSlider.onValueChanged.AddListener(_ => OnSliderValueChanged());
            
            switch (volumeType)
            {
                case VolumeType.MASTER:
                    _volumeSlider.value = _soundManager.masterVolume;
                    break;
                case VolumeType.SFX:
                    _volumeSlider.value = _soundManager.SFXVolume;
                    break;
                case VolumeType.MUSIC:
                    _volumeSlider.value = _soundManager.musicVolume;
                    break;
                default:
                    throw new Exception($"Volume type not supported: {volumeType}");
            }
        }
        private void OnSliderValueChanged()
        {
            switch (volumeType)
            {
                case VolumeType.MASTER:
                    _soundManager.masterVolume = _volumeSlider.value;
                    break;
                case VolumeType.SFX:
                    _soundManager.SFXVolume = _volumeSlider.value;
                    break;
                case VolumeType.MUSIC:
                    _soundManager.musicVolume = _volumeSlider.value;
                    break;
                default:
                    throw new Exception($"Volume is not supported {volumeType}");
            }
        }
    }
}