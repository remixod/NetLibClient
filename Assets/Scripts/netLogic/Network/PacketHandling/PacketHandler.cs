﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Net.Sockets;
using System.Threading;

using netLogic.Shared;
using netLogic.Network;
using netLogic.Constants;


namespace netLogic
{
    public class PacketHandler
    {
        List<PacketHandle> Handles;
        readonly LogonSession tClient;
        readonly WorldSession wClient;


        public PacketHandler(WorldSession client)
        {
            Handles = new List<PacketHandle>();
            wClient = client;
        }

        public PacketHandler(LogonSession client)
        {
            Handles = new List<PacketHandle>();
            tClient = client;
        }

        public void Initialize()
        {
            PacketHandlerAtribute atribute;
            Assembly asm = Assembly.GetCallingAssembly();
            int x = 0;
            foreach (Type asmType in asm.GetTypes())
            {

                foreach (MethodInfo method in asmType.GetMethods())
                {
                    foreach (Attribute attr in method.GetCustomAttributes(true))
                    {
                        atribute = attr as PacketHandlerAtribute;
                        if (null != atribute)
                        {
                            PacketHandle handle = new PacketHandle(method, atribute.PacketID);
                            Handles.Add(handle);
                            x++;
                        }
                    }
                }
            }
            Log.WriteLine(LogType.Success, "Loaded {0} Packet handlers.", x);
        }

        public void HandlePacket(PacketIn packet)
        {

            PacketHandle handle = Handles.Find(s => PacketId.isEqual(s.packetId,packet.PacketId));
            if (handle != null)
            {

                System.Object[] obj = new System.Object[1];
                obj[0] = packet;
                
                try
                {
                    if (packet.PacketId.Service == ServiceType.Logon)
                    {
                        Log.WriteLine(LogType.Network, "Handling packet: {0}", handle.packetId);
                        handle.MethodInfo.Invoke(tClient, obj);                      
                    }
                    else if (packet.PacketId.Service == ServiceType.World)
                    {
                        Log.WriteLine(LogType.Network, "Handling packet: {0}", handle.packetId);
                        if (handle.packetId.RawId == 170)
                        {
                            Log.WriteLine(LogType.Network, "Handling packet: {0}", handle.packetId);
                        }
                        handle.MethodInfo.Invoke(wClient, obj);
                        
                        
                    }
                    //Log.WriteLine(LogType.Packet, packet.ToHex());
                }
                catch
                {
                }

            }
            else
            {
                 //Log.WriteLine(LogType.Normal, "Unhandled packet: {0}", packet.PacketId.ToString());
            }
        }

        enum IgnoredHandles : int
        {
            TEST = 0,
        }

    }

    public class PacketHandle
    {
        public MethodInfo MethodInfo;
        public PacketId packetId;

        public PacketHandle(MethodInfo info, PacketId packetid)
        {
            this.MethodInfo = info;
            this.packetId = packetid;
        }
    }
}
