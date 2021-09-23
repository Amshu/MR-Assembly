using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace HYDAC.Scripts.NET
{
    public class UIBtnRoomUser : MonoBehaviour
    {
        [SerializeField] SocNetUI uiEvents;
        [SerializeField] TextMeshProUGUI nameTxt;

        [SerializeField] Transform promoteTransform;
        [SerializeField] TextMeshProUGUI promoteBtnTxt;

        Button _promoteBtn;

        bool _isInstructor;
        int _userID;

        private void Awake()
        {
            _promoteBtn =  promoteTransform.GetComponent<Button>();
        }

        private void OnEnable()
        {
            _promoteBtn.clicked += OnPromote;
        }


        private void OnDisable()
        {
            _promoteBtn.clicked -= OnPromote;
        }

        private void OnPromote()
        {
            uiEvents.OnUIRequestPromote();
        }


        public void Initialise(int userID, string userName, bool isInstructor)
        {
            if (userID != _userID) return;

            nameTxt.text = userName;

            if (isInstructor)
            {
                _isInstructor = isInstructor;
                promoteBtnTxt.text = "Instructor";
            }
        }


    }
}
