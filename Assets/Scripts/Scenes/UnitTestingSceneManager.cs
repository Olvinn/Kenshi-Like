using CustomDebug;
using Players;
using Units.MVC.Model;
using UnityEngine;

namespace Scenes
{
    public class UnitTestingSceneManager : MonoBehaviour
    {
        [SerializeField] private UnitModelSO[] _models;
        [SerializeField] private int _unitCount;
    
        private UnitManager _manager;
        private int _i;

        private void Awake()
        {
            _manager = new UnitManager();
        }

        void Update()
        {
            if (_i >= _unitCount)
                return;
            
            var temp = _models[Random.Range(0, _models.Length)].model;
            var model = new UnitModel(temp.GetStats(), temp.GetAppearance());
            model.SetPosition(new Vector3(Random.Range(-50,50),0,Random.Range(-50,50)));
            _manager.AddUnit(model);
            _manager.Move(model,new Vector3(Random.Range(-50,50),0,Random.Range(-50,50)));
            _i++;

            FPSCounter.DebugDisplayData = $"Units: {_manager.unitsCount}";
        }
    }
}
