using System.Collections;
using NUnit.Framework;
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
        private UnitStats _stats;
        private UnitView _view;
        private NavMeshSurface _env;

        [SetUp]
        public void SetUp()
        {
            _stats = TestUtils.GetTestStats();

            var env = GameObject.CreatePrimitive(PrimitiveType.Plane);
            env.transform.localScale = new Vector3(100, 0, 100);
            var temp = new GameObject();
            env.transform.SetParent(temp.transform);
            _env = temp.AddComponent<NavMeshSurface>();
            _env.collectObjects = CollectObjects.Children;
            _env.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            _env.BuildNavMesh();
        }
        
        [UnityTest]
        public IEnumerator UnitViewMovingTests()
        {
            bool wasReachedDestination = false;
            Vector3 destination = new Vector3(0, 0, 5);
            Vector3 start = Vector3.zero;
            var temp = new GameObject();
            _view = temp.AddComponent<UnitView>();
            Assert.NotNull(_view);
            var agent = _view.GetComponent<NavMeshAgent>();
            Assert.NotNull(agent);
            
            _view.SetStats(_stats);
            Assert.AreEqual(agent.speed, _stats.speed);
            _view.onReachDestination = () => { wasReachedDestination = true; };
            _view.MoveTo(destination);
            yield return new WaitForSeconds(.1f);
            Assert.AreEqual(MovingStatus.Moving, _view.movingState);
            yield return new WaitForSeconds(5);
            Assert.AreEqual(true, wasReachedDestination);
            Assert.Less(Vector3.Distance(_view.transform.position, destination), Vector3.Distance(start, destination));
            Assert.AreEqual(MovingStatus.Staying, _view.movingState);
            _view.MoveTo(destination);
            Assert.AreEqual(MovingStatus.Staying, _view.movingState);
            _view.MoveTo(start);
            _view.MoveTo(destination);
            Assert.AreEqual(MovingStatus.Staying, _view.movingState);
        }
    }
}
