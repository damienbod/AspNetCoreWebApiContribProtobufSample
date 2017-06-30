using ProtoBuf;

namespace Model
{
    [ProtoContract]
    public class Table
    {
        [ProtoMember(1)]
        public string Name {get;set;}

        [ProtoMember(2)]
        public string Description { get; set; }


        [ProtoMember(3)]
        public string Dimensions { get; set; }
    }
}

