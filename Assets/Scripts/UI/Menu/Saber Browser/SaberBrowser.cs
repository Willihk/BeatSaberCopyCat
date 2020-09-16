using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatGame.Data.Saber;
using BeatGame.UI.Components.Tabs;
using System;
using BeatGame.Logic.Managers;
using System.IO;
using System.Linq;
using MessagePack;
using CustomSaber;

namespace BeatGame.UI.Controllers
{
    public class SaberBrowser : MonoBehaviour
    {
        [SerializeField]
        GameObject entryPrefab;
        [SerializeField]
        Transform entryHolder;
        [SerializeField]
        TabGroup tabGroup;


        void OnEnable()
        {
            if (entryHolder.childCount == 1)
            {
                StartCoroutine(LoadSabersRoutine());
            }

            GameManager.Instance.ActivateSabers(false);
        }

        void OnDisable()
        {
            GameManager.Instance.ActivatePointer();
        }

        public void SaberSelected(int index)
        {
            SaberManager.Instance.SetNewActiveSaber(index);
        }

        IEnumerator LoadSabersRoutine()
        {
            for (int i = 0; i < SaberManager.Instance.LoadedSabers.Count; i++)
            {
                var entryObject = Instantiate(entryPrefab, entryHolder);
                entryObject.GetComponent<SaberEntryController>().Initizalize(SaberManager.Instance.LoadedSabers[i], i);
                entryObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(tabGroup);

                yield return new WaitForSeconds(.2f);
            }
        }
    }
}