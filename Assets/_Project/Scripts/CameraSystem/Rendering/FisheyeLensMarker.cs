using UnityEngine;

namespace CameraSystem.Rendering
{
    // Attach to a security-camera GameObject (the Camera referenced by GameCamera as
    // _correspondingCamera) to opt it into the fisheye lens pass. Cameras without this
    // marker (Main Camera, Map Camera) render unaffected even though they share the
    // same URP Renderer asset.
    [RequireComponent(typeof(Camera))]
    public class FisheyeLensMarker : MonoBehaviour
    {
    }
}
