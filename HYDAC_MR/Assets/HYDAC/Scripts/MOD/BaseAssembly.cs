using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.NET;

namespace HYDAC.Scripts.MOD
{
    public interface IAssembly
    {
        void OnToggleExplode(bool toggle);

        void OnToggleExplodeSubModule(int id);

        AssemblyModule[] GetAssemblyModules();
    }

    public class BaseAssembly : AUnit, IAssembly
    {
        // If you have multiple custom events, it is recommended to define them in the used class
        public const byte OnModuleChangeEventCode = 1;
        
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private AssemblyModule[] modules;
        [SerializeField] private GameObject modelPrefab;

        [Space]
        [SerializeField] private PUNPoolObject[] netObjects;

        public event Action<SModuleInfo> OnModuleSelect;

        private Transform[] _netObjectTransforms;

        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first

        private void Start()
        {
            StartCoroutine(Intialise());
        }

        IEnumerator Intialise()
        {
            var intialisedNetObjects = NetPunPool.AddObjectsToPool(netObjects);
            yield return new WaitUntil(()=>intialisedNetObjects.IsCompleted);

            List<Transform> netObjs = new List<Transform>();

            foreach(var intialisedObject in intialisedNetObjects.Result)
            {
                var netObj = PhotonNetwork.Instantiate(intialisedObject.name, intialisedObject.spawnPosition, intialisedObject.spawnRotation);

                netObjs.Add(netObj.transform);
            }

            _netObjectTransforms = netObjs.ToArray();

         
            Instantiate(modelPrefab, transform).transform.localPosition = new Vector3(0f, 0.815f, 0f);
        }

        public void OnToggleExplode(bool toggle)
        {
            throw new NotImplementedException();
        }

        public void OnToggleExplodeSubModule(int id)
        {
            throw new NotImplementedException();
        }

        public AssemblyModule[] GetAssemblyModules()
        {
            return modules;
        }
    }
}
