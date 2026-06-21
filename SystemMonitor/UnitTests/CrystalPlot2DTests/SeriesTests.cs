using Xunit;
using Crystal.Plot2D;

namespace Crystal.Plot2D.Tests;

public class PlotterLegendTests : WPFTestBase
{
    [Fact]
    public void Plotter_HasLegend()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.Legend);
        });
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Legend_VisibilityCanBeSet(bool isVisible)
    {
        RunTest(() =>
        {
            // Arrange
            var plotter = new Plotter();

            // Act
            plotter.NewLegendVisible = isVisible;

            // Assert
            Assert.Equal(isVisible, plotter.NewLegendVisible);
        });
    }

    [Fact]
    public void Plotter_AxisGridIsNotNull()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.AxisGrid);
        });
    }

    [Fact]
    public void Plotter_AxisGridPathIsNotNull()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.AxisGrid.GridPath);
        });
    }

    [Fact]
    public void Plotter_DefaultContextMenuIsNotNull()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.DefaultContextMenu);
        });
    }
}

public class PlotterNavigationTests : WPFTestBase
{
    [Fact]
    public void Plotter_HasMouseNavigation()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.MouseNavigation);
        });
    }

    [Fact]
    public void Plotter_HasKeyboardNavigation()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.KeyboardNavigation);
        });
    }

    [Fact]
    public void Plotter_HasHorizontalAxisNavigation()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.HorizontalAxisNavigation);
        });
    }

    [Fact]
    public void Plotter_HasVerticalAxisNavigation()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.VerticalAxisNavigation);
        });
    }
}
