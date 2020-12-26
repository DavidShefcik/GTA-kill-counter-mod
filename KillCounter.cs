using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using GTA.Native;

namespace KillCounter
{
    public class KillCounter : Script
    {
        private int totalKilled, pedsKilled, copsKilled, militaryKilled = 0;
        private readonly TextElement _totalKilledLabel, _pedsKilledLabel, _copsKilledLabel, _militaryKilledLabel;
        private readonly GTA.UI.Font GTAFont = GTA.UI.Font.Pricedown;
        private readonly int labelHeight = 22;
        private readonly float labelScale = 0.45f;
        private readonly float labelXPos = GTA.UI.Screen.Width - 135;

        private int lastMoney = 0;

        private List<IntPtr> killedEntities = new List<IntPtr>();

        private readonly List<PedHash> copHash = new List<PedHash>() { PedHash.CopCutscene, PedHash.UndercoverCopCutscene, PedHash.Cop01SFY, PedHash.Cop01SMY, PedHash.Hwaycop01SMY, PedHash.Snowcop01SMM, PedHash.Swat01SMY, PedHash.Sheriff01SFY, PedHash.Sheriff01SMY, PedHash.Security01SMM  };
        private readonly List<PedHash> militaryHash = new List<PedHash>() { PedHash.MilitaryBum, PedHash.Marine01SMM, PedHash.Marine01SMY, PedHash.Marine02SMM, PedHash.Marine02SMY, PedHash.Marine03SMY};

        public KillCounter()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            _totalKilledLabel = new TextElement("Total Killed: " + totalKilled, new PointF(labelXPos, 10), labelScale, Color.White, GTAFont, Alignment.Left, true, true);
            _pedsKilledLabel = new TextElement("Pedestrians Killed: " + pedsKilled, new PointF(labelXPos, 10 + labelHeight), labelScale, Color.White, GTAFont, Alignment.Left, true, true);
            _copsKilledLabel = new TextElement("Cops Killed: " + copsKilled, new PointF(labelXPos, 10 + (labelHeight * 2)), labelScale, Color.White, GTAFont, Alignment.Left, true, true);
            _militaryKilledLabel = new TextElement("Military Killed: " + militaryKilled, new PointF(labelXPos, 10 + (labelHeight * 3)), labelScale, Color.White, GTAFont, Alignment.Left, true, true);

            lastMoney = Game.Player.Money;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Check if player lost 5000 and their health is full, usually means they respawned
            if(lastMoney - 5000 == Game.Player.Money)
            {
                ResetValues();
            }

            Entity[] entities = World.GetNearbyEntities(Game.Player.Character.Position, 100);

            for (int count = 0; count < entities.Length; count++)
            {
                Entity entity = entities[count];
                if(entity.IsDead && killedEntities.IndexOf(entity.MemoryAddress) == -1 && entity.Model.IsPed)
                {
                    killedEntities.Add(entity.MemoryAddress);
                    totalKilled += 1;
                    if(copHash.Contains((PedHash) entity.Model.Hash))
                    {
                        copsKilled += 1;
                    }
                    else if (militaryHash.Contains((PedHash)entity.Model.Hash))
                    {
                        militaryKilled += 1;
                    } else
                    {
                        pedsKilled += 1;
                    }
                }
            }

            _totalKilledLabel.Caption = "Total Killed: " + totalKilled;
            _pedsKilledLabel.Caption = "Pedestrians Killed: " + pedsKilled;
            _copsKilledLabel.Caption = "Cops Killed: " + copsKilled;
            _militaryKilledLabel.Caption = "Military Killed: " + militaryKilled;

            _totalKilledLabel.Draw();
            _pedsKilledLabel.Draw();
            _copsKilledLabel.Draw();
            _militaryKilledLabel.Draw();

            lastMoney = Game.Player.Money;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                Notification.Show("Kill Counter Reset");
                ResetValues();
            }
        }

        private void ResetValues()
        {
            totalKilled = 0;
            pedsKilled = 0;
            copsKilled = 0;
            militaryKilled = 0;

            _totalKilledLabel.Caption = "Total Killed: " + totalKilled;
            _pedsKilledLabel.Caption = "Pedestrians Killed: " + pedsKilled;
            _copsKilledLabel.Caption = "Cops Killed: " + copsKilled;
            _militaryKilledLabel.Caption = "Military Killed: " + militaryKilled;
        }

        public static class Logger
        {
            public static void Log(object message)
            {
                File.AppendAllText("KillCounter.log", DateTime.Now + " : " + message + Environment.NewLine);
            }
        }
    }
}
