namespace WebAppAtemMini.Models
{
    public interface IAtemSwitcher
    {
        void Connect(AtemSwitcherDataClass data);
        void ChangeInput(AtemSwitcherDataClass data);
        void ChangePreview(AtemSwitcherDataClass data);
        void ChangeTransition(AtemSwitcherDataClass data);
        void ChangeTransitionTime(AtemSwitcherDataClass data);
        void PerformAutoTransition();
        void PerformCut();
        void PerformFTB();
        void PerformMakro(AtemSwitcherDataClass data);
        void SetAudioGain(AtemSwitcherDataClass data);
    }
}
