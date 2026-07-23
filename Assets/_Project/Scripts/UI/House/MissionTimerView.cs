using Game.Mission;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    public sealed class MissionTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private TMP_Text nightLabel;

        private MissionManager missionManager;
        private int lastDisplayedSeconds = -1;

        private void Start()
        {
            missionManager = LifetimeScope.Find<MissionScope>().Container.Resolve<MissionManager>();
        }

        // Only touches TMP_Text (layout rebuild + string alloc) when the displayed whole-second
        // value actually changes, not every frame.
        private void Update()
        {
            if (missionManager == null) return;

            int seconds = Mathf.Max(0, Mathf.CeilToInt(missionManager.GetTimer));
            if (seconds == lastDisplayedSeconds) return;

            lastDisplayedSeconds = seconds;
            if (timeLabel != null) timeLabel.text = FormatTime(seconds);
            if (nightLabel != null) nightLabel.text = missionManager.IsEndDay ? "Night over" : string.Empty;
        }

        private static string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
