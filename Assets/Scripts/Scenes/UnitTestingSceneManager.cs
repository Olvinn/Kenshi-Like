using Cameras;
using CustomDebug;
using Players.Variants;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures.Factories;
using UnityEngine;

namespace Scenes
{
    public class UnitTestingSceneManager : MonoBehaviour
    {
        [SerializeField] private Camera3rdPersonController _camera;
        [SerializeField] private int _unitCount;
    
        private TestCrowd _crowd;
        private Unit3rdPersonView _player;

        private void Awake()
        {
            _crowd = new TestCrowd();
        }

        private void Start()
        {
            _crowd.CreateCrowd(_unitCount, new []
            {
                new UnitModel(UnitStatsFactory.CreateRandomZombie(), UnitAppearanceFactory.CreateRandomZombie()),
                new UnitModel(UnitStatsFactory.CreateRandomZombie(), UnitAppearanceFactory.CreateRandomZombie()),
                new UnitModel(UnitStatsFactory.CreateRandomZombie(), UnitAppearanceFactory.CreateRandomZombie()),
                new UnitModel(UnitStatsFactory.CreateRandomZombie(), UnitAppearanceFactory.CreateRandomZombie()),
                new UnitModel(UnitStatsFactory.CreateRandomZombie(), UnitAppearanceFactory.CreateRandomZombie()),
            });

            _player = UnitViewFactory.Create3rdPerson();
            var app = UnitAppearanceFactory.CreateRandomZombie();
            app.skinColor = Color.red;
            var sta = UnitStatsFactory.CreateRandomZombie();
            sta.speed = 2;
            _player.SetAppearance(app);
            _player.SetStats(sta);
            
            _camera.SetCamera(Camera.main);
            _camera.SetTarget(_player.transform);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            _crowd.Update();
            FPSCounter.DebugDisplayData = $"Units: {_crowd.unitsCount}";

            var mov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _player.Move(mov);
            
            var rot = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _camera.Rotate(rot);
            _camera.Scroll(Input.GetAxis("Mouse ScrollWheel"));
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
