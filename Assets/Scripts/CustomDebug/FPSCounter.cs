using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomDebug
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FPSCounter : MonoBehaviour
    {
        public static string DebugDisplayData;
        public static int FPS { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Image _color;
        [SerializeField] private Gradient _gradient;
        private int _frames, _savedSec;

        private void Awake()
        {
            if (_label == null)
                _label = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            if ((int)Time.time != _savedSec)
            {
                _savedSec = (int)Time.time;
                _label.text = $"FPS: {_frames}\n{DebugDisplayData}";;
                FPS = _frames;

                _color.color = _gradient.Evaluate(Mathf.Min(_frames, 144f) / 144f);
                
                _frames = 0;
            }

            _frames++;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_label == null)
                _label = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}
