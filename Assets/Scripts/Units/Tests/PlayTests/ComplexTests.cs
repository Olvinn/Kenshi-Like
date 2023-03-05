using System.Collections;
using NUnit.Framework;
using Units.Commands;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Units.Tests.PlayTests
{
    public class ComplexTests
    {
        private UnitModel _model;
        private UnitRTSView _view;
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
            _view = temp.AddComponent<UnitRTSView>();
        }
        
        [UnityTest]
        public IEnumerator  MovementCompleteCheck()
        {
            bool check = false;
            _controller.SetUp(_model, _view);
            _controller.onCommandsComplete += () => { check = true; };
            _controller.AddCommand(new UnitCommandMove(new Vector3(0,0,5)));
            Assert.AreEqual(true, _controller.isBusy);
            yield return new WaitForSeconds(3);
            Assert.AreEqual(false, _controller.isBusy);
            Assert.AreEqual(true, check);
        }

        [TearDown]
        public void CleanUp()
        {
            GameObject.Destroy(_view.gameObject);
            GameObject.Destroy(_env.gameObject);
            GameObject.Destroy(_controller.gameObject);
        }
    }
}
