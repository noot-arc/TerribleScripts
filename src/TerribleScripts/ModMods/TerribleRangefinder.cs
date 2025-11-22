using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace TerribleScripts.ModMods
{
    public class TerribleRangefinder : MonoBehaviour // im not commenting this
    {
        [Tooltip("Maximum value the rangefinder can calculate. Everything larger will use Inf String.")]                       public float MaxDisplayDistance;
        [Tooltip("Text that comes after the rangefinder output w/o leading space (you have to put it yourself).")]             public string Suffix;
        [Tooltip("What the output will show if the distance is larger than the MaxValue. Does not include the Units text.")]   public string MaxDistanceString;
        [Tooltip("What the raycast will be able to hit (and calculate distance off of).")]                                     public LayerMask LayerMask;
        [Tooltip("Your Text Object.")]                                                                                         public Text DistanceDisplay;
        [Tooltip("Point from which rangefinding calculations happen.")]                                                        public Transform RangefinderPos;
        [Tooltip("OPTIONAL: PIPScope controller for color parity & rangefinding at the muzzle.")]                              public PIPScopeController ScopeController;
        [Tooltip("If PIPScope Controller is enabled, would you like to use the reticle's color as the Text color?")]           public bool UseReticleColor;
        [Tooltip("If Scope Controller is set, would u like to use the mounted firearm's muzzle as the Rangefinder Pos?")]      public bool UseMuzzlePos;
        [HideInInspector] public FVRFireArm Firearm;
        [HideInInspector] public int ColorProperty;
        [HideInInspector] public Material MatInstance;
        [HideInInspector] public bool ScopeAvailable;
        [HideInInspector] public float Distance;
        [HideInInspector] public float Timer;
        [HideInInspector] public Transform FallbackPos;

        
        public void Awake()
        {
            if (ScopeController != null) ScopeAvailable = true;
            ColorProperty = Shader.PropertyToID("_Color");
            MatInstance = Instantiate(DistanceDisplay.material);
            DistanceDisplay.material = MatInstance;
            FallbackPos = RangefinderPos;
        }

        public void OnDestroy()
        {
            Destroy(MatInstance);
        }

        public void Update()
        {
            if (!ScopeAvailable) {return;}
            if (ScopeController.Attachment != null && ScopeController.Attachment.curMount != null && ScopeController.Attachment.curMount.Parent is FVRFireArm) // this check is from the scope script idk why its so large
            { Firearm = ScopeController.Attachment.curMount.Parent as FVRFireArm; }
            if (!UseMuzzlePos || Firearm == null) { RangefinderPos = FallbackPos; return;}
            RangefinderPos.position = Firearm.MuzzlePos.position;
            RangefinderPos.rotation = Firearm.MuzzlePos.rotation;
            if (!UseReticleColor) return;
            MatInstance.SetColor(ColorProperty, ScopeController.PScope.reticleIllumination);
        }

        public void FixedUpdate()
        { 
                RaycastHit RaycastInfo;
                var Hit = Physics.Raycast(RangefinderPos.position, RangefinderPos.forward, out RaycastInfo, MaxDisplayDistance, LayerMask);
                if (Hit)
                {
                    Distance = RaycastInfo.distance;
                    DistanceDisplay.text = $"{Distance:F0}{Suffix}";
                    Timer = 0f;
                } 
                else
                {
                    Timer += Time.fixedDeltaTime;
                    Timer = Mathf.Clamp01(Timer);
                    var lerpValue = Mathf.Lerp(Distance, MaxDisplayDistance + 1f, Timer);
                    DistanceDisplay.text = (lerpValue < MaxDisplayDistance) ? $"{lerpValue:F0}{Suffix}" : $"{MaxDistanceString}";
                }
        }
    }
}