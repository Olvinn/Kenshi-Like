using UnityEngine;

namespace Units.MVC.View
{
    public class UnitViewFactory : MonoBehaviour
    {
        private const string ActorsName = "--- Actors ---";
        private const string ParentName = "-- Units --";
        
        private static int _index = 0;
        private static Transform _controllers, _parent;
        
        public static UnitView Create()
        {
            if (_parent == null)
                FindOrCreateParent();
            var temp = new GameObject($"UnitController_{_index++}");
            var result = temp.AddComponent<UnitView>();
            result.transform.SetParent(_parent);
            temp = new GameObject("Appearance");
            temp.transform.SetParent(result.transform);
            return result;
        }

        private static void FindOrCreateParent()
        {
            var temp = GameObject.Find(ActorsName);
            if (temp)
                _controllers = temp.transform;
            else
                _controllers = new GameObject(ActorsName).transform;
            temp = GameObject.Find(ParentName);
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
