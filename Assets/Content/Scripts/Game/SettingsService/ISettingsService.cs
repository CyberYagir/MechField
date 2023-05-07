using Content.Scripts.Global;

namespace Content.Scripts.Game.SettingsService
{
    public interface ISettingsService
    {
        GameSettingsObject SettingsObject { get; }
        void LoadFile();
        void SaveFile();
    }
}