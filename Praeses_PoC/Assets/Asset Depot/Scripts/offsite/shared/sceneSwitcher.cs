﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneSwitcher : MonoBehaviour {

    public void launchShared()
    {
        SceneManager.LoadScene("inDeviceOffsiteScene", LoadSceneMode.Single);
    }
}
