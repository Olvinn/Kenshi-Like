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
    public class SystemTests
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
            var temp = new GameObject();
            env.transform.SetParent(temp.transform);
            _env = temp.AddComponent<NavMeshSurface>();
            _env.collectObjects = CollectObjects.Children;
            _env.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            temp = new GameObject();
            _controller = temp.AddComponent<UnitController>();
        } 
        
        [UnityTest]
        public IEnumerator UnitViewMovingTests()
        {
            _env.BuildNavMesh();
            var temp = new GameObject();
            _view = temp.AddComponent<UnitView>();
            
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
            yield return new WaitForSeconds(.1f);
            Assert.LessOrEqual(Vector3.Distance(_model.GetPosition(), destination2), .5f);
        }
    }
}
