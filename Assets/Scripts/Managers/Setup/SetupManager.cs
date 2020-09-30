using UnityEngine;
using System.IO;
using MessagePack;

namespace BeatGame.Logic.Managers
{
    public class SetupManager : MonoBehaviour
    {
        void Start()
        {
            SetupModSupportFile();
        }

        void SetupModSupportFile()
        {
            File.WriteAllText(SettingsManager.Instance.Settings["Other"]["RootFolderPath"].StringValue + "ModSupport.json", MessagePackSerializer.SerializeToJson(new SupportedMod[]
          {
              new SupportedMod
              {
                  ModName = "Noodle Extensions",
                  Supported = 1
              },
              new SupportedMod
              {
                  ModName = "Chroma",
                  Supported = 1
              },
               new SupportedMod
              {
                  ModName = "Mapping Extensions",
                  Supported = 1
              },
          }));
        }
    }
}