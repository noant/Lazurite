using OpenZWaveDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class Node
    {
        public Node(byte id, uint homeId, ZWManager manager)
        {
            this.Id = id;
            this.HomeId = homeId;
            Manager = manager;
            Values = new NodeValues();
        }

        public void Refresh()
        {
            Manufacturer = Manager.GetNodeManufacturerName(HomeId, Id);
            Name = Manager.GetNodeName(HomeId, Id);
            Type = Manager.GetNodeType(HomeId, Id);
            ProductName = Manager.GetNodeProductName(HomeId, Id);
            ProductType = Manager.GetNodeType(HomeId, Id);
            Location = Manager.GetNodeLocation(HomeId, Id);
        }

        public ZWManager Manager { get; private set; }
        public NodeValues Values { get; private set; }
        public string ProductType { get; private set; }
        public string Type { get; private set; }
        public string Manufacturer { get; private set; }
        public string Name { get; private set; }
        public byte Id { get; private set; }
        public uint HomeId { get; private set; }
        public string ProductName { get; private set; }
        public string Location { get; private set; }
        public Controller Controller { get; internal set; }

        public bool Failed { get; internal set; }

        internal bool Initialized { get; set; }

        public override int GetHashCode()
        {
            return HomeId.GetHashCode() ^ Name.GetHashCode() ^ ProductName.GetHashCode() ^ Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode()==obj.GetHashCode();
        }
    }
}
