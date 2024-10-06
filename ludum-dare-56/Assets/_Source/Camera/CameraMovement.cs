using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
    public class CameraMovement : MonoBehaviour
    {
        private enum Positions
        {
            Middle,
            Left,
            Right
        }
        
        [SerializeField] private float maxLeftPositionX;
        [SerializeField] private float maxRightPositionX;
        [SerializeField] private float edgePadding;
        [SerializeField] private float moveDuration;

        private Vector3 _initialPosition;
        private Positions _currentPosition = Positions.Middle;
        private bool _isMoving;
        private bool _canMove = true;
        private void Start() 
        {
            _initialPosition = transform.position;
        }
        private void Update()
        {
            if (!_canMove)
            {
                return;
            }
            
            if (_isMoving)
            {
                return;
            }
            
            var mousePos = Input.mousePosition;

            if (mousePos.x <= edgePadding)
            { 
                //left
                if (_currentPosition == Positions.Middle)
                {
                    MoveCameraAsync(Positions.Left).Forget();
                }
                if (_currentPosition == Positions.Right)
                {
                    MoveCameraAsync(Positions.Middle).Forget();
                }
            }
            else if (mousePos.x >= Screen.width - edgePadding)
            {
                //right
                if (_currentPosition == Positions.Middle)
                {
                    MoveCameraAsync(Positions.Right).Forget();
                }
                if (_currentPosition == Positions.Left)
                {
                    MoveCameraAsync(Positions.Middle).Forget();
                }
            }
        }
        public void EnableCameraMovement(bool enable)
        {
            _canMove = enable;
        }
        private async UniTask MoveCameraAsync(Positions targetPosition)
        {
            if (_isMoving)
            {
                return;
            }

            _isMoving = true;
            switch (targetPosition)
            {
                case Positions.Left:
                    await transform.DOLocalMoveX(maxLeftPositionX, moveDuration).ToUniTask();
                    _currentPosition = Positions.Left;
                    break;
                
                case Positions.Middle:
                    await transform.DOLocalMoveX(_initialPosition.x, moveDuration).ToUniTask();
                    _currentPosition = Positions.Middle;
                    break;
                
                case Positions.Right:
                    await transform.DOLocalMoveX(maxRightPositionX, moveDuration).ToUniTask();
                    _currentPosition = Positions.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetPosition), targetPosition, null);
            }

            _isMoving = false;
        }
    }
}
