using Game.AI.Babooshka;
using Game.AI.Employee;
using UnityEngine;

namespace Game.House.AdditionalFunctions
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerDoor : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private bool _isOpen = false;
        [SerializeField] private int _count;

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log(other.name);
            if (other.gameObject.TryGetComponent<BabooshkaController>(out BabooshkaController _) || other.gameObject.TryGetComponent<EmployeeController>(out EmployeeController _))
            {
                _count++;

                if (!_isOpen)
                {
                    _isOpen = true;
                    _animator.SetTrigger("Open");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<BabooshkaController>(out BabooshkaController _) || other.gameObject.TryGetComponent<EmployeeController>(out EmployeeController _))
            {
                _count--;

                if (_isOpen && _count == 0)
                {
                    _isOpen = false;
                    _animator.SetTrigger("Close");
                }
            }
        }
    }
}