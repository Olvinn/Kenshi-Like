using UnityEngine;

namespace Units.MVC.View
{
    public class UnitViewFactory : MonoBehaviour
    {
        private const string ActorsName = "--- Actors ---";
        private const string ParentName = "-- Units --";
        
        private static int _index = 0;
        private static Transform _actors, _parent;
        
        public static NavMeshUnitView CreateRTS()
        {
            if (_parent == null)
                FindOrCreateParent();
            var temp = new GameObject($"UnitView_{_index++}");
            var result = temp.AddComponent<NavMeshUnitView>();
            result.transform.SetParent(_parent);
            return result;
        }
        
        public static CharacterControllerUnitView CreateThirdPerson()
        {
            if (_parent == null)
                FindOrCreateParent();
            var temp = new GameObject($"Unit3rdPersonView_{_index++}");
            var result = temp.AddComponent<CharacterControllerUnitView>();
            result.transform.SetParent(_parent);
            return result;
        }

        private static void FindOrCreateParent()
        {
            var temp = GameObject.Find(ActorsName);
            if (temp)
                _actors = temp.transform;
            else
                _actors = new GameObject(ActorsName).transform;
            temp = GameObject.Find($"{ActorsName}/{ParentName}");
            if (temp)
                _parent = temp.transform;
            else
            {
                _parent = new GameObject(ParentName).transform;
                _parent.SetParent(_actors);
            }
        }
    }
}
