using TMPro;
using UnityEngine;

namespace CustomDebug
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FPSCounter : MonoBehaviour
    {
        public static string DebugDisplayData;
        public static int FPS { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _label;
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
                _frames = 0;
                FPS = _frames;
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
