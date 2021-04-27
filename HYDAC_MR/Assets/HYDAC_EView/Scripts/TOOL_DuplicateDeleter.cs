using UnityEngine;

public class TOOL_DuplicateDeleter : MonoBehaviour
{
    public Transform DuplicateParent;
    public bool Trigger;
    public int NoOfChildren;

    private void OnValidate()
    {
        NoOfChildren = transform.childCount;

        Debug.Log("#Eureka#------------------------- " + NoOfChildren);

        DeleteDuplicateObjectes();
    }

    private void DeleteDuplicateObjectes()
    {
        int secondChildChildrenCount;
        Transform secondChild;

        for (int i = 0; i < NoOfChildren; i++)
        {
            secondChild = transform.GetChild(i);
            secondChildChildrenCount = secondChild.childCount;

            Debug.Log("#Eureka#------------------------- " + secondChildChildrenCount);

            if (secondChildChildrenCount.Equals(2))
            {
                Debug.Log("#Eureka#-------------------------Moving duplicate object");
                secondChild.GetChild(secondChildChildrenCount - 1).parent = DuplicateParent;
                //Object.DestroyImmediate(secondChild.GetChild(secondChildChildrenCount - 1), true);
            }
            if (secondChildChildrenCount.Equals(0))
            {
                secondChild.parent = DuplicateParent;
            }
        }
    }
}
