using FistVR;

namespace TerribleScripts.ModMods
{
    public class DropCompUpdateButton : FVRInteractiveObject
    {
        public TerribleDropComp DropComp;
        public override void SimpleInteraction(FVRViveHand hand)
        {
            base.SimpleInteraction(hand);
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, ManagerSingleton<SM>.Instance.AudioEvent_AttachmentClick_Minor, transform.position);
            DropComp.UpdateData();
        }
    }
}