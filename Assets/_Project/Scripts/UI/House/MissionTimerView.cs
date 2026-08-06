using Game.Bootstrap;
using Game.Localization;
using Game.Mission;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    // "Window.Header.Night" ("NIGHT ") is a separate, static TMP label authored directly in the
    // prefab via LocalizedTextTMP — nightLabel here is only the dynamic part next to it: the
    // night number while the night is running, or a localized "over" once it ends.
    public sealed class MissionTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private TMP_Text nightLabel;

        private MissionManager missionManager;
        private ILocalizationService localization;
        private int lastDisplayedSeconds = -1;
        private bool lastIsEndDay;

        // ILocalizationService is game-wide (GameLifetimeScope), not mission-scoped — same resolve
        // pattern as SettingsPanelView/EmployeeSlotView.
        private ILocalizationService Localization =>
            localization ??= LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<ILocalizationService>();

        private void Start()
        {
            missionManager = LifetimeScope.Find<MissionScope>(gameObject.scene).Container.Resolve<MissionManager>();
            RefreshNightLabel();
        }

        // Only touches TMP_Text (layout rebuild + string alloc) when the displayed whole-second
        // value actually changes, not every frame.
        private void Update()
        {
            if (missionManager == null) return;

            int seconds = Mathf.Max(0, Mathf.CeilToInt(missionManager.GetTimer));
            if (seconds != lastDisplayedSeconds)
            {
                lastDisplayedSeconds = seconds;
                if (timeLabel != null) timeLabel.text = FormatTime(seconds);
            }

            if (missionManager.IsEndDay != lastIsEndDay) RefreshNightLabel();
        }

        private void RefreshNightLabel()
        {
            lastIsEndDay = missionManager != null && missionManager.IsEndDay;
            if (nightLabel == null) return;

            nightLabel.text = lastIsEndDay
                ? Localization.Localize("Window.Header.NightOver")
                : missionManager.CurrentNight.ToString();
        }

        private static string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
