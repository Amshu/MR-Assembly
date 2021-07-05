using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class simulates object "touch" by vibrating the left or right controller when it comes into contact with the object
    /// </summary>
    public class Vibrate : MonoBehaviour
    {
        // Public attributes
        [Range(0, 1)]
        public float amplitude = 0.5f;
        [Range(0, 1)]
        public float frequency = 0.5f;
        [Range(0, 2)]
        public float duration = 0.2f;       // Sets duration of vibration before turning off


        // Private attributes
        private float _TimeElapsed;
        private bool _IsRightHand;
        public bool IsRightHand             
        {
            set { _IsRightHand = value; }   // Accessed by WhiteboardPenController.cs
        }

        private bool _IsVibrating;

        private void Awake()
        {
            _TimeElapsed = 0f;
        }

        private void Update()
        {
            if (!_IsVibrating)
            {
                return;
            }

            if (_TimeElapsed > duration)
            {
                _TimeElapsed = 0f;
                StopVibration();
            }
            else
            {
                _TimeElapsed += Time.deltaTime;
            }
        }

        /// <summary>
        /// Triggers left / right controller vibration
        /// </summary>
        public void TriggerVibration()
        {
            if (_IsRightHand)
            {
                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
            }
            else
            {
                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
            }

            _IsVibrating = true;
        }

        /// <summary>
        /// Triggers left / right controller vibration when a hand holding a tool gets triggered (eg. Allen Key on bleed valve)
        /// </summary>
        /// <param name="righthand"></param>
        public void TriggerVibration(bool righthand)
        {
            _IsRightHand = righthand;
            TriggerVibration();
        }


        /// <summary>
        /// Stops the left / right controller vibration
        /// </summary>
        private void StopVibration()
        {
            float zeroFrequency = 0f;
            float zeroAmplitude = 0f;

            if (_IsRightHand)
            {
                OVRInput.SetControllerVibration(zeroFrequency, zeroAmplitude, OVRInput.Controller.RTouch);
            }
            else
            {
                OVRInput.SetControllerVibration(zeroFrequency, zeroAmplitude, OVRInput.Controller.LTouch);
            }

            _IsVibrating = false;
        }

        #region OnTrigger Events
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("controllerLeft"))
            {
                _IsRightHand = false;
                TriggerVibration();
            }
            else if (other.CompareTag("controllerRight"))
            {
                _IsRightHand = true;
                TriggerVibration();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("controllerLeft"))
            {
                _IsRightHand = false;
                StopVibration();
            }
            else if (other.CompareTag("controllerRight"))
            {
                _IsRightHand = true;
                StopVibration();
            }
        }
        #endregion
    }
}