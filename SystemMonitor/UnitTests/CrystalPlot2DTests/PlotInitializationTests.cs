using Xunit;
using Crystal.Plot2D;

namespace Crystal.Plot2D.Tests;

public class PlotterInitializationTests : WPFTestBase
{
    [Fact]
    public void Constructor_CreatesNewPlotter()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter);
        });
    }

    [Fact]
    public void Plotter_HasMainHorizontalAxis()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.MainHorizontalAxis);
        });
    }

    [Fact]
    public void Plotter_HasMainVerticalAxis()
    {
        RunTest(() =>
        {
            // Arrange & Act
            var plotter = new Plotter();

            // Assert
            Assert.NotNull(plotter.MainVerticalAxis);
        });
    }

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

    [Fact]
    public void Plotter_HasAxisGrid()
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
}
