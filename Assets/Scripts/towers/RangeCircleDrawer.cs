using UnityEngine;

namespace towers
{
    [RequireComponent(typeof(LineRenderer))]
    public class RangeCircleDrawer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private const int SegmentCount = 64;

        public void Initialize(float radius)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            SetupLineRenderer();
            CreateCircle(radius);
            Hide();
        }

        private void SetupLineRenderer()
        {
            // Load material
            var material = Resources.Load<Material>("Materials/RangeCircle");
            if (material == null)
            {
                Debug.LogWarning("RangeCircle material not found, creating default");
                material = new Material(Shader.Find("Unlit/Color"));
                material.color = new Color(0f, 1f, 1f, 0.3f);
            }

            _lineRenderer.material = material;
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.positionCount = SegmentCount + 1;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop = true;
        }

        private void CreateCircle(float radius)
        {
            float angleStep = 360f / SegmentCount;

            for (int i = 0; i <= SegmentCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                // Position at Y = 0.1f to avoid z-fighting with ground
                _lineRenderer.SetPosition(i, new Vector3(x, 0.1f, z));
            }
        }

        public void Show()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = true;
            }
        }

        public void Hide()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
            }
        }
    }
}
