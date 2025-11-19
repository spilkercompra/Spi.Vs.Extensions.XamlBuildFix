namespace Spi.Vs.Extensions.XamlBuildFix
{
  using System.Drawing;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text.Json;
  using HarmonyLib;

  /// <summary>
  /// Harmony patch for ProcessParameterControl.EnsureGrid.
  /// </summary>
  /// <remarks>
  /// https://developercommunity.visualstudio.com/t/Azure-DevOps-Error---0x00-is-an-inval/10603645
  /// </remarks>
  [HarmonyPatch]
  internal static class ProcessParameterControl_EnsureGrid_Prefix_Patch
  {
    #region Methods

    /// <summary>
    /// Harmony prefix for ProcessParameterControl.EnsureGrid.
    /// Ensures the property grid is initialized in a new AppDomain.
    /// </summary>
    /// <param name="__instance">The instance of the target class.</param>
    /// <param name="___m_propertyGrid">Reference to the private m_propertyGrid field.</param>
    /// <param name="___m_gridDomain">Reference to the private m_gridDomain field.</param>
    /// <param name="___m_additionalServices">Reference to the private m_additionalServices field.</param>
    /// <returns>True to continue with the original method, false to skip.</returns>
    public static bool Prefix(
        object __instance,
        ref object ___m_propertyGrid,
        ref AppDomain ___m_gridDomain,
        object ___m_additionalServices
    )
    {
      // Org:
      ///////////////////////////////////////////////////
      ////private void EnsureGrid()
      ////{
      ////    if (this.m_propertyGrid != null)
      ////    {
      ////        return;
      ////    }
      ////    this.SetLoadProgressText(ResourceStrings.Get("ProcessParameterControl_StatusPreparingEnvironment"));
      ////    AppDomainSetup appDomainSetup = new AppDomainSetup();
      ////    appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
      ////    appDomainSetup.AppDomainManagerAssembly = AppDomain.CurrentDomain.SetupInformation.AppDomainManagerAssembly;
      ////    appDomainSetup.AppDomainManagerType = AppDomain.CurrentDomain.SetupInformation.AppDomainManagerType;
      ////    appDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
      ////    appDomainSetup.DisallowBindingRedirects = false;
      ////    appDomainSetup.LoaderOptimization = LoaderOptimization.SingleDomain;
      ////    string text = string.Format(CultureInfo.InvariantCulture, "Build Definition Environment - {0}", Guid.NewGuid());
      ////    this.m_gridDomain = AppDomain.CreateDomain(text, null, appDomainSetup);
      ////    byte[] array = null;
      ////    BinaryFormatter binaryFormatter = new BinaryFormatter();
      ////    using (MemoryStream memoryStream = new MemoryStream())
      ////    {
      ////        binaryFormatter.Serialize(memoryStream, this.InheritedFont);
      ////        memoryStream.Seek(0L, SeekOrigin.Begin);
      ////        array = new byte[memoryStream.Length];
      ////        Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Length);
      ////    }
      ////    this.m_propertyGrid = (ProcessParameterControl.ProcessParameterControlProxy)this.m_gridDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProcessParameterControl.ProcessParameterControlProxy).FullName, false, BindingFlags.Default, null, new object[] { this, array, this.m_additionalServices }, CultureInfo.CurrentUICulture, new object[0]);
      ////}

      // Patch:
      ///////////////////////////////////////////////////
      ////    if (this.m_propertyGrid != null)
      ////    {
      ////        return;
      ////    }
      if (___m_propertyGrid != null)
      {
        return true;
      }
      ////this.SetLoadProgressText(ResourceStrings.Get("ProcessParameterControl_StatusPreparingEnvironment"));
        ;

      AppDomainSetup appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationBase = AppDomain
          .CurrentDomain
          .SetupInformation
          .ApplicationBase;
      appDomainSetup.AppDomainManagerAssembly = AppDomain
          .CurrentDomain
          .SetupInformation
          .AppDomainManagerAssembly;
      appDomainSetup.AppDomainManagerType = AppDomain
          .CurrentDomain
          .SetupInformation
          .AppDomainManagerType;
      appDomainSetup.ConfigurationFile = AppDomain
          .CurrentDomain
          .SetupInformation
          .ConfigurationFile;
      appDomainSetup.DisallowBindingRedirects = false;
      appDomainSetup.LoaderOptimization = LoaderOptimization.SingleDomain;
      string text = string.Format(
          CultureInfo.InvariantCulture,
          "Build Definition Environment - {0}",
          Guid.NewGuid()
      );

      ////this.m_gridDomain = AppDomain.CreateDomain(text, null, appDomainSetup);
      ___m_gridDomain = AppDomain.CreateDomain(text, null, appDomainSetup);

      ////byte[] array = null;
      ////BinaryFormatter binaryFormatter = new BinaryFormatter();
      ////using (MemoryStream memoryStream = new MemoryStream())
      ////{
      ////    binaryFormatter.Serialize(memoryStream, this.InheritedFont);
      ////    memoryStream.Seek(0L, SeekOrigin.Begin);
      ////    array = new byte[memoryStream.Length];
      ////    Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Length);
      ////}

      // Font auf null setzen wegen:
      // Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported. Type 'System.Drawing.Font'.
      // var inheritedFont = AccessTools
      //                    .DeclaredProperty(__instance.GetType(), "InheritedFont")
      //                    .GetValue(__instance)
      //                    as Font;
      var inheritedFont = (Font?)null;
      byte[]? array = null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        JsonSerializer.Serialize<Font?>(memoryStream, inheritedFont);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        array = new byte[memoryStream.Length];
        Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Length);
      }

      ////this.m_propertyGrid = (ProcessParameterControl.ProcessParameterControlProxy)this.m_gridDomain.CreateInstanceAndUnwrap(
      ///                         Assembly.GetExecutingAssembly().FullName,
      ///                         typeof(ProcessParameterControl.ProcessParameterControlProxy).FullName,
      ///                         false,
      ///                         BindingFlags.Default,
      ///                         null,
      ///                         new object[] { this, array, this.m_additionalServices },
      ///                         CultureInfo.CurrentUICulture,
      ///                         new object[0]);
      var processParameterControlType = __instance.GetType();
      var assemblyName = processParameterControlType.Assembly.FullName;
      //  "Microsoft.TeamFoundation.Build.Controls.ProcessParameterControl+ProcessParameterControlProxy"
      var typeName = processParameterControlType
          .GetNestedType(
              "ProcessParameterControlProxy",
              BindingFlags.Public | BindingFlags.NonPublic
          )
          .FullName;
      ___m_propertyGrid = ___m_gridDomain.CreateInstanceAndUnwrap(
          assemblyName,
          typeName,
          false,
          BindingFlags.Default,
          null,
          new object[] { __instance, array, ___m_additionalServices },
          CultureInfo.CurrentUICulture,
          new object[0]
      );

      return true;
    }

    /// <summary>
    /// Returns the target method to patch: ProcessParameterControl.EnsureGrid.
    /// </summary>
    /// <returns>The MethodBase for EnsureGrid.</returns>
    public static MethodBase TargetMethod()
    {
      // Microsoft.TeamFoundation.Build.Controls.dll
      // c:\program files\microsoft visual studio\18\professional\common7\ide\commonextensions\microsoft\teamfoundation\team explorer\Microsoft.TeamFoundation.Build.Controls.dll
      // Microsoft.TeamFoundation.Build.Controls, Version=18.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
      var majorVersion = VSVersionInfo.MajorVersion > 0 ? VSVersionInfo.MajorVersion : 18;

      var assemblyName =
          $"Microsoft.TeamFoundation.Build.Controls, Version={majorVersion}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
      var typeName = "Microsoft.TeamFoundation.Build.Controls.ProcessParameterControl";
      var methodName = "EnsureGrid";

      Assembly assembly = Assembly.Load(assemblyName);
      Type type = assembly.GetType(typeName);
      MethodInfo method = type.GetMethod(
          methodName,
          BindingFlags.NonPublic | BindingFlags.Instance
      );

      return method;
    }

    #endregion Methods
  }
}
