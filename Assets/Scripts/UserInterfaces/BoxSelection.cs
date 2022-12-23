using Inputs;
using UnityEngine;

namespace UserInterfaces
{
    [RequireComponent(typeof(RectTransform))]
    public class BoxSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform myTransform;

        private Vector2 _start, _end;
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
            Vector2 pos = new Vector2(_end.x < _start.x ? _end.x : _start.x, _end.y < _start.y ? _end.y : _start.y);
            Vector2 size = new Vector2(Mathf.Abs(_end.x - _start.x), Mathf.Abs(_end.y - _start.y));
            myTransform.position =  pos;
            myTransform.sizeDelta = size;
            _start = Vector2.zero;
            _end = Vector2.zero;
        }

        void DrawBox(Vector2 start, Vector2 end)
        {
            _start = start;
            _end = end;
        }
    }
}
