using UnityEngine;

namespace HYDAC.Scripts.INFO
{
    public abstract class ASInfo : ScriptableObject
    {
        [Tooltip("Uncheck after renaming is done")]
        public bool renameOnValidate = false;
        
        [Space]
        [Header("Main Information")] 
        public int ID;

        public string iname;
        [TextArea] public string description;
        
        // Image?
        // Video?
        // Schematic
        
        protected abstract void ChangeFileName();

        private void OnValidate()
        {
            if (renameOnValidate)
            {
                ChangeFileName();
            }
        }
    }
}
