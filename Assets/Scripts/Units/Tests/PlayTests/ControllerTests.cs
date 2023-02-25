using System.Collections;
using NUnit.Framework;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Units.Tests.PlayTests
{
    public class ControllerTests
    {
        private UnitModel _model;
        private UnitView _view;
        private UnitController _controller;
        private NavMeshSurface _env;

        [SetUp]
        public void SetUp()
        {
            _model = new UnitModel(TestUtils.GetTestStats(), TestUtils.GetTestAppearance());

            var env = GameObject.CreatePrimitive(PrimitiveType.Plane);
            env.transform.localScale = new Vector3(100, 0, 100);
            var temp = new GameObject("Environment");
            env.transform.SetParent(temp.transform);
            _env = temp.AddComponent<NavMeshSurface>();
            _env.collectObjects = CollectObjects.Children;
            _env.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            _controller = UnitControllerFactory.Create();
            _env.BuildNavMesh();
            temp = new GameObject("View");
            _view = temp.AddComponent<UnitView>();
        } 
        
        [UnityTest]
        public IEnumerator UnitMovingTests()
        {
            _controller.SetModel(_model);
            _controller.SetView(_view);
            
            var agent = _view.GetComponent<NavMeshAgent>();
            Assert.NotNull(agent);
            Assert.AreEqual(agent.speed, _model.GetStats().speed);

            Vector3 destination1 = new Vector3(0, 0, 5), destination2 = new Vector3(5, 0, 0);

            _model.MoveTo(destination1);
            Assert.AreEqual(MovingStatus.Moving, _view.movingStatus);
            yield return new WaitForSeconds(3);
            Assert.AreEqual(MovingStatus.Staying, _view.movingStatus);
            Assert.LessOrEqual(Vector3.Distance(_model.GetPosition(), destination1), .5f);
            _model.SetPosition(destination2);
            Assert.AreEqual(MovingStatus.Staying, _view.movingStatus);
            Assert.LessOrEqual(Vector3.Distance(_model.GetPosition(), destination2), .5f);
        }

        [UnityTest]
        public IEnumerator EventsTests()
        {
            _controller.SetModel(_model);
            _controller.SetView(_view);
            
            _controller.SetModel(new UnitModel(TestUtils.GetTestStats(), TestUtils.GetTestAppearance()));
            _view.WarpTo(new Vector3(0,0,5));

            yield return null;
            
            Assert.AreEqual(Vector3.zero, _model.GetPosition());
        }
    }
}
