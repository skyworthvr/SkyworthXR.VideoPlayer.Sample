using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class SvrImagePlayerDemo : MonoBehaviour
{

    public SvrImagePlayer imagePlayer;
    [SerializeField]
    private string[] ImageUrls;
    [SerializeField]
    private SvrImageControlPanel SvrImageControlPanel;
    [SerializeField]
    private StereoType StereoType;
    
    private int currentPlayingImageIndex;

    private void Awake()
    {

    }

    private void Start()
    {

        if (imagePlayer == null)
            imagePlayer = GetComponent<SvrImagePlayer>();
        SvrImageControlPanel.SvrImagePlayerDemo = this;
        Debug.LogError("Start");
        PlayImageByIndex(0);
    }

    public void PlayImageByIndex(int index)
    {
        Debug.LogError("PlayImageByIndex " + index);
        int imageCount = ImageUrls.Length;
        if (imageCount < 0)
            return;

        if (currentPlayingImageIndex + index > imageCount - 1)
            currentPlayingImageIndex = 0;
        else if (currentPlayingImageIndex + index < 0)
            currentPlayingImageIndex = imageCount - 1;
        else
            currentPlayingImageIndex += index;
        imagePlayer.PreparedPlayImage(ImageUrls[currentPlayingImageIndex]);
        
        // Set image stereo mode.
        if (StereoType == StereoType.LeftRight)
            imagePlayer.SetPlayMode3DLeftRight();
        else if (StereoType == StereoType.TopBottom)
            imagePlayer.SetPlayMode3DTopBottom();
        else
            imagePlayer.SetPlayMode2D();

        string name = ImageUrls[currentPlayingImageIndex];
        if (!name.StartsWith("http://") && !name.StartsWith("https://"))
        {
            name = ImageUrls[currentPlayingImageIndex].Substring(ImageUrls[currentPlayingImageIndex].LastIndexOf('/') + 1);
        }
        SvrImageControlPanel.SetVideoName(name);
    }
}
