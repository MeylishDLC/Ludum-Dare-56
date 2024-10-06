using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sound;
using UnityEngine;
using Zenject;

namespace Items
{
    public class SoundButton : MonoBehaviour
    {
        private enum Sides
        {
            Left,
            Right
        }
        
        public event Action OnSoundPlayed;
        [SerializeField] private float cooldown;
        [SerializeField] private Sides side;

        private bool _isOnCooldown;
        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (!_isOnCooldown)
                    {
                        //todo: change sprite to pressed
                        PlaySound();
                        OnSoundPlayed?.Invoke();
                        StartCooldownAsync(CancellationToken.None).Forget(); 
                    }
                }
            }
        }
        private async UniTask StartCooldownAsync(CancellationToken token)
        {
            _isOnCooldown = true;
            await UniTask.Delay(TimeSpan.FromSeconds(cooldown), cancellationToken: token);
            _isOnCooldown = false;
        }
        private void PlaySound()
        {
            _soundManager.PlayOneShot(side == Sides.Left
                ? _soundManager.FMODEvents.LeftButton
                : _soundManager.FMODEvents.RightButton);
        }
    }
}
