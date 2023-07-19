using BMDSwitcherAPI;
using System.Runtime.InteropServices;
using static System.TimeZoneInfo;


namespace WebAppAtemMini.Models
{
    public class AtemSwitcher : IAtemSwitcher

    {
        // Atem Switcher 
        private IBMDSwitcher switcher;

        // Přechody
        private IBMDSwitcherMixEffectBlock me0;
        private IBMDSwitcherTransitionParameters me0TransitionParams;
        private IBMDSwitcherTransitionWipeParameters me0WipeTransitionParams;
        private IBMDSwitcherTransitionDipParameters me0DipTransitionParams;
        private IBMDSwitcherTransitionMixParameters me0MixTransitionParams;
        private IBMDSwitcherTransitionDVEParameters me0DVETransitionParams;
        private IBMDSwitcherFairlightAudioMixer m_fairlightAudioMixer;

        public AtemSwitcher()
        {
            // nothing to do
        }

        public void Connect(AtemSwitcherDataClass data)
        {
            // Připojení
            IBMDSwitcherDiscovery discovery = new CBMDSwitcherDiscovery();
            discovery.ConnectTo(data.ip, out IBMDSwitcher switcher, out _BMDSwitcherConnectToFailure failureReason);

            // Vytvoření switcheru
            this.switcher = switcher;

            // Vytvoření přechodů a jejich parametrů a zvuku
            me0 = MixEffectBlocks.First();
            me0TransitionParams = me0 as IBMDSwitcherTransitionParameters;
            me0WipeTransitionParams = me0 as IBMDSwitcherTransitionWipeParameters;
            me0DipTransitionParams = me0 as IBMDSwitcherTransitionDipParameters;
            me0MixTransitionParams = me0 as IBMDSwitcherTransitionMixParameters;
            me0DVETransitionParams = me0 as IBMDSwitcherTransitionDVEParameters;
            m_fairlightAudioMixer = (IBMDSwitcherFairlightAudioMixer)switcher;
        }

        //Získávání efektů
        public IEnumerable<IBMDSwitcherMixEffectBlock> MixEffectBlocks
        {
            get
            {
                // Create a mix effect block iterator
                switcher.CreateIterator(typeof(IBMDSwitcherMixEffectBlockIterator).GUID, out IntPtr meIteratorPtr);
                IBMDSwitcherMixEffectBlockIterator meIterator = Marshal.GetObjectForIUnknown(meIteratorPtr) as IBMDSwitcherMixEffectBlockIterator;
                if (meIterator == null)
                    yield break;

                // Iterate through all mix effect blocks
                while (true)
                {
                    meIterator.Next(out IBMDSwitcherMixEffectBlock me);

                    if (me != null)
                        yield return me;
                    else
                        yield break;
                }
            }
        }

        // Získávání inputů
        public IEnumerable<IBMDSwitcherInput> SwitcherInputs
        {
            get
            {
                // Create an input iterator
                switcher.CreateIterator(typeof(IBMDSwitcherInputIterator).GUID, out IntPtr inputIteratorPtr);
                IBMDSwitcherInputIterator inputIterator = Marshal.GetObjectForIUnknown(inputIteratorPtr) as IBMDSwitcherInputIterator;
                if (inputIterator == null)
                    yield break;

                // Scan through all inputs
                while (true)
                {
                    inputIterator.Next(out IBMDSwitcherInput input);

                    if (input != null)
                        yield return input;
                    else
                        yield break;
                }
            }
        }
        static long GetInputId(IBMDSwitcherInput input)
        {
            input.GetInputId(out long id);
            return id;
        }
        private static IBMDSwitcherInput getSwitcherInput(AtemSwitcher sw, int cam)
        {
            return sw.SwitcherInputs
                                        .Where((i, ret) =>
                                        {
                                            i.GetPortType(out _BMDSwitcherPortType type);
                                            return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
                                        })
                                        .ElementAt(cam);
        }

        // Změna vstupu
        public void ChangeInput(AtemSwitcherDataClass data)
        {
            me0.SetProgramInput(GetInputId(getSwitcherInput(this, data.camera)));
        }

        // Změna preview
        public void ChangePreview(AtemSwitcherDataClass data)
        {
            me0.SetPreviewInput(GetInputId(getSwitcherInput(this, data.previewCamera)));
        }

        // Nastavení přechodu
        public void ChangeTransition(AtemSwitcherDataClass data)
        {
            switch (data.transition)
            {
                case "mix":
                    me0TransitionParams.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleMix);
                    break;
                case "dve":
                    me0TransitionParams.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleDVE);
                    break;
                case "wipe":
                    me0TransitionParams.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleWipe);
                    me0WipeTransitionParams.SetPattern(_BMDSwitcherPatternStyle.bmdSwitcherPatternStyleRectangleIris);
                    break;
                case "dip":
                    me0TransitionParams.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleDip);
                    break;
            }
        }

        // Nastavení délky přechodu
        public void ChangeTransitionTime(AtemSwitcherDataClass data)
        {
            me0WipeTransitionParams.SetRate(data.time);
            me0DipTransitionParams.SetRate(data.time);
            me0MixTransitionParams.SetRate(data.time);
            me0DVETransitionParams.SetRate(data.time);
        }

        // Provedení AUTO přechodu
        public void PerformAutoTransition()
        {
            me0.PerformAutoTransition();
        }

        // Provedení CUT přechodu
        public void PerformCut()
        {
            me0.PerformCut();
        }

        // Provedení FTB přechodu
        public void PerformFTB()
        {
            me0.PerformFadeToBlack();
        }

        public void PerformMakro(AtemSwitcherDataClass data)
        {
            ChangePreview(data);
            ChangeTransition(data);
            ChangeTransitionTime(data);
            PerformAutoTransition();
        }
        public void SetAudioGain(AtemSwitcherDataClass data)
        {
            m_fairlightAudioMixer.SetMasterOutFaderGain(data.gain);
        }
    }
}
