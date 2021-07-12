using UnityEngine;

namespace HYDAC.Scripts.USER
{
    /// <summary>
    /// This script is 
    /// </summary>
    
    
    
    public class NetPlacement : MonoBehaviour
    {
        private void AdjustPosition(int noOfUsers, Vector3 modelPosition, float distanceFromCenter)
        {
            /* Distance around the circle */  
            var radians = 2 * Mathf.PI / _maxNoOfUsers * noOfUsers;

            /* Get the vector direction */ 
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians); 
     
            var spawnDir = new Vector3 (horizontal, 0, vertical);
     
            /* Get the spawn position */ 
            var spawnPos = modelPosition + spawnDir * distanceFromCenter;
                
            Debug.Log("Changing user position to: " + spawnPos);

            transform.position = spawnPos;
            
            /* Rotate the enemy to face towards player */
            transform.LookAt(modelPosition);
        }
    }
}
