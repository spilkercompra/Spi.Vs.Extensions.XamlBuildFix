namespace Spi.Vs.Extensions.About
{
  using System.Diagnostics;
  using System.Threading.Tasks;
  using Microsoft;
  using Microsoft.VisualStudio.Extensibility;
  using Microsoft.VisualStudio.Extensibility.Commands;
  using Microsoft.VisualStudio.Extensibility.Shell;

  [VisualStudioContribution]
  internal class AboutCommand : Command
  {
    #region Fields

    private readonly TraceSource logger;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AboutCommand"/> class.
    /// </summary>
    /// <param name="traceSource">Trace source instance to utilize.</param>
    public AboutCommand(TraceSource traceSource)
    {
      // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
      // other services here as well.
      this.logger = Requires.NotNull(traceSource, nameof(traceSource));
      this.DisableDuringExecution = true;
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration =>
        new("%Spi.Vs.Extensions.About.AboutCommand.DisplayName%")
        {
          // Use this object initializer to set optional parameters for the command. The required parameter,
          // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
          Icon = new(ImageMoniker.KnownValues.AboutBox, IconSettings.IconAndText),
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
      var version = this.GetType().Assembly.GetName().Version;
      var fileVersionInfo = FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location);

      var usesNerdbankVersioning = Vsix.Version == "|%CurrentProject%;GetBuildVersion|";
      var vsixVersion = usesNerdbankVersioning ? fileVersionInfo.FileVersion : Vsix.Version;

      await this
          .Extensibility.Shell()
          .ShowPromptAsync(
              $"""
              {Vsix.Name} - {vsixVersion}
              {Vsix.Description}

              AssemblyVersion {version}
              FileVersion {fileVersionInfo.FileVersion}
              ProductVersion {fileVersionInfo.ProductVersion}
              """,
              PromptOptions.OK,
              cancellationToken
          );
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
      // Use InitializeAsync for any one-time setup or initialization.
      await base.InitializeAsync(cancellationToken);
    }

    #endregion Methods
  }
}
