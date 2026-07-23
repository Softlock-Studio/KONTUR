using UnityEngine;

namespace Game.Map
{
    public sealed class MapIconBillboard : MonoBehaviour
    {
        [SerializeField] private Vector3 fixedWorldEulerAngles = new Vector3(90f, 0f, 0f);

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(fixedWorldEulerAngles);
        }
    }
}
