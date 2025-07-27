using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace TerribleScripts.Animations
{
    public class AnimPlayOnButtonPress : MonoBehaviour
    {
        [Tooltip("Put your animations in a sequential list; they will play on every press and loop back around to first position")]
        public List<string> Animations;
        [Tooltip("Animation controller thingy on your object")]
        public Animation Animation;
        [Tooltip("Object you're pressing the buttons on")]
        public FVRInteractiveObject Object;
        [Tooltip("Whether the animation should reset on every button press. Leave true if using with fire selectors/other things")]
        public bool Reset = true;
        [Tooltip("Should the animation play a FireSelector sound when it starts?")]
        public bool PlaySound;
        [Tooltip("Which streamlined button should the animation play on?")]
        public TerribleScriptsBase.StreamlinedButton EnumButton = TerribleScriptsBase.StreamlinedButton.AX;
        [Tooltip("Which classic touchpad side should the animation play on?")]
        public TerribleScriptsBase.TouchpadVector EnumVector = TerribleScriptsBase.TouchpadVector.left;
        private int i;
        private FVRFireArm Firearm;
        private bool Button;
        private Vector2 Vector;
        public void Awake()
        {
            if (Object is FVRFireArm)
            { 
                Firearm = Object as FVRFireArm;
            }
            
            switch (EnumVector) // set the needed vector direction
            {
                case TerribleScriptsBase.TouchpadVector.up:
                    Vector = Vector2.up;
                    break;
                case TerribleScriptsBase.TouchpadVector.down:
                    Vector = Vector2.down;
                    break;
                case TerribleScriptsBase.TouchpadVector.left:
                    Vector = Vector2.left;
                    break;
                case TerribleScriptsBase.TouchpadVector.right:
                    Vector = Vector2.right;
                    break;
            }
        }

        public void PlayAnimation()
        {
            for (var j = 0; j < Animations.Count; j++)
            { if (!Reset && Animation.IsPlaying(Animations[j])) return; } //if one of the animations is playing, don't play another one until its done
            Animation.Play(Animations[i]);
            i++;
            if (PlaySound && Object is FVRFireArm)
            { Firearm.PlayAudioEvent(FirearmAudioEventType.FireSelector); }
            if (i >= Animations.Count) i = 0; //make sure the animations played are sequential and looping
        }
        
        public void Update()
        {
            if (Object.m_hand != null)
            {
                
                switch (Object.m_hand.IsInStreamlinedMode)
                {
                    case false:
                    {
                        if (Object.m_hand.Input.TouchpadDown && Object.m_hand.Input.TouchpadAxes.magnitude > 0.1f &&
                            Vector2.Angle(Object.m_hand.Input.TouchpadAxes, Vector) <= 45f)
                        { PlayAnimation(); }
                        break;
                    }
                    case true:
                        //set the needed buttons
                        Button = EnumButton == TerribleScriptsBase.StreamlinedButton.AX ? Object.m_hand.Input.AXButtonDown : Object.m_hand.Input.BYButtonDown;
                        if (Button)
                        { PlayAnimation(); }
                        break;
                }
            }
        }
    }
}