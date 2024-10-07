using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sound;
using UnityEngine;
using Zenject;

namespace Items
{
    public class Tomato: MonoBehaviour
    {
        private enum Sides
        {
            Left,
            Right
        }
        
        public event Action OnTomatoClicked;

        [SerializeField] private float cooldown;
        [SerializeField] private Sides side;

        private bool _isOnCooldown;
        private Animator _animator;
        private SoundManager _soundManager;
        private static readonly int property = Animator.StringToHash("throw");

        [Inject]
        public void Initialize(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        private void Start()
        {
            _animator = GetComponent<Animator>();
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
                        _animator.SetTrigger(property);
                        PlaySound().Forget();
                        
                        OnTomatoClicked?.Invoke();
                        StartCooldownAsync(CancellationToken.None).Forget();
                    }
                }
            }
        }
        private async UniTask PlaySound()
        {
            await UniTask.Delay(420);
            _soundManager.PlayOneShot(side == Sides.Left
                ? _soundManager.FMODEvents.LeftTomato
                : _soundManager.FMODEvents.RightTomato);
        }
        private async UniTask StartCooldownAsync(CancellationToken token)
        {
            _isOnCooldown = true;
            await UniTask.Delay(TimeSpan.FromSeconds(cooldown), cancellationToken: token);
            _isOnCooldown = false;
        }
    }
}