using System.Collections;
using NUnit.Framework;
using Players;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Tests;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Actors.Tests
{
    public class UnitManagerTests
    {
        private NavMeshSurface _env;
        
        [SetUp]
        public void SetUp()
        {
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
        public IEnumerator UnitManagerSimpleTests()
        {
            var manager = new UnitManager();
            var model = new UnitModel(TestUtils.GetTestStats(), TestUtils.GetTestAppearance());
            manager.AddUnit(model);
            Assert.AreEqual(1, manager.unitsCount);
            Assert.NotNull(GameObject.FindObjectOfType(typeof(UnitRTSView)));
            yield return null;
        }
    }
}
