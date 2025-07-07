using FistVR;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace TerribleScripts.ModMods
{
    public class AmmoCountUIColorShenanigans : MonoBehaviour
    {
        [Header("Note: There should be one gradient per object with them having")]
        [Header("the same positions (topmost object will use the topmost gradient, for example)")]
        [Tooltip("Graphics to recolor")] 
        public List<Graphic> Graphics;
        [Tooltip("Lasers & Lights to recolor (Uses gradients that come immediately after the graphics ones)")]
        public List<LaserLight> LaserLights;
        [Tooltip("Gradients to recolor the graphics with; key position is the fullness factor (e.g 0.2f is 20% capacity remaining)")]
        public List<Gradient> Gradients;
        [Tooltip("Firearm :)")]
        public FVRFireArm FireArm;
        [Tooltip("Does your firearm have/uses magazines or is it direct chamber-loaded?")]
        public bool UsesMags;
        // [Tooltip("Should the firearm play a sound when the percentage of rounds left is below a certain amount?")]
        // public bool PlaysAudio;
        // [Tooltip("AudioClip to play below a certain percentage (Multiple for randomisation)")]
        // public List<AudioClip> AudioNotifications;
        // [Tooltip("Percentage below which the audio notification sound will play")]
        // [Range(0f, 1f)]
        // public float AudioPercentage;
        [HideInInspector] public int MaxCap;
        [HideInInspector] public float CurrentAmount;
        [HideInInspector] public int LoadedChambers;
        [HideInInspector] public float CurrentPercentage;
        [HideInInspector] public Color UIColor;
        [HideInInspector] public bool DelayFlip = true;

        public void Update() //once again i have 0 clue how to not do this in update
        {
            DelayFlip = !DelayFlip;
            if (DelayFlip)
            {
                for (int i = 0; i < FireArm.FChambers.Count; i++) //count amount of loaded chambers for mag-less firearms
                {
                    if (FireArm.FChambers[i].IsFull)
                    {
                        LoadedChambers++;
                    }
                }
                if (FireArm.Magazine != null) // sets max and current amounts on firearms with magazines
                {
                    MaxCap = FireArm.Magazine.m_capacity;
                    CurrentAmount = FireArm.Magazine.m_numRounds;
                }
                else // same thing, only on mag-less firearms
                {
                    MaxCap = UsesMags ? FireArm.FChambers.Count : 1;
                    CurrentAmount = LoadedChambers;
                }
                var UnclampedPercentage  = CurrentAmount / MaxCap; // make the current amount into a fraction from 0 to 1
                CurrentPercentage = Mathf.Clamp01(UnclampedPercentage); // make sure it doesn't escape any bounds
            
                if (DelayFlip)
                {
                    for (int i = 0; i < (Graphics.Count); i++) // set the colors by taking the color @ the gradient position corresponding to the fraction we just calculated
                    {
                        UIColor = Gradients[i].Evaluate(CurrentPercentage);
                        Graphics[i].color = UIColor;
                    }
                    for (int i = 0; i < (LaserLights.Count); i++) // same thing for the lasers
                    {
                        UIColor = Gradients[i + (Graphics.Count)].Evaluate(CurrentPercentage);
                        LaserLights[i].color = UIColor;
                    }
                }
            
                LoadedChambers = 0; //make sure the number doesn't increase after we're done with it
            }
        }
    }
}