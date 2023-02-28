using System;
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
        
        public static async Task<GameObject> LoadAsset<T>(string key, Transform parent, Action<T> onLoaded)
        {
            var handle = Addressables.InstantiateAsync(key, parent);
            handle.Completed += (x) =>
            {
                onLoaded?.Invoke(x.Result.GetComponent<T>());
            };
            await handle.Task;
            return handle.Result;
        }
        
        public static async Task<GameObject> LoadAsset(string key, Transform parent, Action<GameObject> onLoaded)
        {
            var handle = Addressables.InstantiateAsync(key, parent);
            handle.Completed += (x) =>
            {
                onLoaded?.Invoke(x.Result);
            };
            await handle.Task;
            return handle.Result;
        }
    }
}
