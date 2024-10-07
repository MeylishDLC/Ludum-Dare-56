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

        [Header("Visuals")] 
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Sprite notPressedSprite;

        private SpriteRenderer _spriteRenderer;
        private bool _isOnCooldown;
        private SoundManager _soundManager;
        
        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = notPressedSprite;
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
            _spriteRenderer.sprite = pressedSprite;
            await UniTask.Delay(TimeSpan.FromSeconds(cooldown), cancellationToken: token);
            _spriteRenderer.sprite = notPressedSprite;
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
