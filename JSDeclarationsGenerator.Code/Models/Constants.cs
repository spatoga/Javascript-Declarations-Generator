using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeclarationsGenerator.Code.Models
{
    internal static class Constants
    {
        public const string ASPX_Code_Block = @"<%(.|\.)*%>";
        public const string ASPX_Tag = @"</?([a-zA-Z0-9_])*:([a-zA-Z0-9_])*";
        public const string Valid_Address = @"^([\$a-zA-Z_]+[\$a-zA-Z0-9_]*)$";
        public const string ID_Selector = "//*[@id]";

    }
}
