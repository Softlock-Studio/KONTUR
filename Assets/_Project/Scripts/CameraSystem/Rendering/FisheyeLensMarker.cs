using UnityEngine;

namespace CameraSystem.Rendering
{
    // Opts a camera into the fisheye lens + static-noise pass (FisheyeRendererFeature).
    // Cameras without this marker (Main Camera, Map Camera) render unaffected even
    // though they share the same URP Renderer asset.
    [RequireComponent(typeof(Camera))]
    public class FisheyeLensMarker : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] private float _noiseBlend;

        public float NoiseBlend => _noiseBlend;

        public void SetNoiseBlend(float blend) => _noiseBlend = Mathf.Clamp01(blend);
    }
}
