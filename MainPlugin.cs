using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace NoWires
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NoWiresPlugin : BaseUnityPlugin

    {
        internal const string ModName = "NoWires";
        internal const string ModVersion = "1.0.1";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource NoWiresLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private void Awake()
        {
            _harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Wire), nameof(Wire.Awake))]
    static class WireAwakePatch
    {
        static void Postfix(Wire __instance)
        {
            TurnOffWire(__instance);
        }

        public static void TurnOffWire(Wire wire)
        {
            // Not really sure why I need reflection here, but it doesn't work without it despite being a public bool
            bool isFinished = (bool)AccessTools.Field(typeof(Wire), "IsFinished").GetValue(wire);
            if (isFinished)
            {
                wire.lineRenderer.forceRenderingOff = true;
                foreach (WirePart wireWirePart in wire.WireParts)
                {
                    wireWirePart.MeshRenderer.forceRenderingOff = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Wire), nameof(Wire.CreateModel))]
    static class WireCreateModelPatch
    {
        static void Postfix(Wire __instance)
        {
            WireAwakePatch.TurnOffWire(__instance);
        }
    }
}