using System;
using UnityEngine;

namespace Data
{
    public class GameContext : MonoBehaviour
    {
        public static GameContext Instance;
        
        [field: SerializeField] public Constants Constants { get; private set; }
        
        [field: SerializeField] public ColorsPreset Colors { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}
