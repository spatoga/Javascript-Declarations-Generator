using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeclarationsGenerator.Code.Models
{
    internal class Element
    {
        public string Tag { get; set; }
        public string ID { get; set; }
        public string Address { get; set; }

        public Element()
        {

        }

        public Element(string tag, string id)
        {
            Tag = tag;
            ID = id;
        }

        public Element(string tag, string address, string id)
        {
            Tag = tag;
            Address = address;
            ID = id;
        }
    }
}
