using UnityEngine;
using UnityEngine.Serialization;

public class ToolDuplicateDeleter : MonoBehaviour
{
    [FormerlySerializedAs("DuplicateParent")] public Transform duplicateParent;
    [FormerlySerializedAs("Trigger")] public bool trigger;
    [FormerlySerializedAs("NoOfChildren")] public int noOfChildren;

    private void OnValidate()
    {
        noOfChildren = transform.childCount;

        Debug.Log("#Eureka#------------------------- " + noOfChildren);

        DeleteDuplicateObjectes();
    }

    private void DeleteDuplicateObjectes()
    {
        int secondChildChildrenCount;
        Transform secondChild;

        for (int i = 0; i < noOfChildren; i++)
        {
            secondChild = transform.GetChild(i);
            secondChildChildrenCount = secondChild.childCount;

            Debug.Log("#Eureka#------------------------- " + secondChildChildrenCount);

            if (secondChildChildrenCount.Equals(2))
            {
                Debug.Log("#Eureka#-------------------------Moving duplicate object");
                secondChild.GetChild(secondChildChildrenCount - 1).parent = duplicateParent;
                //Object.DestroyImmediate(secondChild.GetChild(secondChildChildrenCount - 1), true);
            }
            if (secondChildChildrenCount.Equals(0))
            {
                secondChild.parent = duplicateParent;
            }
        }
    }
}
