using Inputs;
using UnityEngine;

namespace UserInterfaces
{
    [RequireComponent(typeof(RectTransform))]
    public class BoxSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform myTransform;

        private Vector2 _pos, _size;
        private void Awake()
        {
            if (myTransform == null)
                myTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            InputController.Instance.OnDrawBox += DrawBox;
            myTransform.sizeDelta = Vector2.zero;
        }

        private void LateUpdate()
        {
            myTransform.position =  _pos;
            myTransform.sizeDelta = _size;
            _pos = Vector2.zero;
            _size = Vector2.zero;
        }

        void DrawBox(Vector2 pos, Vector2 size)
        {
            _pos = pos;
            _size = size;
        }
    }
}
