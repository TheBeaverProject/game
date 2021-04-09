using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Multiplayer
{
    public class EventCodes
    {
        public static byte Kill = 1;
    }

    public class EventCustomData
    {
        public struct Kill
        {
            public int killerActorNum;
            public int assistActorNum;
            public int deadActorNum;
        }
    }

    public class Events : MonoBehaviourPunCallbacks
    {
        public static void SendKillEvent(int killerActorNum, int assistActorNum, int deadActorNum)
        {
            RaiseEventOptions options = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };
            
            EventCustomData.Kill eventData = new EventCustomData.Kill
            {
                killerActorNum = killerActorNum,
                assistActorNum = assistActorNum,
                deadActorNum = deadActorNum
            };

            PhotonNetwork.RaiseEvent(EventCodes.Kill, eventData, options, SendOptions.SendReliable);
        }
    }
}