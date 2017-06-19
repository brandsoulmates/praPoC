﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class triggers : MonoBehaviour {

    public GameObject maleAvatar;
    bool smile;
    public float time;

    [Header("Cursor States and local space converters")]
    public GameObject cursorOri;
    public GameObject cursorHand;
    Vector3 initHandPos;
    Vector3 initLocalPos;

    bool editState;
    float xPos;
    float yPos;

    float tempDist;

    [Tooltip("Object for world orient local space to camera")]
    public GameObject handPosLocal;

    bool navigating;

    float sensitivity;

    public GameObject buttons;
    public List<GameObject> others;

    private void Start()
    {
        initHandPos = new Vector3(0, 0, 0);
        navigating = false;
        editState = false;
        sensitivity = 1.2f;
        tempDist = 0.0f;
        buttons.SetActive(false);
    }

    private void FixedUpdate()
    {
        maleAvatar.GetComponent<LoomDeformerLoomie_male>().targetPosition = Camera.main.transform.position;
        mouthHandManip();
    }

    #region
    private void OnMouseDown()
    {
        makeSmileToggle();
    }

    public void makeSmileToggle()
    {
        //if (!smile)
        //{
        //    StartCoroutine(smoothSmile(Vector3.zero, Vector3.one, time));
        //    smile = true;
        //}else
        //{
        //    StartCoroutine(smoothSmile(Vector3.one, Vector3.zero, time));
        //    smile = false;
        //}
    }

    public IEnumerator smoothSmile(Vector3 start, Vector3 end, float seconds)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            Vector3 tempV = Vector3.Lerp(start, end, Mathf.SmoothStep(0.0f, 1, t));
            maleAvatar.GetComponent<LoomDeformerLoomie_male>().Smile = tempV.x;

            yield return true;
        }
    }
    #endregion

    private void mouthHandManip()
    {
        if (GazeManager.Instance.HitObject == gameObject || navigating)
        {
            if (sourceManager.Instance.sourcePressed)
            {
                if (!navigating)
                {

                    tempDist = Vector3.Distance(Camera.main.transform.position, GazeManager.Instance.HitPosition);

                    initHandPos = HandsManager.Instance.ManipulationHandPosition;
                    initLocalPos = handPosLocal.transform.localPosition;
                    editState = true;
                    navigating = true;
                }
                navigating = true;
                cursorOri.SetActive(false);
                cursorHand.SetActive(true);

                handPosLocal.transform.position = HandsManager.Instance.ManipulationHandPosition - initHandPos;

                handPosLocal.transform.localPosition = (new Vector3(handPosLocal.transform.localPosition.x,
                                                                    handPosLocal.transform.localPosition.y,
                                                                    handPosLocal.transform.localPosition.z) * sensitivity) + initLocalPos;
                xPos = handPosLocal.transform.localPosition.x;
                yPos = handPosLocal.transform.localPosition.y;

                cursorHand.transform.localPosition = new Vector3(xPos, yPos, tempDist / 100 - .025f);
                mouthExpression(initLocalPos);
                othersActiveState(false);
                //buttons.SetActive(true);
            }
            else
            {
                editState = false;
                navigating = false;
                cursorOri.SetActive(true);
                cursorHand.SetActive(false);
                initHandPos = new Vector3(0, 0, 0);
                othersActiveState(true);
                buttons.SetActive(false);
            }
        }
        

    }

    void mouthExpression(Vector3 offset)
    {
        maleAvatar.GetComponent<LoomDeformerLoomie_male>().JawClench = Mathf.Clamp(handPosLocal.transform.localPosition.y * 10, 0, 1) ;
        maleAvatar.GetComponent<LoomDeformerLoomie_male>().JawDrop = Mathf.Clamp(handPosLocal.transform.localPosition.y * -10, 0, 1);

        maleAvatar.GetComponent<LoomDeformerLoomie_male>().JawLeft = Mathf.Clamp(handPosLocal.transform.localPosition.x * 10, 0, 1);
        maleAvatar.GetComponent<LoomDeformerLoomie_male>().JawRight = Mathf.Clamp(handPosLocal.transform.localPosition.x * -10, 0, 1);
    }

    void othersActiveState(bool condition)
    {
        foreach (GameObject obj in others)
        {
            obj.GetComponent<Collider>().enabled = condition;
        }
    }
}
