using UnityEngine;

namespace Loader.SceneController
{
    // Draws the string field as a dropdown of every .unity scene under Assets/_Project, instead
    // of a free-typed name — see SceneDropdownDrawer (Editor-only) for the actual picker + the
    // Build Settings presence check.
    public sealed class SceneDropdownAttribute : PropertyAttribute
    {
    }
}
