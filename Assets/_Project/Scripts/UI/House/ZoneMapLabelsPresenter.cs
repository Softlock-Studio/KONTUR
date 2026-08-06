using System.Collections.Generic;
using Game.House;
using Game.House.Presentation;
using Game.Mission;
using Game.House.Model;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.House
{
    // Live "NN%" infection labels floating above each Zone, visible on the Map RawImage feed via
    // the top-down Map Camera (culling mask = MapIcon layer only — see TagManager.asset). Spawned
    // procedurally, no prefab: TMP Settings has a default font asset, so a 3D TextMeshPro needs no
    // authored asset to render correctly.
    public sealed class ZoneMapLabelsPresenter : MonoBehaviour
    {
        private const string MapIconLayerName = "MapIcon";

        [SerializeField] private float labelHeight = 1.5f;
        [SerializeField] private Vector3 labelEuler = new Vector3(90f, 0f, 0f);
        [SerializeField] private float fontSize = 3f;
        [SerializeField] private Color labelColor = Color.white;
        [SerializeField] private TMP_FontAsset labelFont;

        private readonly Dictionary<ZoneId, TextMeshPro> labels = new();

        private ZoneRegistry registry;
        private Dictionary<ZoneId, Zone> zonesById;

        // Lazy for the same reason ResourceGridPresenter's resolver is lazy: HousePresenter (a
        // VContainer entry point) can call into RenderZones/UpdateZone as part of its own Start(),
        // which can run before this MonoBehaviour's Start().
        private Dictionary<ZoneId, Zone> ZonesById
        {
            get
            {
                if (zonesById != null) return zonesById;

                registry = LifetimeScope.Find<MissionScope>(gameObject.scene).Container.Resolve<ZoneRegistry>();
                zonesById = new Dictionary<ZoneId, Zone>();
                foreach (Zone zone in registry.Zones)
                    zonesById[ZoneId.From(zone)] = zone;

                return zonesById;
            }
        }

        public void Render(IReadOnlyList<ZoneViewState> zones)
        {
            foreach (ZoneViewState zone in zones)
                UpdateItem(zone);
        }

        public void UpdateItem(ZoneViewState zone)
        {
            if (!labels.TryGetValue(zone.Id, out TextMeshPro label))
            {
                label = TrySpawnLabel(zone.Id);
                if (label == null) return;

                labels[zone.Id] = label;
            }

            label.text = $"{zone.InfectionPercent:F0}%";
        }

        private TextMeshPro TrySpawnLabel(ZoneId zoneId)
        {
            if (!ZonesById.TryGetValue(zoneId, out Zone zone) || zone == null) return null;
            if (!zone.IncludeInInfectionStats) return null;

            var labelObject = new GameObject("Infection Label");
            labelObject.layer = LayerMask.NameToLayer(MapIconLayerName);
            labelObject.transform.SetParent(zone.transform, false);
            labelObject.transform.localPosition = Vector3.up * labelHeight;
            labelObject.transform.localEulerAngles = labelEuler;

            var label = labelObject.AddComponent<TextMeshPro>();
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = fontSize;
            label.font = labelFont;
            label.color = labelColor;

            return label;
        }
    }
}
