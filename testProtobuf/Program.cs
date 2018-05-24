using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.IO;

namespace testProtobuf
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public class Tutorial
    {
        [ProtoMember(1)]
        public string Author { get; set; }
        //[ProtoMember(2)]
        public string Name { get; set; }
        //[ProtoMember(3)]
        public int PageCount { get; set; }

    }

}
