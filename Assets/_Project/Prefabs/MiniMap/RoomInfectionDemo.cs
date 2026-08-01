using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class RoomInfectionDemo : MonoBehaviour
{
    [Range(0f, 1f)]
    public float infectionAmount = 0f;

    static readonly int InfectionId = Shader.PropertyToID("_InfectionAmount");

    MaterialPropertyBlock mpb;
    Renderer rend;
    float lastValue = -1f;

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        Apply();
    }

    void Update()
    {
        if (rend == null || Mathf.Approximately(lastValue, infectionAmount)) return;
        Apply();
    }

    void Apply()
    {
        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(InfectionId, infectionAmount);
        rend.SetPropertyBlock(mpb);
        lastValue = infectionAmount;
    }

#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif
}