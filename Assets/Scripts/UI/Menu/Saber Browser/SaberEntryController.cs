using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BeatGame.Data.Saber;
using TMPro;

namespace BeatGame.UI.Controllers
{
    public class SaberEntryController : MonoBehaviour
    {
        public Image CoverImage;

        public TextMeshProUGUI SaberNameText;
        public TextMeshProUGUI CreatorNameText;

        public CustomSaberInfo SongData;
        public int ValueIndex;

        public void Initizalize(CustomSaberInfo data, int index)
        {
            SongData = data;
            ValueIndex = index;

            SaberNameText.text = data.SaberDescriptor.SaberName;
            CreatorNameText.text = data.SaberDescriptor.AuthorName;
        }
    }
}