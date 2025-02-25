﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using System.IO;
using RenderHeads.Media.AVProVideo.Demos;
using RenderHeads.Media.AVProVideo;
using UnityEngine.EventSystems;

public class addCommentButton : MonoBehaviour {

    public GameObject addNewCommentWindow;
    public Button exButton;
    public InputField field;
    public GameObject contentParent;
    public GameObject commentSimplePrefab;
    public GameObject commentPhotoPrefab;
    public GameObject commentVideoPrefab;
    public Button addNoteDone;
    public MediaPlayer videoPlayer;
    public List<GameObject> commentHolder;
    public bool isEngaged;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isEngaged)
        {
            if (Input.GetKeyDown("return"))
            {
                if(field.text != "")
                {
                    addOneViolationComment(this);
                }else
                {
                    closeWindow(this);
                }
                isEngaged = false;
            }

        }
	}

    private void OnMouseDown()
    {
        addNoteDone.onClick.RemoveAllListeners();
        addNoteDone.onClick.AddListener(delegate(){addOneViolationComment(this); });

        exButton.onClick.RemoveAllListeners();
        exButton.onClick.AddListener(delegate () { closeWindow(this); });

        addNewCommentWindow.SetActive(true);
        field.ActivateInputField();
        isEngaged = true;
    }

        public void closeWindow(addCommentButton script)
    {
        script.addNoteDone.onClick.RemoveAllListeners();
        script.exButton.onClick.RemoveAllListeners();
        script.field.text = "";
        script.addNewCommentWindow.SetActive(false);
    }

    public void addOneViolationComment(addCommentButton script)
    {
        GameObject newItem;
        newItem = Instantiate(commentSimplePrefab);
        float xOffset = 5 + 540 * script.commentHolder.Count;
        newItem.transform.SetParent(script.contentParent.transform);
        newItem.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -18, 0);
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        script.contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2((commentHolder.Count+1) * 540 + 10,
                                                                            script.contentParent.GetComponent<RectTransform>().rect.height);
        newItem.GetComponent<offsiteFieldItemValueHolder>().content.text = field.text;
        newItem.GetComponent<offsiteFieldItemValueHolder>().user = metaManager.Instance.user;
        newItem.GetComponent<offsiteFieldItemValueHolder>().date = metaManager.Instance.date();

        newItem.GetComponent<offsiteFieldItemValueHolder>().meta.text = metaManager.Instance.date() + " - " + metaManager.Instance.user;

        newItem.GetComponent<offsiteFieldItemValueHolder>().comment.content = newItem.GetComponent<offsiteFieldItemValueHolder>().content.text;
        newItem.GetComponent<offsiteFieldItemValueHolder>().comment.user = metaManager.Instance.user;
        newItem.GetComponent<offsiteFieldItemValueHolder>().comment.date = metaManager.Instance.date();
        newItem.GetComponent<offsiteFieldItemValueHolder>().comment.type = 0;

        //media player stuff
        newItem.GetComponent<offsiteMediaPlayer>().photoMaterial = violationsParentSpawner.Instance.photoMaterial;
        newItem.GetComponent<offsiteMediaPlayer>().mediaWindow = violationsParentSpawner.Instance.offsiteMediaWindow;
        newItem.GetComponent<offsiteMediaPlayer>().mediaPlane = violationsParentSpawner.Instance.mediaPlane;
        newItem.GetComponent<offsiteMediaPlayer>().guidedTargetObj = violationsParentSpawner.Instance.guidedTargetObj;
        newItem.GetComponent<offsiteMediaPlayer>().videoPlayer = violationsParentSpawner.Instance.videoPlayer;
        newItem.GetComponent<offsiteMediaPlayer>().playButton = violationsParentSpawner.Instance.playButton;
        newItem.GetComponent<offsiteMediaPlayer>().hideThis = violationsParentSpawner.Instance.hideThis;
        newItem.GetComponent<offsiteMediaPlayer>().simpleText = violationsParentSpawner.Instance.simpleText;
        newItem.GetComponent<offsiteMediaPlayer>().diffBG = violationsParentSpawner.Instance.diffBG;
        newItem.GetComponent<offsiteMediaPlayer>().commentType = 0;

        //update to database
        int vioInt = this.gameObject.transform.parent.gameObject.GetComponent<violationsCollapseableBox>().vioInt;
        int nodeInt = databaseMan.Instance.values.Location.Equipment[0].Violations[vioInt].nodeIndex;
 
        newItem.GetComponent<offsiteFieldItemValueHolder>().nodeIndex = nodeInt;

        databaseMan.tempComment newComment = new databaseMan.tempComment();
        newComment.content = newItem.GetComponent<offsiteFieldItemValueHolder>().content.text;
        newComment.user = metaManager.Instance.user;
        newComment.date = metaManager.Instance.date();
        newComment.type = 0;

        foreach(databaseMan.NodeClass node in databaseMan.Instance.values.Location.Equipment[0].Nodes)
        {
            if (node.indexNum == nodeInt)
            {
                databaseMan.Instance.commentToClassValueSync(nodeInt, newComment);
            }
        }

        script.commentHolder.Add(newItem);
        violationsParentSpawner.Instance.spawnedVioPrefabs[vioInt].violationMedias.Add(newItem);
        closeWindow(this);
    }

    public virtual GameObject addOneSimple(JU_databaseMan.tempComment comment)
    {
        GameObject newItem;
        newItem = Instantiate(commentSimplePrefab);
        float xOffset = 5 + 540 * commentHolder.Count;
        newItem.transform.SetParent(contentParent.transform);
        newItem.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -18, 0);
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2((commentHolder.Count + 1) * 540 + 10,
                                                                            contentParent.GetComponent<RectTransform>().rect.height);
        newItem.GetComponent<offsiteFieldItemValueHolder>().content.text = comment.content;
        newItem.GetComponent<offsiteFieldItemValueHolder>().user = comment.user;
        newItem.GetComponent<offsiteFieldItemValueHolder>().date = comment.date;
        commentHolder.Add(newItem);
        return newItem;
    }

    public virtual GameObject addOnePhoto(JU_databaseMan.tempComment comment)
    {
        GameObject newItem;
        newItem = Instantiate(commentPhotoPrefab);

        newItem.GetComponent<offsiteFieldItemValueHolder>().path.text = comment.path;

        Material newMat = Instantiate(newItem.GetComponent<offsiteMediaPlayer>().photoMaterial);
        Material newMatUI = Instantiate(newItem.GetComponent<offsiteMediaPlayer>().photoMaterialUI);
        Texture2D targetTexture = new Texture2D(2048, 1152);
        string pathAppend = Path.Combine(Application.persistentDataPath, newItem.GetComponent<offsiteFieldItemValueHolder>().path.text);
        var bytesRead = System.IO.File.ReadAllBytes(pathAppend);
        targetTexture.LoadImage(bytesRead);
        newMat.mainTexture = targetTexture;
        newMatUI.mainTexture = targetTexture;
        newItem.GetComponent<offsiteMediaPlayer>().photoMaterial = newMat;
        newItem.GetComponent<offsiteMediaPlayer>().thumbPlane.GetComponent<Image>().material = newMatUI;
        newItem.GetComponent<offsiteMediaPlayer>().playerPlane.GetComponent<Renderer>().material = newMat;
        newItem.GetComponent<offsiteMediaPlayer>().thumbMat = newMat;

        float xOffset = 5 + 540 * commentHolder.Count;
        newItem.transform.SetParent(contentParent.transform);
        newItem.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -18, 0);
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2((commentHolder.Count + 1) * 540 + 10,
                                                                            contentParent.GetComponent<RectTransform>().rect.height);
        newItem.GetComponent<offsiteFieldItemValueHolder>().user = comment.user;
        newItem.GetComponent<offsiteFieldItemValueHolder>().date = comment.date;
        commentHolder.Add(newItem);
        return newItem;
    }
  
    public virtual  GameObject addOneVideo(JU_databaseMan.tempComment comment)
    {
        GameObject newItem;
        newItem = Instantiate(commentVideoPrefab);

        newItem.GetComponent<offsiteMediaPlayer>().videoPlayer = videoPlayer;
        newItem.GetComponent<offsiteFieldItemValueHolder>().path.text = comment.path;

        videoPlayer.m_VideoPath = comment.path;
        videoPlayer.LoadVideoPlayer();
        newItem.GetComponent<offsiteMediaPlayer>().thumbMat = Instantiate(newItem.GetComponent<offsiteMediaPlayer>().videoMaterialUI);
        //videoPlayer.gameObject.GetComponent<FrameExtract>().activeComment = newItem;
        videoPlayer.gameObject.GetComponent<FrameExtract>().addThumbnail(comment.path,newItem);

        float xOffset = 5 + 540 * commentHolder.Count;
        newItem.transform.SetParent(contentParent.transform);
        newItem.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, -18, 0);
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2((commentHolder.Count + 1) * 540 + 10,
                                                                            contentParent.GetComponent<RectTransform>().rect.height);

        commentHolder.Add(newItem);

        return newItem;
    }
}
