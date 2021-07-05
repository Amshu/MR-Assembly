using System;
using UnityEngine;


public class OVRGrabbableExtended : OVRGrabbable
    {
        public new bool allowOffhandGrab
        {
            get { return m_allowOffhandGrab; }
            set { m_allowOffhandGrab = value; }
        }
    }
