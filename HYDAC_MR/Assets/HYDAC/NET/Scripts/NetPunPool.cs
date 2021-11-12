using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Photon.Pun;

using HYDAC.Scripts.ADD;

namespace HYDAC.Scripts.NET
{
    [Serializable]
    public class PUNPoolObject
    {
        public AssetReference assetReference;

        [Space]
        public string name;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;

        public void SetSpawnValues(string nameValue, Vector3 pos, Quaternion rot)
        {
            this.name = nameValue;
            this.spawnPosition = pos;
            this.spawnRotation = rot;
        }
    }

    public static class NetPunPool
    {
        private static DefaultPool _punPool;

        public static void Initialise()
        {
            _punPool = PhotonNetwork.PrefabPool as DefaultPool;
        }

        public async static Task<PUNPoolObject> AddObjectToPool(PUNPoolObject punPoolObject)
        {
            var loadedObject = await AddressableLoader.LoadFromReference(punPoolObject.assetReference);

            _punPool.ResourceCache.Add(loadedObject.name, loadedObject);

            punPoolObject.SetSpawnValues(loadedObject.name, loadedObject.transform.position, loadedObject.transform.rotation);

            return punPoolObject;
        }

        public static async Task<PUNPoolObject[]> AddObjectsToPool(PUNPoolObject[] punPoolObjects)
        {
            List<PUNPoolObject> loadedPoolObjects = new List<PUNPoolObject>();

            // Then load and add all the other networked objects
            foreach (PUNPoolObject poolObject in punPoolObjects)
            {
                var intialisedPUNObject = await AddObjectToPool(poolObject);

                loadedPoolObjects.Add(intialisedPUNObject);

                Debug.Log("#NETPunPool#-------------Loaded NetObject to PUN pool: " + poolObject.name + " " + poolObject.name + " " + poolObject.spawnPosition);
            }

            return loadedPoolObjects.ToArray();
        }
    }
}