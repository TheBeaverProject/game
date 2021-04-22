using System.Collections.Generic;
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
        public static Dictionary<string, int> Kill(int killerActorNum, int assistActorNum, int deadActorNum)
        {
            return new Dictionary<string, int>
            {
                {"killerActorNum", killerActorNum},
                {"assistActorNum", assistActorNum},
                {"deadActorNum", deadActorNum}
            };
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

            var content = EventCustomData.Kill(killerActorNum, assistActorNum, deadActorNum);

            PhotonNetwork.RaiseEvent(EventCodes.Kill, content, options, SendOptions.SendReliable);
        }
    }
}