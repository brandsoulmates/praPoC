﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;


public class playPauseTextSwitch : MonoBehaviour {

    public TextMesh textObj;
    public MediaPlayer videoPlayer;

    private void Update()
    {
        if (videoPlayer.Control.IsPlaying())
        {
            setToPause();
        }
        if (videoPlayer.Control.IsFinished())
        {
            setToPlay();
        }
    }

    public void setToPlay()
    {
        textObj.text = "Play\nMedia";
    }

    public void setToPause()
    {
        textObj.text = "Pause\nMedia";
    }
}