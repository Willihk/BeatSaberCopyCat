using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using HarmonyLib;
using UnityEngine.Rendering;


public class BetterBloomController : MonoBehaviour
{
    private const string betterBloomID = "com.caeden117.chromapper.betterbloom";

    private Harmony betterBloomHarmony;

    // Start is called before the first frame update
    void Start()
    {
        betterBloomHarmony = new Harmony(betterBloomID);

            Type ppPass = typeof(PostProcessPass);
            MethodBase setupBloom = ppPass.GetMethod("SetupBloom",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
                Type.DefaultBinder,
                new Type[] { typeof(CommandBuffer), typeof(int), typeof(Material) }, new ParameterModifier[] { });
            HarmonyMethod transpiler = new HarmonyMethod(typeof(BetterBloomController), nameof(PatchSetupBloom));
            betterBloomHarmony.Patch(setupBloom, transpiler: transpiler);
    }

    private void OnDestroy()
    {
        betterBloomHarmony.UnpatchAll(betterBloomID);
    }

    /*
     * Replace the native IL for the "SetupBloom" function to remove bit shifting to the right.
     * This gives us a full resolution Bloom effect, much more realistic to Beat Saber's.
     * 
     * This is called once, not every time the method is called, so no big performance drops will happen.
     * 
     * Thanks to DaNike from the Beat Saber Modding Group for helping me with this transpiler patch.
     */
    private static IEnumerable<CodeInstruction> PatchSetupBloom(IEnumerable<CodeInstruction> insns)
    {
        var resList = new List<CodeInstruction>();
        int seqCount = 0;
        bool foundLdc1 = false;
        foreach (var ci in insns)
        {
            if (seqCount < 2)
            {
                if (!foundLdc1 && ci.opcode == OpCodes.Ldc_I4_1)
                    foundLdc1 = true;
                else if (foundLdc1 && ci.opcode == OpCodes.Shr)
                {
                    foundLdc1 = false; seqCount++;
                    continue;
                }
            }
            if (!foundLdc1)
                resList.Add(ci);
        }
        return resList;
    }
}