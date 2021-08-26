using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace HYDAC.Scripts.ADD
{
    public static class AddressableLocationLoader
    {
        internal static async Task LoadFromLabel(string label, IList<IResourceLocation> loadedLocations)
        {
            var unloadedLocations = await Addressables.LoadResourceLocationsAsync(label).Task;

            foreach(var location in unloadedLocations)
            {
                loadedLocations.Add(location);
            }
        }

        internal static async Task LoadLabels(string[] labels, IList<IResourceLocation> loadedLocations)
        {
            foreach(var label in labels)
            {
                IList<IResourceLocation> locationsOfLabel = new List<IResourceLocation>();

                await LoadFromLabel(label, locationsOfLabel);

                Debug.Log("#AddressableLocationLoader#-------Loaded assets with label: " + label);

                foreach (var location in locationsOfLabel)
                    loadedLocations.Add(location);
            }
        }
    }

}
