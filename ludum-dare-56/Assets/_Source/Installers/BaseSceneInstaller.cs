using Camera;
using Core;
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
        [SerializeField] private NightTimeTracker nightTimeTracker;
        [SerializeField] private CameraMovement cameraMovement;
        public override void InstallBindings()
        {
            BindScreamer();
            BindFlashlight();
            BindNightTimeTracker();
            BindCameraMovement();
        }
        private void BindScreamer()
        {
            Container.Bind<Screamer>().FromInstance(screamer).AsSingle();
        }   
        private void BindFlashlight()
        {
            Container.Bind<Flashlight>().FromInstance(flashlight).AsSingle();
        }
        private void BindNightTimeTracker()
        {
            Container.Bind<NightTimeTracker>().FromInstance(nightTimeTracker).AsSingle();
        }
        private void BindCameraMovement()
        {
            Container.Bind<CameraMovement>().FromInstance(cameraMovement).AsSingle();
        }
    }
}