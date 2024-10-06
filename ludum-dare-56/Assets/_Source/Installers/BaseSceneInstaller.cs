using Gnomes;
using Items;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BaseSceneInstaller : MonoInstaller
    {
        [SerializeField] private Screamer screamer;
        [SerializeField] private Flashlight flashlight;
        public override void InstallBindings()
        {
            BindScreamer();
            BindFlashlight();
        }
        private void BindScreamer()
        {
            Container.Bind<Screamer>().FromInstance(screamer).AsSingle();
        }   
        private void BindFlashlight()
        {
            Container.Bind<Flashlight>().FromInstance(flashlight).AsSingle();
        }
    }
}