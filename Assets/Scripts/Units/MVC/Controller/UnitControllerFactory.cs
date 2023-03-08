using UnityEngine;

namespace Units.MVC.Controller
{
    public static class UnitControllerFactory
    {
        private const string ControllersName = "--- Controllers ---";
        private const string ParentName = "-- Units --";
        
        private static int _index = 0;
        private static Transform _controllers, _parent;
        
        public static NPCUnitController CreateNPC()
        {
            if (_parent == null)
                FindOrCreateParent();
            var temp = new GameObject($"NPCUnitController{_index++}");
            var result = temp.AddComponent<NPCUnitController>();
            result.transform.SetParent(_parent);
            return result;
        }
        
        public static PlayerUnitController CreatePlayer()
        {
            if (_parent == null)
                FindOrCreateParent();
            var temp = new GameObject($"PlayerUnitController{_index++}");
            var result = temp.AddComponent<PlayerUnitController>();
            result.transform.SetParent(_parent);
            return result;
        }

        private static void FindOrCreateParent()
        {
            var temp = GameObject.Find(ControllersName);
            if (temp)
                _controllers = temp.transform;
            else
                _controllers = new GameObject(ControllersName).transform;
            temp = GameObject.Find($"{ControllersName}/{ParentName}");
            if (temp)
                _parent = temp.transform;
            else
            {
                _parent = new GameObject(ParentName).transform;
                _parent.SetParent(_controllers);
            }
        }
    }
}
