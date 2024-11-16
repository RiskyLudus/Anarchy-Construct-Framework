using System;
using Anarchy.Enums;

namespace Anarchy
{
    [Serializable]
    public class AnarchyEventData
    {
        public string eventName;
        public AnarchyEventDataTypes type1;
        public AnarchyEventDataTypes type2;
        public AnarchyEventDataTypes type3;
        public AnarchyEventDataTypes type4;
    }
}
