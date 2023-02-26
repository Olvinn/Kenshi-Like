using Players;
using Units.MVC.Model;
using Units.Tests;
using UnityEngine;

namespace Scenes
{
    public class UnitTestingSceneManager : MonoBehaviour
    {
        private UnitManager _manager;

        private void Awake()
        {
            _manager = new UnitManager();
        }

        void Start()
        {
            var model = new UnitModel(TestUtils.GetTestStats(), TestUtils.GetTestAppearance());
            _manager.AddUnit(model);
            _manager.Move(Vector3.forward * 10);
        }

        void Update()
        {
        
        }
    }
}
