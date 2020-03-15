using System;

namespace PUBG_Lite_test
{
    class Game
    {
        private readonly IntPtr moduleBase = IntPtr.Zero;

        public Game(string gameName, string moduleName)
        {
            if (!Driver.Init(gameName))
            {
                Driver.CloseHandle();
                throw new Exception("Invalid handle to driver or could not find " + gameName);
            }

            moduleBase = Driver.GetModuleBase(moduleName);
            if (moduleBase == IntPtr.Zero)
            {
                Driver.CloseHandle();
                throw new Exception(moduleName + " not found");
            }
        }

        public UWorld UWorld()
        {
            IntPtr GWorld = Driver.Read<IntPtr>(moduleBase + Offset.oUWorld);
            return Driver.Read<UWorld>(GWorld);
        }
    }
}
