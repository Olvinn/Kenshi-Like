using System.Collections;
using NUnit.Framework;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Units.Tests.PlayTests
{
    public class UnitViewTest
    {
        private UnitModel _model;
        private UnitView _view;
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
        }
        
        [UnityTest]
        public IEnumerator UnitViewMovingTests()
        {
            bool wasReachedDestination = false;
            Vector3 destination = new Vector3(0, 0, 5);
            Vector3 start = Vector3.zero;
            _env.BuildNavMesh();
            var temp = new GameObject();
            _view = temp.AddComponent<UnitView>();
            Assert.NotNull(_view);
            var agent = _view.GetComponent<NavMeshAgent>();
            Assert.NotNull(agent);
            
            _view.SetStats(_model.GetStats());
            Assert.AreEqual(agent.speed, _model.GetStats().speed);
            _view.onReachDestination = () => { wasReachedDestination = true; };
            _view.MoveTo(destination);
            yield return new WaitForSeconds(.1f);
            Assert.AreEqual(MovingStatus.Moving, _view.movingStatus);
            yield return new WaitForSeconds(5);
            Assert.AreEqual(true, wasReachedDestination);
            Assert.Less(Vector3.Distance(_view.transform.position, destination), Vector3.Distance(start, destination));
            Assert.AreEqual(MovingStatus.Staying, _view.movingStatus);
            _view.MoveTo(destination);
            Assert.AreEqual(MovingStatus.Staying, _view.movingStatus);
            _view.MoveTo(start);
            _view.MoveTo(destination);
            Assert.AreEqual(MovingStatus.Staying, _view.movingStatus);
        }
    }
}
