using System;
using System.Collections.Generic;
using System.Text;

namespace ClangReader.StructTranslation
{
    public class PacketWriteEntry
    {

    }

    public class SecondaryObjectPacketWriteEntry : PacketWriteEntry
    {
        public bool IsArray { get; set; }

        //Struct, Vector or smth
        public string ObjectType { get; set; }

        // pInfo
        public string ObjectName { get; set; }

        // Length
        public string FieldName { get; set; }
    }
}
