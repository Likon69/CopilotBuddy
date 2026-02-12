namespace Styx
{
    /// <summary>
    /// FEAT-16: Login/character selection screen states.
    /// Ported from HB 4.3.4 — values match WotLK 3.3.5a client.
    /// </summary>
    public enum GlueScreen
    {
        Unknown = -1,
        Login,
        PatchDownload,
        CharSelect,
        CharCreate,
        TrialConvert,
        Movie,
        RealmList,
        Credits,
        RealmWizard,
        End
    }
}
