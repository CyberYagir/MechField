using UnityEngine;

namespace Content.Scripts.Game.Bot
{
    public class BotStateRunAway: BotStateMoveToSector
    {
        private float runTime;
        public BotStateRunAway(BotStates type, BotStateController botStateController, MapData mapData) : base(type, botStateController, mapData)
        {
            
        }


        public override void Run()
        {
            base.Run();
            runTime += Time.deltaTime;


            if (runTime >= 5)
            {
                FinishState();
            }
        }
    }
}