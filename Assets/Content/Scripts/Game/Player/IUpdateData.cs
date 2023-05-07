namespace Content.Scripts.Game.Player
{
    public interface IUpdateData
    {
        public bool Enabled { get; }

        public void SetEnabled(bool state);

        public void Update();
    }
}