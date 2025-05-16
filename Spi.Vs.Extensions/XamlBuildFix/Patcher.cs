namespace Spi.Vs.Extensions.XamlBuildFix
{
    using HarmonyLib;
    using System.Threading;

    /// <summary>
    /// Handles Harmony patching for the extension.
    /// </summary>
    internal static class Patcher
    {
        // Fields
        private static Harmony? harmony;
        private static bool initialized;
        private static object? syncLock;

        /// <summary>
        /// Gets a value indicating whether the patcher is initialized.
        /// </summary>
        public static bool Initialized => initialized;

        /// <summary>
        /// Applies all Harmony patches for this extension.
        /// </summary>
        public static void Patch()
        {
            // Ensures Harmony is initialized and patches are applied only once.
            _ = LazyInitializer.EnsureInitialized(
                ref harmony,
                ref initialized,
                ref syncLock,
                () =>
                {
                    var harmonyInstance = new Harmony("Spi.Vs.Extensions.XamlBuildFix");
                    harmonyInstance.PatchAll();
                    return harmonyInstance;
                });
        }
    }
}
