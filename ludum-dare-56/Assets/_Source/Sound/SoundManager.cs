using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Volume")] [Range(0, 1)] public float masterVolume = 1;
        [Range(0, 1)] public float musicVolume = 1;
        [Range(0, 1)] public float ambienceVolume = 1;
        [Range(0, 1)] public float SFXVolume = 1;

        [field:SerializeField] public FMODEvents FMODEvents { get; private set; }
        
        private Bus masterBus;
        private Bus musicBus;
        private Bus ambienceBus;
        private Bus sfxBus;

        private List<EventInstance> eventInstances = new();

        private EventInstance musicEventInstance;

        [Inject]
        public void Initialize()
        {
            //GetBuses();
        }
        private void Update()
        {
            masterBus.setVolume(masterVolume);
            musicBus.setVolume(musicVolume);
            ambienceBus.setVolume(ambienceVolume);
            sfxBus.setVolume(SFXVolume);
        }
        public void SetMusicArea(MusicAct act)
        {
            musicEventInstance.setParameterByName("act", (float) act);
        }
        public void PlayOneShot(EventReference sound)
        {
            RuntimeManager.PlayOneShot(sound);
        }
        private EventInstance CreateInstance(EventReference eventReference)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstances.Add(eventInstance);
            return eventInstance;
        }

        public void PlayMusicDuringTime(float time, EventReference music)
        {
            var instance = CreateInstance(music);
    
            var wrapper = new EventInstanceWrapper(instance);
    
            wrapper.Instance.start();

            StopMusicAfterTime(wrapper, time).Forget();
        }

        private async UniTask StopMusicAfterTime(EventInstanceWrapper wrapper, float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
    
            wrapper.Instance.stop(STOP_MODE.ALLOWFADEOUT);
    
            wrapper.Instance.release();
    
            eventInstances.Remove(wrapper.Instance);
        }
        public void InitializeMusic(EventReference musicEventReference)
        {
            musicEventInstance = CreateInstance(musicEventReference);
            musicEventInstance.start();
        }
        private void CleanUp()
        {
            foreach (var eventInstance in eventInstances)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        private void OnDestroy()
        {
            CleanUp();
        }
        private void GetBuses()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }
    }
    
    public class EventInstanceWrapper
    {
        public EventInstance Instance { get; }

        public EventInstanceWrapper(EventInstance instance)
        {
            Instance = instance;
        }
    }
}