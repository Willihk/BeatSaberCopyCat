using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using UnityEngine.UI;
using Unity.Mathematics;

namespace BeatGame.UI.Controllers
{
    public class HealthBarUIController : MonoBehaviour
    {
        [SerializeField]
        Image barImage;

        public void UpdateBar()
        {
            barImage.fillAmount = math.lerp(barImage.fillAmount, HealthManager.Instance.Health / HealthManager.Instance.MaxHealth, .1f);
            StopAllCoroutines();
            StartCoroutine(SmoothUpdateBar());
        }

        IEnumerator SmoothUpdateBar()
        {
            while (barImage.fillAmount != HealthManager.Instance.Health / HealthManager.Instance.MaxHealth)
            {
                barImage.fillAmount = math.lerp(barImage.fillAmount, HealthManager.Instance.Health / HealthManager.Instance.MaxHealth, .1f);
                yield return null;
            }
        }
    }
}