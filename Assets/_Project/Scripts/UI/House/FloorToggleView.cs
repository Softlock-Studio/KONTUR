using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.House
{
    // Temporary floor "system": moves the (orthographic) Map Camera between two preset heights
    // instead of toggling floor GameObjects — see the plan's design decision for why (disabling a
    // floor would freeze that floor's Zone simulation, and this fixes visual overlap for free
    // since orthographic apparent scale doesn't change with distance).
    //
    // floorNumberLabel is the dynamic half of the map's "FLOOR N" readout — the static "FLOOR"
    // word next to it is Window.Map.Label.Floor, a separate always-localized TMP authored directly
    // in the prefab (same split as MissionTimerView's "NIGHT " + night number). Just a digit, so
    // it needs no localization of its own.
    public sealed class FloorToggleView : MonoBehaviour
    {
        [SerializeField] private MapUiConfig config;
        [SerializeField] private Camera mapCamera;
        [SerializeField] private Button floor1Button;
        [SerializeField] private Button floor2Button;
        [SerializeField] private TMP_Text floorNumberLabel;

        public static float CurrentMapCameraY { get; private set; }
        public static bool HasActiveFloor { get; private set; }
        public static int CurrentFloorNumber { get; private set; }

        private void Awake()
        {
            if (floor1Button != null) floor1Button.onClick.AddListener(ShowFloor1);
            if (floor2Button != null) floor2Button.onClick.AddListener(ShowFloor2);

            if (mapCamera != null)
            {
                float y = mapCamera.transform.position.y;
                SetCameraY(y);
                SetFloorNumber(InferFloorNumber(y));
            }
        }

        public void ShowFloor1()
        {
            SetCameraY(config.Floor1ViewY);
            SetFloorNumber(1);
        }

        public void ShowFloor2()
        {
            SetCameraY(config.Floor2ViewY);
            SetFloorNumber(2);
        }

        private void SetCameraY(float y)
        {
            if (mapCamera == null) return;

            Vector3 position = mapCamera.transform.position;
            position.y = y;
            mapCamera.transform.position = position;

            CurrentMapCameraY = y;
            HasActiveFloor = true;
        }

        private void SetFloorNumber(int floorNumber)
        {
            CurrentFloorNumber = floorNumber;
            if (floorNumberLabel != null) floorNumberLabel.text = floorNumber.ToString();
        }

        // Seeds the label from whatever height the camera was authored at in the scene (before any
        // button click) by matching it to the nearest of the two presets.
        private int InferFloorNumber(float y)
        {
            return Mathf.Abs(y - config.Floor1ViewY) <= Mathf.Abs(y - config.Floor2ViewY) ? 1 : 2;
        }
    }
}
