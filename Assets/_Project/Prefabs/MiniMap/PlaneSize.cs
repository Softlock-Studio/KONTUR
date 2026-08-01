using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class AutoPlaneMaterialSetup : MonoBehaviour
{
    MaterialPropertyBlock mpb;
    Renderer rend;

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        UpdateProps();
    }

    void Update()
    {
        if (!Application.isPlaying) UpdateProps(); // обновление при движении/масштабировании в редакторе
    }

    void UpdateProps()
    {
        if (rend == null) return;
        Bounds b = rend.bounds; // мировые размеры плейна
        Vector2 size = new Vector2(b.size.x, b.size.z); // если плейн лежит в плоскости XZ — поменяй на нужные оси

        rend.GetPropertyBlock(mpb);
        mpb.SetVector("_PlaneSize", size);
        mpb.SetVector("_WorldOffset", transform.position);
        rend.SetPropertyBlock(mpb);
    }
}