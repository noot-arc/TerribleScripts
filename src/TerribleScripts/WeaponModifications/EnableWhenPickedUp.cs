using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace TerribleScripts.WeaponModifications
{
    public class ToggleOnGrabRelease : MonoBehaviour
    {
        [Tooltip("Object that you pick up/release")]
        public FVRPhysicalObject Parent;
        [Tooltip("When true, enables objects when parent is held and disables them when not held; Reverse when false")]
        public bool Enable;
        [Tooltip("Objects to toggle")]
        public List<GameObject> Objects;
        [HideInInspector] public bool Held;
        
        public void Hook()
        {
            GM.CurrentSceneSettings.ObjectPickedUpEvent += ObjectPickedUp;
        }

        public void Awake()
        {
            Hook();
        }

        public void ObjectPickedUp(FVRPhysicalObject ParentObject)
        {
            if (ParentObject == Parent)
            {
                for (int i = 0; i < (Objects.Count); i++)
                {
                    Objects[i].SetActive(Enable);
                    Held = true;
                }
            }
        }
        private void Update()
        {
            if (Held && !Parent.m_isHeld)
            {
                for (int i = 0; i < (Objects.Count); i++)
                {
                    Objects[i].SetActive(!Enable);
                }
                Held = false;
            }
        }
    }
}