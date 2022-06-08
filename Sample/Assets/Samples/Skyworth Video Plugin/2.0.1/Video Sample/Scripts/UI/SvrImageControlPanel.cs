// Copyright 2018 Skyworth VR. All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SvrImageControlPanel : MonoBehaviour
{
    public SvrImagePlayerDemo SvrImagePlayerDemo;
    [SerializeField]
    private Text ImageNameText;
    [SerializeField]
    private Button PreviousBtn;
    [SerializeField]
    private Button NextBtn;
    

    public void ClickPreviousBtn()
    {
        SvrImagePlayerDemo.PlayImageByIndex(-1);
    }

    public void ClickNextBtn()
    {
        SvrImagePlayerDemo.PlayImageByIndex(1);
    }

    public void SetVideoName(string name)
    {
        ImageNameText.text = name;
    }
}

