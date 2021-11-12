using UnityEngine;

using Photon.Pun;
using TMPro;
using System.Collections;

public class SYS_HyboxMaxi : MonoBehaviour
{
    [Header("Bore Cylinders")]
    [SerializeField] Transform hCylinderFontClevis;
    [SerializeField] Transform vCylinderFontClevis;

    [Header("Manifold Arrows")]
    [SerializeField] Transform manifoldRightArrow1;
    [SerializeField] Transform manifoldRightArrow2;
    [SerializeField] Transform manifoldLeftArrow1;
    [SerializeField] Transform manifoldLeftArrow2;


    [Header("Cylinder Arrows")]
    [SerializeField] Transform hCylinderArrow1;
    [SerializeField] Transform hCylinderArrow2;
    [SerializeField] Transform vCylinderArrow1;
    [SerializeField] Transform vCylinderArrow2;

    [Space]
    [SerializeField] Transform gaugeNeedle;
    [SerializeField] TextMeshProUGUI screenText;

    [Space]
    [SerializeField] AudioSource audioSource;
    [SerializeField] SYS_Switch switchScript;

    [Space]
    [SerializeField] float extendLerpDuration = 11;
    [SerializeField] float retractLerpDuration = 4;
    [SerializeField] float lerpDistance;

    Vector3 _hCylinderStartPos;
    Vector3 _vCylinderStartPos;
    
    
    float startValue = 0;
    float endValue = 10;
    float valueToLerp;

    PhotonView _photonView;
    bool _isRunning;

    EHyboxMaxiSystemState _eState;
    bool _isInOnPosition;

    enum EHyboxMaxiSystemState
    {
        DEFAULT = 0,
        OFF = 1,
        IDLE = 2,
        EXTEND = 3,
        RETRACT = 4,
        RAISE = 5,
        LOWER = 6
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();

        _hCylinderStartPos = hCylinderFontClevis.localPosition;
        _vCylinderStartPos = vCylinderFontClevis.localPosition;

        ChangeSystemState(EHyboxMaxiSystemState.OFF);
    }

    private void OnEnable()
    {
        switchScript.OnSwitchClick += OnToggleSwitch;
    }

    private void OnDisable()
    {
        switchScript.OnSwitchClick -= OnToggleSwitch;
    }

    public void OnToggleSwitch(bool toggle)
    {
        if (!_photonView.IsMine) return;

        var toggleState = (toggle) ? 1 : 0;

        _photonView.RPC("ChangeSwitchState", RpcTarget.All, new object[] { toggleState });
    }
    [PunRPC]
    void ChangeSwitchState(int toggle)
    {
        var toggleState = (toggle == 1) ?  true : false;

        switchScript.ChangeSwitchState(toggleState);
    }


    public void PowerOnMachine(bool toggle)
    {
        if (!_photonView.IsMine || _isRunning == toggle || _isInOnPosition) return;

        _isRunning = toggle;

        if (toggle)
        {
            _photonView.RPC("ChangeSystemStateRPC", RpcTarget.All, new object[] { (int)EHyboxMaxiSystemState.IDLE });
        }
        else
        {
            _photonView.RPC("ChangeSystemStateRPC", RpcTarget.All, new object[] { (int)EHyboxMaxiSystemState.OFF });
        }
    }


    public void OnControlButtonPress(int stateToChangeTo)
    {
        if (!_photonView.IsMine) return;
        
        _photonView.RPC("ChangeSystemStateRPC", RpcTarget.All, new object[] { (int)stateToChangeTo });
    }
    public void OnControlButtonRelease()
    {
        if (!_isRunning || !_photonView.IsMine) return;

        _photonView.RPC("ChangeSystemStateRPC", RpcTarget.All, new object[] { (int)EHyboxMaxiSystemState.IDLE });
    }


    [PunRPC]
    void ChangeSystemStateRPC(int stateToChangeTo)
    {
        ChangeSystemState((EHyboxMaxiSystemState)stateToChangeTo);
    }

    private void ChangeSystemState(EHyboxMaxiSystemState stateToChangeTo)
    {
        StopAllCoroutines();

        switch (stateToChangeTo)
        {
            case EHyboxMaxiSystemState.OFF:

                // Stop audio
                // Reset screen text
                // Reset gauge needle

                audioSource.Stop();
                screenText.text = "";
                gaugeNeedle.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

                manifoldRightArrow1.gameObject.SetActive(false);
                manifoldRightArrow2.gameObject.SetActive(false);
                manifoldLeftArrow1.gameObject.SetActive(false);
                manifoldLeftArrow2.gameObject.SetActive(false);

                hCylinderArrow1.gameObject.SetActive(false);
                hCylinderArrow2.gameObject.SetActive(false);
                vCylinderArrow1.gameObject.SetActive(false);
                vCylinderArrow2.gameObject.SetActive(false);

                break;

            case EHyboxMaxiSystemState.IDLE:

                // Play audio
                // Change screen text
                // Move gauge needle
                // Hide arrows

                audioSource.Play();
                screenText.text = "Ready To Start";
                gaugeNeedle.localRotation = Quaternion.Euler(-30.0f, 0f, 0f);

                manifoldRightArrow1.gameObject.SetActive(false);
                manifoldRightArrow2.gameObject.SetActive(false);
                manifoldLeftArrow1.gameObject.SetActive(false);
                manifoldLeftArrow2.gameObject.SetActive(false);

                hCylinderArrow1.gameObject.SetActive(false);
                hCylinderArrow2.gameObject.SetActive(false);
                vCylinderArrow1.gameObject.SetActive(false);
                vCylinderArrow2.gameObject.SetActive(false);

                break;

            case EHyboxMaxiSystemState.EXTEND:
                Extend();
                break;


            case EHyboxMaxiSystemState.RETRACT:
                Retract();
                break;

            case EHyboxMaxiSystemState.RAISE:

                Raise();
                break;

            case EHyboxMaxiSystemState.LOWER:
                Lower();
                break;
        }

        _eState = stateToChangeTo;
    }


