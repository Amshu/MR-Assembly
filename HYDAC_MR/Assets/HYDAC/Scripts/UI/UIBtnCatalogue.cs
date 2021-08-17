using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.SOCS;

namespace HYDAC.Scripts.UI
{
    public class UIBtnCatalogue : MonoBehaviour
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image productImage;
        [SerializeField] private Button productButton;

        private SCatalogueInfo info;
        private Transform _parent;
        
        public void Initialize(SCatalogueInfo _info, Transform uiParent)
        {
            _parent = uiParent;
            
            info = _info;

            nameText.text = info.iname;
            productImage.sprite = _info.ProductImage;
            
            if(info.isLoadable)
                productButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Debug.Log("#UIBtnCatalogue#------------Catalogue n=button clicked: " + info.iname);
            
            assemblyEvents.OnAssemblySelected(info);
            
            if(info.isLoadable)
                productButton.onClick.RemoveListener(OnButtonClicked);
            
            _parent.gameObject.SetActive(false);
        }
    }
}