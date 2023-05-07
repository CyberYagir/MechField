namespace Content.Scripts.Game.Bot
{
    public class BotStateMoveToSector : TypeState<BotStates>
    {
        private BotStateController botStateController;
        private MapData mapData;
        private MapData.Sectors.Sector sector;

        public BotStateMoveToSector(BotStates type, BotStateController botStateController, MapData mapData) : base(type)
        {

            this.mapData = mapData;
            this.botStateController = botStateController;
        }

        public override void Start()
        {
            base.Start();
            sector = null;
        }

        public override void Run()
        {
            base.Run();

            if (sector == null)
            {
                var sr = mapData.SectorsData.GetRandomSector();
                var point = sr.GetRandomPoint();
                if (mapData.PointIsOnNavmesh(point) && botStateController.CanBuildPath(point))
                {
                    sector = sr;
                    botStateController.MoveTo(mapData.HitOnNavMesh(point).position);
                }
            }
            else
            {
                if (botStateController.IsArrive())
                {
                    FinishState();
                }
            }
        }
    }
}