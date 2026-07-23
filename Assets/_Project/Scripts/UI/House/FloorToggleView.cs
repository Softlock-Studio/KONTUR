using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.House
{
    // Temporary floor "system": moves the (orthographic) Map Camera between two preset heights
    // instead of toggling floor GameObjects — see the plan's design decision for why (disabling a
    // floor would freeze that floor's Zone simulation, and this fixes visual overlap for free
    // since orthographic apparent scale doesn't change with distance).
    public sealed class FloorToggleView : MonoBehaviour
    {
        [SerializeField] private MapUiConfig config;
        [SerializeField] private Camera mapCamera;
        [SerializeField] private Button floor1Button;
        [SerializeField] private Button floor2Button;

        private void Awake()
        {
            if (floor1Button != null) floor1Button.onClick.AddListener(ShowFloor1);
            if (floor2Button != null) floor2Button.onClick.AddListener(ShowFloor2);
        }

        public void ShowFloor1() => SetCameraY(config.Floor1ViewY);

        public void ShowFloor2() => SetCameraY(config.Floor2ViewY);

        private void SetCameraY(float y)
        {
            if (mapCamera == null) return;

            Vector3 position = mapCamera.transform.position;
            position.y = y;
            mapCamera.transform.position = position;
        }
    }
}
