using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KG_SCP_SL
{
    class SystemCommandSender : CommandSender
    {
        public override string Nickname => "System";

        public override string SenderId => "System";

        public override ulong Permissions => 0;

        public override byte KickPower => 0;

        public override bool FullPermissions => true;

        public override void Print(string text)
        {
            return;
        }

        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
        {
            return;
        }
    }
}
