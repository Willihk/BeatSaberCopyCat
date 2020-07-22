using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongEntryController : MonoBehaviour
{
    public Image CoverImage;

    public TextMeshProUGUI SongNameText;
    public TextMeshProUGUI ArtistNameText;

    public AvailableSongData SongData;


    public void Initizalize(AvailableSongData songData)
    {
        SongData = songData;

        SongNameText.text = songData.SongInfoFileData.SongName;
        ArtistNameText.text = songData.SongInfoFileData.SongAuthorName;

        byte[] imageData = File.ReadAllBytes(songData.DirectoryPath + $@"\{songData.SongInfoFileData.CoverImageFilename}");
        var tempTexture = new Texture2D(2, 2);
        tempTexture.LoadImage(imageData);
        CoverImage.sprite = Sprite.Create(tempTexture, new Rect(0,0, tempTexture.width, tempTexture.height), new Vector2());
    }
}
