using Gnomes;
using Items;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BaseSceneInstaller : MonoInstaller
    {
        [SerializeField] private Screamer screamer;
        public override void InstallBindings()
        {
            BindScreamer();
        }

        private void BindScreamer()
        {
            Container.Bind<Screamer>().FromInstance(screamer).AsSingle();
        }
    }
}