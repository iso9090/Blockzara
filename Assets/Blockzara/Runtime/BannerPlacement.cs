namespace Blockzara.Runtime
{
    public enum BannerSurface
    {
        None,
        Home,
        Gameplay
    }

    public sealed class BannerPlacement
    {
        public BannerSurface Surface { get; private set; } = BannerSurface.None;
        public int Loads { get; private set; }
        public bool Failed { get; private set; }
        public string LastError { get; private set; } = "";

        public void Show(BannerSurface surface)
        {
            if (surface == BannerSurface.None)
            {
                Surface = BannerSurface.None;
                Failed = false;
                LastError = "";
                return;
            }

            if (surface == Surface && !Failed) return;
            Surface = surface;
            Failed = false;
            LastError = "";
            Loads++;
        }

        public void Fail(string message)
        {
            Failed = true;
            LastError = message ?? "";
        }

        public void Loaded()
        {
            Failed = false;
            LastError = "";
        }
    }
}
