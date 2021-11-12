using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerManager : MonoBehaviour
{
    [SerializeField] 

    private Transform _headTransform;

    private void Awake()
    {
        _headTransform = Camera.main.transform;


    }
}
