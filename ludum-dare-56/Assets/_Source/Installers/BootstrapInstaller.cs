using Sound;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BootstrapInstaller: MonoInstaller
    {
        [SerializeField] private GameObject SoundManagerPrefab;
        public override void InstallBindings()
        {
            BindSoundManager();
        }
        private void BindSoundManager()
        {
            var audioManager = Container.InstantiatePrefabForComponent<SoundManager>(SoundManagerPrefab);
            Container.Bind<SoundManager>().FromInstance(audioManager).AsSingle();
        }
    }
}