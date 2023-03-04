using System;
using System.Linq;
using CustomDebug;
using Players;
using Players.Variants;
using Units.MVC.Model;
using UnityEngine;

namespace Scenes
{
    public class UnitTestingSceneManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private UnitModelSO[] _models;
        [SerializeField] private int _unitCount;
    
        private TestCrowd _manager;

        private void Awake()
        {
            _manager = new TestCrowd();
        }

        private void Start()
        {
            _manager.CreateCrowd(_unitCount, _models.Select(x => x.model).ToArray());
        }

        void Update()
        {
            _manager.Update();
            FPSCounter.DebugDisplayData = $"Units: {_manager.unitsCount}";
            _camera.transform.RotateAround(Vector3.up * _camera.transform.position.y, Vector3.up,  Time.deltaTime * 15f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_camera.transform.position, 30);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_camera.transform.position, 60);
        }
#endif
    }
}
