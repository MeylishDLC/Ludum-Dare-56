using UnityEngine;

namespace Items
{
    public class Flashlight: MonoBehaviour
    {
        [SerializeField] private GameObject lightObject;
        
        private void Start()
        {
            lightObject.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    TurnOnFlashlight(true);
                }
                else
                {
                    TurnOnFlashlight(false);
                }
            }
            else
            {
                TurnOnFlashlight(false);
            }
        }

        public void TurnOnFlashlight(bool on)
        {
            lightObject.SetActive(on);
        }
    }
}