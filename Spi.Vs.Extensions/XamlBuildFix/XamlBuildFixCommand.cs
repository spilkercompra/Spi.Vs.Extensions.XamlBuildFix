namespace Spi.Vs.Extensions.XamlBuildFix
{
    using System.Diagnostics;
    using Microsoft;
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.Extensibility.Commands;
    using Microsoft.VisualStudio.Extensibility.Shell;

    /// <summary>
    /// Xaml Build Fix Command.
    /// </summary>
    [VisualStudioContribution]
    internal class XamlBuildFixCommand : Command
    {
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlBuildFixCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public XamlBuildFixCommand(TraceSource traceSource)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            this.logger = Requires.NotNull(traceSource, nameof(traceSource));
            this.DisableDuringExecution = true; 
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%Spi.Vs.Extensions.XamlBuildFix.XamlBuildFixCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
            ////Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        };

        /// <inheritdoc />
        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.
            return base.InitializeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            try
            {
                // Apply Harmony patching when the command is executed.
                Patcher.Patch();

                this.DisableDuringExecution = false;
                this.SetEnabledState(false);
                this.DisplayName = "XamlBuildFix executed.";

                await this.Extensibility.Shell().ShowPromptAsync("XamlBuildFix executed.", PromptOptions.OK, cancellationToken);
            }
            catch (Exception exc)
            {
                await this.Extensibility.Shell().ShowPromptAsync($"Exception {exc}", PromptOptions.OK, cancellationToken);
            }
        }
    }
}