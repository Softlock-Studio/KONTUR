using Game.House;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class RoomInfectionDemo : MonoBehaviour
{
    [SerializeField] private Zone _zone;
    [Space]

    [Range(0f, 1f)]
    public float infectionAmount = 0f;

    static readonly int InfectionId = Shader.PropertyToID("_InfectionAmount");

    MaterialPropertyBlock mpb;
    Renderer rend;
    float lastValue = -1f;

    private void OnDisable()
    {
        if (_zone != null)
            _zone.InfectionChanged -= OnInfectionChanged;
    }

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        Apply();
    }

    private void Start()
    {
        if(_zone != null)
            _zone.InfectionChanged += OnInfectionChanged;
    }

    void Update()
    {
        if (rend == null || Mathf.Approximately(lastValue, infectionAmount)) return;
        Apply();
    }

    void Apply()
    {

        if(mpb != null)
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat(InfectionId, infectionAmount);
            rend.SetPropertyBlock(mpb);
            lastValue = infectionAmount;
        }
    }

    private void OnInfectionChanged(float Infection)
    {
        infectionAmount = Infection/100f;
    }

#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif
}