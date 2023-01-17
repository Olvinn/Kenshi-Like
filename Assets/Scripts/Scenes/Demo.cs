using Inputs;
using Stages;
using UnityEngine;

namespace Scenes
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private Stage defaultStage;
        
        [SerializeField] private EscapeStage escapeStage;

        private Stage _currentStage;

        private void Start()
        {
            ChangeStage(defaultStage);
        }

        private void OnEnable()
        {
            InputController.Instance.OnEscape = Escape;
            
            escapeStage.OnContinue = Continue;
        }

        private void OnDisable()
        {
            InputController.Instance.OnEscape = null;
            
            escapeStage.OnContinue = null;
        }

        private void ChangeStage(Stage stage)
        {
            if (_currentStage != null)
                _currentStage.Close();
            _currentStage = stage;
            _currentStage.Open();
        }

        private void Escape()
        {
            if (_currentStage == escapeStage)
                ChangeStage(defaultStage);
            else
                ChangeStage(escapeStage);
        }

        private void Continue()
        {
            ChangeStage(defaultStage);
        }
    }
}
