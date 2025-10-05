using UnityEngine;
using UnityEngine.UI;

namespace TerribleScripts.ModMods
{
    public class TerribleRangefinder : MonoBehaviour
    {
        [Tooltip("Maximum value the rangefinder can calculate. Everything larger will use Inf String.")]
        public float MaxValue;
        [Tooltip("Text that comes after the rangefinder output w/o leading space (you have to put it yourself).")]
        public string Units;
        [Tooltip("What the output will show if the distance is larger than the MaxValue. Does not include the Units text.")]
        public string InfString;
        [Tooltip("Transform that will raycast forward.")]
        public Transform LaserHit;
        [Tooltip("What the raycast will be able to hit (and calculate distance off of).")]
        public LayerMask LayerMask;
        [Tooltip("Your Text Object.")]
        public Text Text;
        [Tooltip("OPTIONAL: PIP Scope for using color parity between your reticle and the rangefinder display.")]
        public PIPScope PIPScope;
        public bool Override;
        [HideInInspector] public int Color;
        [HideInInspector] public Material Material;
        [HideInInspector] public bool ScopeAvailable;
        [HideInInspector] public int Blend;
        [HideInInspector] public float Distance;

        
        public void Awake()
        {
            if (PIPScope != null) ScopeAvailable = true;
            Color = Shader.PropertyToID("_Color");
            Blend = Shader.PropertyToID("_DstBlend");
            Material = Instantiate(Text.material);
            Text.material = Material;
        }
        public void OnDestroy()
        {
            Destroy(Material);
        }

        public void Update()
        {
            if (!ScopeAvailable) return;
            var ReticleColor = PIPScope.reticleIllumination;
            Material.SetColor(Color, ReticleColor);

        }

        public void FixedUpdate()
        {
            if (Override)
            {
                if (Distance < MaxValue) Text.text = $"{Distance:F0}{Units}";
                else
                {
                    var lerpValue = Mathf.Lerp(Distance, MaxValue + 1f, 1f);
                    Text.text = (lerpValue < MaxValue) ? $"{lerpValue:F0}{Units}" : $"{InfString}";
                }
                return;
            }
            
            RaycastHit RaycastInfo;
            var Hit = Physics.Raycast(LaserHit.position, LaserHit.TransformDirection(Vector3.forward), out RaycastInfo, MaxValue, LayerMask);
            if (Hit)
            {
                Distance = RaycastInfo.distance;
                Text.text = $"{Distance:F0}{Units}";
            }
            else
            {
                var lerpValue = Mathf.Lerp(Distance, MaxValue + 1f, 1f);
                Text.text = (lerpValue < MaxValue) ? $"{lerpValue:F0}{Units}" : $"{InfString}";
            }
        }
    }
}