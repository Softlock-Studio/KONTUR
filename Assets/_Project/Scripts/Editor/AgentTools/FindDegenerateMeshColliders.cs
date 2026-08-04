using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.EditorTools
{
    // Scans all loaded scenes for MeshCollider components whose shared mesh has
    // no non-degenerate triangle (the "must have at least one non-degenerate
    // triangle" console error gives no clickable object reference, so this finds
    // the offending GameObject(s) directly instead of relying on the mesh name).
    public static class FindDegenerateMeshColliders
    {
        [MenuItem("Tools/AgentTools/Find Degenerate Mesh Colliders")]
        public static void Run()
        {
            int sceneCount = EditorSceneManager.sceneCount;
            int found = 0;

            for (int s = 0; s < sceneCount; s++)
            {
                var scene = EditorSceneManager.GetSceneAt(s);
                if (!scene.isLoaded) continue;

                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var col in root.GetComponentsInChildren<MeshCollider>(true))
                    {
                        var mesh = col.sharedMesh;
                        if (mesh == null)
                        {
                            Debug.LogWarning($"[FindDegenerateMeshColliders] {GetPath(col.transform)} — MeshCollider has no mesh assigned.", col);
                            continue;
                        }

                        if (!HasNonDegenerateTriangle(mesh))
                        {
                            found++;
                            Debug.LogError($"[FindDegenerateMeshColliders] Degenerate collision mesh on '{GetPath(col.transform)}' (mesh: {mesh.name}, verts: {mesh.vertexCount}, tris: {mesh.triangles.Length / 3}).", col);
                        }
                    }
                }
            }

            if (found == 0)
                Debug.Log("[FindDegenerateMeshColliders] No degenerate MeshCollider meshes found in loaded scenes.");
        }

        private static bool HasNonDegenerateTriangle(Mesh mesh)
        {
            var verts = mesh.vertices;
            var tris = mesh.triangles;
            const float epsilon = 1e-9f;

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 a = verts[tris[i]];
                Vector3 b = verts[tris[i + 1]];
                Vector3 c = verts[tris[i + 2]];
                float area = Vector3.Cross(b - a, c - a).sqrMagnitude;
                if (area > epsilon) return true;
            }

            return false;
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}
