using UnityEngine;

namespace Runtime.Battle.Ui.UiBoard
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float dragSpeed = 150f;
        [SerializeField] private float dragThreshold = 0.1f;
        private Transform myTransform;

        private Camera Camera { get; set; }
        private Vector3 Origin { get; set; }
        private bool IsDragging { get; set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            myTransform = transform;
        }

        private void Update()
        {
            HandleMouseDrag();
        }

        private void HandleMouseDrag()
        {
            if (Input.GetMouseButtonUp(1))
            {
                IsDragging = false;
            }

            if (!Input.GetMouseButton(1))
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Origin = Input.mousePosition;
            }

            if (!IsDragging && Vector3.Distance(Origin, Input.mousePosition) > dragThreshold)
            {
                IsDragging = true;
                Origin = Input.mousePosition;
            }

            if (!IsDragging)
            {
                return;
            }

            var difference = Camera.ScreenToWorldPoint(Input.mousePosition) - Camera.ScreenToWorldPoint(Origin);
            myTransform.position -= difference * (dragSpeed * Time.deltaTime);
            Origin = Input.mousePosition;
        }
    }
}