    private void Extend()
    {
        if (!_isRunning) return;

        screenText.text = "Extending";

        // Enable and orient right manifold arrows
        // Enable and orient horizontal cylinder arrows
        // Start coroutine to extend horizontal cylinder

        manifoldRightArrow1.gameObject.SetActive(true);
        manifoldRightArrow1.localRotation = Quaternion.Euler(0f, 0f, 180f);
        manifoldRightArrow2.gameObject.SetActive(true);
        manifoldRightArrow2.localRotation = Quaternion.identity;

        hCylinderArrow1.gameObject.SetActive(true);
        hCylinderArrow1.localRotation = Quaternion.Euler(0f, 0f, 180f);
        hCylinderArrow2.gameObject.SetActive(true);
        hCylinderArrow2.localRotation = Quaternion.identity;

        StartCoroutine(LerpTarget(hCylinderFontClevis,
            _hCylinderStartPos + (Vector3.forward * lerpDistance),
            extendLerpDuration));
    }
    private void Retract()
    {
        if (!_isRunning) return;

        screenText.text = "Retracting";

        // Enable and orient right manifold arrows
        // Enable and orient horizontal cylinder arrows
        // Start coroutine to retract horizontal cylinder

        manifoldRightArrow1.gameObject.SetActive(true);
        manifoldRightArrow1.localRotation = Quaternion.identity;
        manifoldRightArrow2.gameObject.SetActive(true);
        manifoldRightArrow2.localRotation = Quaternion.Euler(0f, 0f, 180f);

        hCylinderArrow1.gameObject.SetActive(true);
        hCylinderArrow1.localRotation = Quaternion.identity;
        hCylinderArrow2.gameObject.SetActive(true);
        hCylinderArrow2.localRotation = Quaternion.Euler(0f, 0f, 180f);

        StartCoroutine(LerpTarget(hCylinderFontClevis, 
            _hCylinderStartPos, 
            retractLerpDuration));
    }

    private void Raise()
    {
        if (!_isRunning) return;

        screenText.text = "Raising";

        // Enable and orient left manifold arrows
        // Enable and orient vertical cylinder arrows
        // Start coroutine to raise vertical cylinder

        manifoldLeftArrow1.gameObject.SetActive(true);
        manifoldLeftArrow1.localRotation = Quaternion.Euler(0f, 0f, 180f);
        manifoldLeftArrow2.gameObject.SetActive(true);
        manifoldLeftArrow2.localRotation = Quaternion.identity;

        vCylinderArrow1.gameObject.SetActive(true);
        vCylinderArrow1.localRotation = Quaternion.Euler(0f, 0f, 180f);
        vCylinderArrow2.gameObject.SetActive(true);
        vCylinderArrow2.localRotation = Quaternion.identity;

        StartCoroutine(LerpTarget(vCylinderFontClevis, 
            _vCylinderStartPos, 
            retractLerpDuration));
    }

    private void Lower()
    {
        if (!_isRunning) return;

        screenText.text = "Ready To Start";

        // Enable and orient left manifold arrows
        // Enable and orient vertical cylinder arrows
        // Start coroutine to lower vertical cylinder

        manifoldLeftArrow1.gameObject.SetActive(true);
        manifoldLeftArrow1.localRotation = Quaternion.identity;
        manifoldLeftArrow2.gameObject.SetActive(true);
        manifoldLeftArrow2.localRotation = Quaternion.Euler(0f, 0f, 180f);

        vCylinderArrow1.gameObject.SetActive(true);
        vCylinderArrow1.localRotation = Quaternion.identity;
        vCylinderArrow2.gameObject.SetActive(true);
        vCylinderArrow2.localRotation = Quaternion.Euler(0f, 0f, 180f);

        StartCoroutine(LerpTarget(vCylinderFontClevis, 
            _vCylinderStartPos + (Vector3.forward * lerpDistance), 
            extendLerpDuration));
    }


    IEnumerator LerpTarget(Transform objectToMove, Vector3 endPosition, float lerpDuration)
    {
        float t = 0;
        Vector3 startPosition = objectToMove.localPosition;

        // Calculate the time required 
        //Vector3 currentPosition = vCylinderFontClevis.localPosition;
        //var timeRequiredToEndPos = lerpDuration;//(lerpDuration * currentPosition.z) / (endPosition.z);

        //float timeElapsed = 0;

        while (t < 1)
        {
            t += Time.deltaTime / lerpDuration;
            objectToMove.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            //vCylinderFontClevis.localPosition = Vector3.Lerp(vCylinderFontClevis.localPosition, endPosition, timeElapsed / timeRequiredToEndPos);
            //timeElapsed += Time.deltaTime;

            yield return null;
        }
    }
}
