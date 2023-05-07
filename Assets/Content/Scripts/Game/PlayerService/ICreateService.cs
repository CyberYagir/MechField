using Content.Scripts.Game.Player;
using Content.Scripts.Global;

namespace Content.Scripts.Game.PlayerService
{
    public interface ICreateService
    {
        PlayerBuilder PlayerBuilder { get; }
        MapObject CurrentMap { get; }
        MapData CurrentMapInstance { get; }
    }
}