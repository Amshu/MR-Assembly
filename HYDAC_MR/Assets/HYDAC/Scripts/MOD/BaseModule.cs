using System;
using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public interface IBaseModule
    {
        void ToggleFocus(bool toggle);
        
        /// <summary>
        ///  This is for the part in focus 
        /// </summary>
        void Reset();
    }
    
    public class BaseModule: AModule, IBaseModule
    {
        private Vector3 _defaultPosition;
        private Vector3 _defaultRotation;
        private Vector3 _defaultScale;

        protected bool isInFocus = false;

        protected virtual void Awake()
        {
            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation.eulerAngles;
            _defaultScale = transform.localScale;
            
            isInFocus = false;
        }

        #region IBASEMODULE IMPLEMENTATION ---------
        
        void IBaseModule.ToggleFocus(bool toggle)
        {
            Debug.Log("#BaseModule#------------------ ToggleFocus: " + toggle + " for: "+ transform.name);

            isInFocus = toggle;

            if(toggle)
                OnFocused();
            else
                OnUnfocused();
        }

        void IBaseModule.Reset()
        {
            OnReset();
        }
        
        #endregion
        
        
        

        #region ABSTRACT PARENT METHODS IMPLEMENTATION--------------------------------------------------------------------

        protected override void OnFocused()
        {
            //if (isInFocus) return;
            
            Debug.Log("#BaseModule#------------------ OnFocused: "+ transform.name);

            isInFocus = true;
        }

        protected override void OnUnfocused()
        {
            //if (!isInFocus) return;

            Debug.Log("#BaseModule#------------------ OnUnFocused: "+ transform.name);

            
            isInFocus = false;

            // Disappear
            StopAllCoroutines();
            StartCoroutine(LerpVector3(transform.localScale,Vector3.zero, 1, result =>
            {
                transform.localScale = result;
            }));
        }
        
        protected override void OnReset()
        {
            if (isInFocus)
                ResetPositionNRotation();
            else
                ResetScale();

            isInFocus = false;
        }


        protected override void ResetScale()
        {
            StopAllCoroutines();
            StartCoroutine(LerpVector3(transform.localScale,_defaultScale, 1, result =>
            {
                transform.localScale = result;
            }));
        }

        protected override void ResetPositionNRotation()
        {
            StopAllCoroutines();
            
            // Reset Position
            StartCoroutine(LerpVector3(transform.position,_defaultPosition, 1, result =>
            {
                transform.position = result;
            }));
            
            
            // Reset rotation
            StartCoroutine(LerpVector3(transform.rotation.eulerAngles,_defaultRotation, 1, result =>
            {
                transform.rotation = Quaternion.Euler(result);
            }));
        }

        protected override IEnumerator LerpVector3(Vector3 start, Vector3 end, float timeTaken, Action<Vector3> updateCall)
        {
            var t = 0f;

            while (t < 1)
            {
                //Debug.Log("#AModule#---------------Lerping from " + start + " to " + end);
                
                t += Time.deltaTime / timeTaken;

                Vector3 result = Vector3.Lerp(start, end, t);
                
                updateCall?.Invoke(result);
                
                yield return null;
            }
        }

        #endregion
    }
}