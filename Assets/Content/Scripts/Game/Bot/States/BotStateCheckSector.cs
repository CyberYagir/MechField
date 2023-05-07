using UnityEngine;

namespace Content.Scripts.Game.Bot
{
    public class BotStateCheckSector : TypeState<BotStates>
    {
        private BotStateController botStateController;
        private MapData mapData;
        private MapData.Sectors.Sector sector;

        private bool findPoint;
        
        public BotStateCheckSector(BotStates type, BotStateController botStateController, MapData mapData) : base(type)
        {
            this.mapData = mapData;
            this.botStateController = botStateController;
        }

        public override void Start()
        {
            base.Start();
            findPoint = false;
            sector = mapData.SectorsData.GetSectorByPos(botStateController.Transform.position);
        }

        public override void Run()
        {
            base.Run();

            if (!findPoint)
            {
                GetRandomPointInBounds();
                return;
            }

            if (botStateController.IsArrive())
            {
                FinishState();
            }
        }

        public void GetRandomPointInBounds()
        {
            Vector3 point;
            point = sector.GetRandomPoint();
            if (mapData.PointIsOnNavmesh(point) && botStateController.CanBuildPath(point))
            {
                botStateController.MoveTo(mapData.HitOnNavMesh(point).position);
                findPoint = true;
            }
        }
    }
}