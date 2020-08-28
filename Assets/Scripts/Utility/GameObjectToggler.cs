using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utility
{
    public class GameObjectToggler : MonoBehaviour
    {
        public void ToggleObject(GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}