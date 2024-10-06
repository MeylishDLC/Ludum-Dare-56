using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Items
{
    public class SoundButton : MonoBehaviour
    {
        public event Action OnSoundPlayed;
        [SerializeField] private float cooldown;

        private bool _isOnCooldown;
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
                        Debug.Log("Play sound");
                        //todo: play sound
                        //todo: change sprite to pressed
                        
                        OnSoundPlayed?.Invoke();
                        StartCooldownAsync(CancellationToken.None).Forget(); 
                    }
                }
            }
        }
        private async UniTask StartCooldownAsync(CancellationToken token)
        {
            _isOnCooldown = true;
            await UniTask.Delay(TimeSpan.FromSeconds(cooldown));
            _isOnCooldown = false;
        }
    }
}
