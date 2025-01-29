using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HarmonyLib;
using Vintagestory.GameContent;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.Server;
using ProperVersion;

namespace Project1
{


    public class Project1ModSystem : ModSystem
    {

        //public static TermporalogConfig Config = null!;
        //private readonly string _configFile = "VSMetrics.json";
        private int CollectionInterval = 10000;

        private Process? _vsClientProcess;

        private Process? _vsServerProcess;
        private ServerMain _server = null!;

        private long _writeDataServerListenerId;
        private long _writeDataClientListenerId;
        ICoreServerAPI _sapi;
        ICoreClientAPI _capi;



        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            _sapi = sapi;
            sapi.Logger.Notification("Hello from template mod server side: " + Lang.Get("project1:hello"));
            _server = (ServerMain)sapi.World;
            _writeDataServerListenerId = sapi.Event.RegisterGameTickListener(ServerUseageCheck, CollectionInterval);
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = capi;
            _vsClientProcess = Process.GetCurrentProcess();
            capi.Logger.Notification("Hello from template mod client side: " + Lang.Get("project1:hello"));
            _writeDataServerListenerId = capi.Event.RegisterGameTickListener(ClientUseageCheck, CollectionInterval);

        }

        private void ClientUseageCheck(float t1)
        {
            _vsClientProcess?.Refresh();
            var totalMemory = _vsClientProcess?.PrivateMemorySize64 / 1048576;
            var managedMemory = GC.GetTotalMemory(false) / 1048576;
            _capi.Logger.Debug("CLIENT: TOTAL MEMORY: {0}, MANAGED MEMORY: {1}", totalMemory, managedMemory);

            
        }
        private void ServerUseageCheck(float t1)
        {
            _vsServerProcess?.Refresh();
            var totalMemory = _vsServerProcess?.PrivateMemorySize64 / 1048576;
            var managedMemory = GC.GetTotalMemory(false) / 1048576;
            _sapi.Logger.Debug("SERVER: TOTAL MEMORY: {0}, MANAGED MEMORY: {1}", totalMemory, managedMemory);
        }

        public override void Dispose()
        {
           
            if (_sapi != null)
            _sapi.Event.UnregisterGameTickListener(_writeDataServerListenerId);
           
            if (_capi != null)
            _capi.Event.UnregisterGameTickListener(_writeDataClientListenerId);

        }
    }
}
