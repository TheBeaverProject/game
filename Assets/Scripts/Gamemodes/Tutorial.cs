using System;
using Scripts.Gamemodes.Mechanics;

namespace Scripts.Gamemodes
{
    public class Tutorial : DominationGamemode
    {
        public TutorialManager _manager;

        private void FixedUpdate()
        {
            if (_manager.ReadyToSpawn)
            {
                HandleZones();
                _manager.playerManager.HUD.UpdateZones(ZoneAPoints, ZoneBPoints, PointsToCaptureZone);
            }
        }

        protected override void OnZoneCaptured(DominationPoint Zone, byte zoneCapturedBy)
        {
            _manager.playerManager.HUD.DisplayAnnouncement($"You've captured zone {Zone.gameObject.name}");
        }
    }
}