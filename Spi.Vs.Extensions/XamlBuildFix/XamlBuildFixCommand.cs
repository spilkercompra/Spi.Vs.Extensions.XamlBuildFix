namespace Spi.Vs.Extensions.XamlBuildFix
{
  using System.Diagnostics;
  using Microsoft;
  using Microsoft.VisualStudio.Extensibility;
  using Microsoft.VisualStudio.Extensibility.Commands;
  using Microsoft.VisualStudio.Extensibility.Shell;
  using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Threading;
  using DTE = EnvDTE.DTE;

  /// <summary>
  /// Xaml Build Fix Command.
  /// </summary>
  [VisualStudioContribution]
  internal class XamlBuildFixCommand : Command
  {
    #region Fields

    private readonly AsyncServiceProviderInjection<DTE, DTE> dteProvider;
    private readonly TraceSource logger;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XamlBuildFixCommand"/> class.
    /// </summary>
    /// <param name="traceSource">Trace source instance to utilize.</param>
    /// <param name="dteProvider">DTE service provider.</param>
    public XamlBuildFixCommand(
        TraceSource traceSource,
        AsyncServiceProviderInjection<DTE, DTE> dteProvider
    )
    {
      // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
      // other services here as well.
      this.logger = Requires.NotNull(traceSource, nameof(traceSource));
      this.DisableDuringExecution = true;
      this.dteProvider = dteProvider;
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration =>
        new("%Spi.Vs.Extensions.XamlBuildFix.XamlBuildFixCommand.DisplayName%")
        {
          // Use this object initializer to set optional parameters for the command. The required parameter,
          // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
          Icon = new(ImageMoniker.KnownValues.ActionTool, IconSettings.IconAndText),
          ////Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        };

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(
        IClientContext context,
        CancellationToken cancellationToken
    )
    {
      try
      {
        // Apply Harmony patching when the command is executed.
        Patcher.Patch();

        this.DisableDuringExecution = false;
        this.SetEnabledState(false);
        this.DisplayName = "XamlBuildFix executed.";

        await this
            .Extensibility.Shell()
            .ShowPromptAsync("XamlBuildFix executed.", PromptOptions.OK, cancellationToken);
      }
      catch (Exception exc)
      {
        await this
            .Extensibility.Shell()
            .ShowPromptAsync($"Exception {exc}", PromptOptions.OK, cancellationToken);
      }
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
      // Use InitializeAsync for any one-time setup or initialization.

      // Determine Visual Studio version.
      // (siehe https://stackoverflow.com/questions/11082436/detect-the-visual-studio-version-inside-a-vspackage
      // für genauere Versionserkennung)
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var dte = await this.dteProvider.GetServiceAsync();
      VSVersionInfo.MajorVersion = dte.Application.Version switch
      {
        string version when version.StartsWith("17") => 17,
        string version when version.StartsWith("18") => 18,
        _ => -1,
      };
      await TaskScheduler.Default;

      await base.InitializeAsync(cancellationToken);
    }

    #endregion Methods
  }
}
