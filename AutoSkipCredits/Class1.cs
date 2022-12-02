using BepInEx;
using RoR2;
using UnityEngine;
using System;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute
    {
    }
}

namespace AutoSkipCredits
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.AutoSkipCredits", "AutoSkipCredits", "1.0.0")]
    public class AutoSkipCredits : BaseUnityPlugin
    {
        public static ConfigEntry<bool> skipCredits;
        public static ConfigEntry<bool> skipOutro;

        public void Awake()
        {
            skipCredits = Config.Bind("General", "Skip Credits", true, "Skip the ending credits.");
            skipOutro = Config.Bind("General", "Skip Outro", false, "Skip the ending cutscene.");
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions")) RiskOfOptionsSupport();

            On.EntityStates.GameOver.ShowCredits.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && skipCredits.Value)
                {
                    self.outer.SetNextState(new EntityStates.GameOver.ShowReport());
                }
            };

            On.EntityStates.GameOver.RoR2MainEndingPlayCutscene.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && skipOutro.Value)
                {
                    self.outer.SetNextStateToMain();
                }
            };

            On.EntityStates.GameOver.VoidEndingPlayCutscene.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && skipOutro.Value)
                {
                    self.outer.SetNextStateToMain();
                }
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsSupport()
        {
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(skipCredits));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(skipOutro));
        }
    }
}
