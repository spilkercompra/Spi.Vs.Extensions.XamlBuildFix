namespace Spi.Vs.Extensions
{
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.Extensibility.Commands;
    using Spi.Vs.Extensions.XamlBuildFix;

    /// <summary>
    /// Configures menus and command groups for the extension.
    /// </summary>
    internal class MyMenuConfiguration
    {
        /// <summary>
        /// Gets the custom menu configuration.
        /// </summary>
        [VisualStudioContribution]
        public static MenuConfiguration MyMenu => new("%MyMenu.DisplayName%")
        {
            Children = new[]
            {
                MenuChild.Command<XamlBuildFixCommand>(),
            },
        };

        /// <summary>
        /// Gets the command group configuration for the Tools menu.
        /// </summary>
        [VisualStudioContribution]
        public static CommandGroupConfiguration MyGroup => new(GroupPlacement.KnownPlacements.ToolsMenu)
        {
            Children = new[]
            {
                GroupChild.Menu(MyMenu),
            }
        };
    }
}