using UnityEngine;
using System.Collections;
using Valve.VR;
using BeatGame.Logic.VR;

namespace Assets.Scripts.Controllers.VR
{
    public class PointerControllerSwitcher : MonoBehaviour
    {
        [SerializeField]
        Transform leftController;
        [SerializeField]
        Transform rightController;

        [SerializeField]
        Transform pointer;
        [SerializeField]
        SteamInputModule inputModule;

        [SerializeField]
        SteamVR_Action_Boolean interactWithUIAction;



        // Update is called once per frame
        void Update()
        {
            if (interactWithUIAction == null)
                return;

            if (interactWithUIAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                pointer.SetParent(rightController);
                inputModule.m_Source = SteamVR_Input_Sources.RightHand;
                pointer.transform.localPosition = Vector3.zero;
                pointer.transform.localRotation = Quaternion.identity;
            }
            else if (interactWithUIAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                pointer.SetParent(leftController);
                inputModule.m_Source = SteamVR_Input_Sources.LeftHand;
                pointer.transform.localPosition = Vector3.zero;
                pointer.transform.localRotation = Quaternion.identity;
            }
        }
    }
}