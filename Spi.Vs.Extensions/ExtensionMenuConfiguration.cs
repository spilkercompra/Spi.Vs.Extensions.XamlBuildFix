namespace Spi.Vs.Extensions
{
  using Microsoft.VisualStudio.Extensibility;
  using Microsoft.VisualStudio.Extensibility.Commands;
  using Spi.Vs.Extensions.About;
  using Spi.Vs.Extensions.XamlBuildFix;

  /// <summary>
  /// Configures menus and command groups for the extension.
  /// </summary>
  internal class ExtensionMenuConfiguration
  {
    #region Properties

    /// <summary>
    /// Gets the command group configuration for the ExtensionsMenu menu.
    /// </summary>
    [VisualStudioContribution]
    public static CommandGroupConfiguration Group =>
        new(GroupPlacement.KnownPlacements.ExtensionsMenu)
        {
          Children = new[] { GroupChild.Menu(Menu) },
        };

    /// <summary>
    /// Gets the custom menu configuration.
    /// </summary>
    [VisualStudioContribution]
    public static MenuConfiguration Menu =>
        new("%Spi.Vs.Extensions.Menu.DisplayName%")
        {
          Children = new[]
            {
                    MenuChild.Command<XamlBuildFixCommand>(),
                    MenuChild.Separator,
                    MenuChild.Command<AboutCommand>(),
            },
        };

    #endregion Properties
  }
}
