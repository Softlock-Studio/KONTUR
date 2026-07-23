using TMPro;
using UnityEngine;

namespace Game.UI.House
{
    // Single latest-message toast (Order Label) for assignment/task-failed/activity-aborted
    // notifications — not a scrolling log, just "the most recent thing that happened".
    public sealed class OrdersToastView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private float displaySeconds = 4f;

        private float hideTimer;

        private void Awake()
        {
            if (label != null) label.text = string.Empty;
        }

        private void Update()
        {
            if (hideTimer <= 0f) return;

            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f && label != null) label.text = string.Empty;
        }

        public void Show(string message)
        {
            if (label == null) return;

            label.text = message;
            hideTimer = displaySeconds;
        }
    }
}
