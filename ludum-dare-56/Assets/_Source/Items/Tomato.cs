using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Items
{
    public class Tomato: MonoBehaviour
    {
        public event Action OnTomatoClicked;

        [SerializeField] private float cooldown;

        private bool _isOnCooldown;
        private Animator _animator;
        private static readonly int property = Animator.StringToHash("throw");
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
                        Debug.Log("Tomato thrown");
                        _animator.SetTrigger(property);
                        OnTomatoClicked?.Invoke();
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
    }
}