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
        [Tooltip("Line Renderers to recolor the Emissions tint of (uses gradients that come immediately after the Laser Light ones")]
        public List<LineRenderer> Lines;
        [Tooltip("Gradients to recolor the graphics with; key position is the fullness factor (e.g 0.2f is 20% capacity remaining)")]
        public List<Gradient> Gradients; /*GradientHDR does nothing. oh well*/
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
        [HideInInspector] public bool LaserLightsAvailable;
        [HideInInspector] public bool LinesAvailable;
        [HideInInspector] public bool GraphicsAvailable;
        [HideInInspector] public int EmissionColor;

        public void Awake()
        {
            if (LaserLights.Count > 0) LaserLightsAvailable = true;
            if (Lines.Count > 0) LinesAvailable = true;
            if (Graphics.Count > 0) GraphicsAvailable = true;
            EmissionColor = Shader.PropertyToID("_EmissionColor"); //so we dont have to do a string lookup
        }
        public void Update() //once again i have 0 clue how to not do this in update
        {
                for (var i = 0; i < FireArm.FChambers.Count; i++) //count amount of loaded chambers for mag-less firearms
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
                
                var j = 0; // make sure we're going down the gradient list. this would benefit from an entire class but oh well. you live and you dont learn

                if (GraphicsAvailable)
                {
                    for (var i = 0; i < (Graphics.Count); i++, j++) // set the colors by taking the color @ the gradient position corresponding to the fraction we just calculated
                    {
                        UIColor = Gradients[j].Evaluate(CurrentPercentage);
                        Graphics[i].color = UIColor;
                    }
                }

                if (LaserLightsAvailable)
                {
                    for (var i = 0; i < (LaserLights.Count); i++, j++) // same thing for the lasers
                    {
                        UIColor = Gradients[j].Evaluate(CurrentPercentage);
                        LaserLights[i].color = UIColor;
                    }
                }

                if (LinesAvailable)
                {
                    for (var i = 0; i < (Lines.Count); i++, j++) // same thing for the line renderers
                    {
                        MaterialPropertyBlock PropBlock = new MaterialPropertyBlock();
                        UIColor = Gradients[j].Evaluate(CurrentPercentage);
                        PropBlock.SetColor(EmissionColor, UIColor);
                        //this is a bodge. it was supposed to be on mats so you dont
                        //have to restrict it to line renderers but property blocks
                        //can only be applied to renderers. the MOMENT i need to
                        //introduce something else to this script is the moment this bodge fails. for now, it stands
                        Lines[i].SetPropertyBlock(PropBlock); 
                    }
                }
                LoadedChambers = 0; //make sure the number doesn't increase after we're done with it
        }
    }
}