using Cameras;
using CustomDebug;
using Players.Variants;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures.Factories;
using UnityEngine;

namespace Scenes
{
    public class UnitTestingSceneManager : MonoBehaviour
    {
        [SerializeField] private CameraThirdPersonController _camera;
        [SerializeField] private int _unitCount;
    
        private TestCrowd _crowd;
        private PlayerUnitController _player;

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

            _player = UnitControllerFactory.CreatePlayer();
            var app = UnitAppearanceFactory.CreateRandomZombie();
            app.skinColor = Color.red;
            var sta = UnitStatsFactory.CreateRandomZombie();
            sta.speed = 2;
            var model = new UnitModel(sta, app);
            var view = UnitViewFactory.CreateThirdPerson();
            _player.SetUp(model, view);
            
            _camera.SetCamera(Camera.main);
            _camera.SetTarget(view.transform);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            _crowd.Update();
            FPSCounter.DebugDisplayData = $"Units: {_crowd.unitsCount}";
            
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
