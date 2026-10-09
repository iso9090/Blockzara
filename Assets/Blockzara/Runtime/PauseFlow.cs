namespace Blockzara.Runtime
{
    public enum PauseScreen
    {
        Hidden,
        Pause,
        ConfirmRestart,
        ConfirmLeave,
        Settings
    }

    public enum PauseBackResult
    {
        None,
        Stay,
        Resume
    }

    public sealed class PauseFlow
    {
        public PauseScreen Screen { get; private set; } = PauseScreen.Hidden;

        public void Open() => Screen = PauseScreen.Pause;

        public void Resume() => Screen = PauseScreen.Hidden;

        public void AskRestart() => Screen = PauseScreen.ConfirmRestart;

        public void AskLeave() => Screen = PauseScreen.ConfirmLeave;

        public void OpenSettings() => Screen = PauseScreen.Settings;

        public void CloseSettings() => Screen = PauseScreen.Pause;

        public void CancelDialog() => Screen = PauseScreen.Pause;

        public void ConfirmRestart() => Screen = PauseScreen.Hidden;

        public void ConfirmLeave() => Screen = PauseScreen.Hidden;

        public void Reset() => Screen = PauseScreen.Hidden;

        public PauseBackResult Back()
        {
            if (Screen == PauseScreen.ConfirmRestart || Screen == PauseScreen.ConfirmLeave || Screen == PauseScreen.Settings)
            {
                Screen = PauseScreen.Pause;
                return PauseBackResult.Stay;
            }

            if (Screen == PauseScreen.Pause)
            {
                Screen = PauseScreen.Hidden;
                return PauseBackResult.Resume;
            }

            return PauseBackResult.None;
        }
    }
}
