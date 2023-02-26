using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AssetsManagement
{
    public static class AssetsManager
    {
        public static async Task<GameObject> LoadAsset(string key)
        {
            return Addressables.InstantiateAsync(key).Result;
        }
        
        public static async Task<GameObject> LoadAsset(string key, Transform parent)
        {
            return Addressables.InstantiateAsync(key, parent).Result;
        }
    }
}
