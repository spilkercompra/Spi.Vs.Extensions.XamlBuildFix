namespace Spi.Vs.Extensions
{
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.VisualStudio.Extensibility;

  /// <summary>
  /// Extension entrypoint for the VisualStudio.Extensibility extension.
  /// </summary>
  [VisualStudioContribution]
  internal class ExtensionEntrypoint : Extension
  {
    #region Properties

    /// <inheritdoc />
    public override ExtensionConfiguration ExtensionConfiguration =>
        new() { RequiresInProcessHosting = true };

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
      base.InitializeServices(serviceCollection);

      // You can configure dependency injection here by adding services to the serviceCollection.
    }

    #endregion Methods
  }
}
