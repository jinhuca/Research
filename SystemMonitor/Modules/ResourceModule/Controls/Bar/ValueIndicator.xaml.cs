using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResourceModule.Controls.Bar;

public partial class ValueIndicator : UserControl {
  public ValueIndicator() {
    InitializeComponent();
  }

  #region Control Font Properties

  #region FontFamily

  [Bindable(true)]
  [Category("Appearance")]
  [Localizability(LocalizationCategory.Font)]
  public FontFamily ControlFontFamily {
    get => (FontFamily)GetValue(ControlFontFamilyProperty);
    set => SetValue(ControlFontFamilyProperty, value);
  }

  public static readonly DependencyProperty ControlFontFamilyProperty = DependencyProperty.Register(
    nameof(ControlFontFamily),
    typeof(FontFamily),
    typeof(ValueIndicator),
    new PropertyMetadata(new FontFamily("Segoe UI")));

  #endregion FontFamily

  #region FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double ControlFontSize {
    get => (double)GetValue(ControlFontSizeProperty);
    set => SetValue(ControlFontSizeProperty, value);
  }

  public static readonly DependencyProperty ControlFontSizeProperty = DependencyProperty.Register(
    nameof(ControlFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(14.0));

  #endregion FontSize

  #region FontWeight

  public FontWeight ControlFontWeight {
    get => (FontWeight)GetValue(ControlFontWeightProperty);
    set => SetValue(ControlFontWeightProperty, value);
  }

  public static readonly DependencyProperty ControlFontWeightProperty = DependencyProperty.Register(
    nameof(ControlFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion FontWeight

  #region Foreground

  public Brush ControlForeground {
    get => (Brush)GetValue(ControlForegroundProperty);
    set => SetValue(ControlForegroundProperty, value);
  }

  public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register(
    nameof(ControlForeground),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Foreground

  #endregion Control Font Properties

  #region Title Dependency Properties

  #region Text

  public string Title {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
    nameof(Title),
    typeof(string),
    typeof(ValueIndicator),
    new PropertyMetadata(string.Empty));

  #endregion Text

  #region FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double TitleFontSize {
    get => (double)GetValue(TitleFontSizeProperty);
    set => SetValue(TitleFontSizeProperty, value);
  }

  public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(
    nameof(TitleFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion FontSize

  #region FontWeight

  public FontWeight TitleFontWeight {
    get => (FontWeight)GetValue(TitleFontWeightProperty);
    set => SetValue(TitleFontWeightProperty, value);
  }

  public static readonly DependencyProperty TitleFontWeightProperty = DependencyProperty.Register(
    nameof(TitleFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion FontWeight

  #region Foreground

  public Brush TitleForeground {
    get => (Brush)GetValue(TitleForegroundProperty);
    set => SetValue(TitleForegroundProperty, value);
  }

  public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(
    nameof(TitleForeground),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Foreground

  #endregion Title Dependency Properties

  #region ValueId Dependency Properties

  #region Text

  public string ValueId {
    get => (string)GetValue(ValueIdProperty);
    set => SetValue(ValueIdProperty, value);
  }

  public static readonly DependencyProperty ValueIdProperty = DependencyProperty.Register(
    nameof(ValueId),
    typeof(string),
    typeof(ValueIndicator),
    new PropertyMetadata(string.Empty));

  #endregion Text

  #region FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double ValueIdFontSize {
    get => (double)GetValue(ValueIdFontSizeProperty);
    set => SetValue(ValueIdFontSizeProperty, value);
  }

  public static readonly DependencyProperty ValueIdFontSizeProperty = DependencyProperty.Register(
    nameof(ValueIdFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion FontSize

  #region FontWeight

  public FontWeight ValueIdFontWeight {
    get => (FontWeight)GetValue(ValueIdFontWeightProperty);
    set => SetValue(ValueIdFontWeightProperty, value);
  }

  public static readonly DependencyProperty ValueIdFontWeightProperty = DependencyProperty.Register(
    nameof(ValueIdFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion FontWeight

  #region Foreground

  public Brush ValueIdForeground {
    get => (Brush)GetValue(ValueIdForegroundProperty);
    set => SetValue(ValueIdForegroundProperty, value);
  }

  public static readonly DependencyProperty ValueIdForegroundProperty = DependencyProperty.Register(
    nameof(ValueIdForeground),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Foreground

  #endregion ValueId Dependency Properties

  #region Indicator Bar Properties

  #region Height

  [TypeConverter(typeof(LengthConverter))]
  [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
  public double IndicatorHeight {
    get => (double)GetValue(IndicatorHeightProperty);
    set => SetValue(IndicatorHeightProperty, value);
  }

  public static readonly DependencyProperty IndicatorHeightProperty = DependencyProperty.Register(
    nameof(IndicatorHeight),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion Height

  #region Weight

  [TypeConverter(typeof(LengthConverter))]
  [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
  public double IndicatorWidth {
    get => (double)GetValue(IndicatorWidthProperty);
    set => SetValue(IndicatorWidthProperty, value);
  }

  public static readonly DependencyProperty IndicatorWidthProperty = DependencyProperty.Register(
    nameof(IndicatorWidth),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(120.0));

  #endregion Weight

  #region Brush

  public Brush IndicatorBrush {
    get => (Brush)GetValue(IndicatorBrushProperty);
    set => SetValue(IndicatorBrushProperty, value);
  }

  public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.Register(
    nameof(IndicatorBrush),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Green));

  #endregion Brush

  #endregion Indicator Bar Properties

  #region Value DependencyProperties

  #region Value

  private const double DefaultMinValue = 0, DefaultMaxValue = 100;

  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(double),
    typeof(ValueIndicator),
    new FrameworkPropertyMetadata(
      DefaultMinValue,
      new PropertyChangedCallback(OnValueChanged)));

  public double Value {
    get => (double)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }

  private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
    var control = (ValueIndicator)obj;
    RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
        (double)args.OldValue, (double)args.NewValue, ValueChangedEvent);
    control.OnValueChanged(e);
  }

  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
    nameof(ValueChanged),
    RoutingStrategy.Bubble,
    typeof(RoutedPropertyChangedEventHandler<double>),
    typeof(ValueIndicator));

  public event RoutedPropertyChangedEventHandler<double> ValueChanged {
    add { AddHandler(ValueChangedEvent, value); }
    remove { RemoveHandler(ValueChangedEvent, value); }
  }

  protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args) {
    RaiseEvent(args);
  }

  #endregion Value Text

  #region FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double ValueFontSize {
    get => (double)GetValue(ValueFontSizeProperty);
    set => SetValue(ValueFontSizeProperty, value);
  }

  public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register(
    nameof(ValueFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion FontSize

  #region FontFamily

  [Bindable(true)]
  [Category("Appearance")]
  [Localizability(LocalizationCategory.Font)]
  public FontFamily ValueFontFamily {
    get => (FontFamily)GetValue(ValueFontFamilyProperty);
    set => SetValue(ValueFontFamilyProperty, value);
  }

  public static readonly DependencyProperty ValueFontFamilyProperty = DependencyProperty.Register(
    nameof(ValueFontFamily),
    typeof(FontFamily),
    typeof(ValueIndicator),
    new PropertyMetadata(new FontFamily("Segoe UI")));

  #endregion FontFamily

  #region FontWeight

  public FontWeight ValueFontWeight {
    get => (FontWeight)GetValue(ValueFontWeightProperty);
    set => SetValue(ValueFontWeightProperty, value);
  }

  public static readonly DependencyProperty ValueFontWeightProperty = DependencyProperty.Register(
    nameof(ValueFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion FontWeight

  #region Foreground

  public Brush ValueForeground {
    get => (Brush)GetValue(ValueForegroundProperty);
    set => SetValue(ValueForegroundProperty, value);
  }

  public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register(
    nameof(ValueForeground),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Foreground

  #endregion Value Text

  #region Unit Dependency Properties

  #region Unit

  public string Unit {
    get => (string)GetValue(UnitProperty);
    set => SetValue(UnitProperty, value);
  }

  public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
    nameof(Unit),
    typeof(string),
    typeof(ValueIndicator),
    new PropertyMetadata(string.Empty));

  #endregion Unit

  #region FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double UnitFontSize {
    get => (double)GetValue(UnitFontSizeProperty);
    set => SetValue(UnitFontSizeProperty, value);
  }

  public static readonly DependencyProperty UnitFontSizeProperty = DependencyProperty.Register(
    nameof(UnitFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion FontSize

  #region FontWeight

  public FontWeight UnitFontWeight {
    get => (FontWeight)GetValue(UnitFontWeightProperty);
    set => SetValue(UnitFontWeightProperty, value);
  }

  public static readonly DependencyProperty UnitFontWeightProperty = DependencyProperty.Register(
    nameof(UnitFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion FontWeight

  #region Foreground

  public Brush UnitForeground {
    get => (Brush)GetValue(UnitForegroundProperty);
    set => SetValue(UnitForegroundProperty, value);
  }
  public static readonly DependencyProperty UnitForegroundProperty = DependencyProperty.Register(
    nameof(UnitForeground),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Foreground

  #endregion Unit Dependency Properties

  #region Minimum DependencyProperty

  [Bindable(true)]
  [Category("Behavior")]
  public double Minimum {
    get => (double)GetValue(MinimumProperty);
    set => SetValue(MinimumProperty, value);
  }

  public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
    nameof(Minimum),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(0.0));

  #endregion Minimum DependencyProperty

  #region Maximum DependencyProperty

  [Bindable(true)]
  [Category("Behavior")]
  public double Maximum {
    get => (double)GetValue(MaximumProperty);
    set => SetValue(MaximumProperty, value);
  }

  public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
    nameof(Maximum),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(0.0));

  #endregion Maximum DependencyProperty

  #region Separator

  #region Width

  [TypeConverter(typeof(LengthConverter))]
  [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
  public double SeparatorWidth {
    get => (double)GetValue(SeparatorWidthProperty);
    set => SetValue(SeparatorWidthProperty, value);
  }

  public static readonly DependencyProperty SeparatorWidthProperty = DependencyProperty.Register(
    nameof(SeparatorWidth),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(1.0));

  #endregion Width

  #region Margin

  public Thickness SeparatorMargin {
    get => (Thickness)GetValue(SeparatorMarginProperty);
    set => SetValue(SeparatorMarginProperty, value);
  }

  public static readonly DependencyProperty SeparatorMarginProperty = DependencyProperty.Register(
    nameof(SeparatorMargin),
    typeof(Thickness),
    typeof(ValueIndicator),
    new PropertyMetadata(new Thickness(0)));

  #endregion Margin

  #region Brush

  public Brush SeparatorBrush {
    get => (Brush)GetValue(SeparatorBrushProperty);
    set => SetValue(SeparatorBrushProperty, value);
  }

  public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(
    nameof(SeparatorBrush),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.LightGray));

  #endregion Brush

  #endregion Separator

  #region Max Dependency Properties

  #region Title

  public string MaxTitle {
    get => (string)GetValue(MaxTitleProperty);
    set => SetValue(MaxTitleProperty, value);
  }

  public static readonly DependencyProperty MaxTitleProperty = DependencyProperty.Register(
    nameof(MaxTitle),
    typeof(string),
    typeof(ValueIndicator),
    new PropertyMetadata(string.Empty));

  #endregion Title

  #region Title FontSize

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double MaxTitleFontSize {
    get => (double)GetValue(MaxTitleFontSizeProperty);
    set => SetValue(MaxTitleFontSizeProperty, value);
  }

  public static readonly DependencyProperty MaxTitleFontSizeProperty = DependencyProperty.Register(
    nameof(MaxTitleFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  #endregion Title FontSize

  #region Title FontWeight

  public FontWeight MaxTitleFontWeight {
    get => (FontWeight)GetValue(MaxTitleFontWeightProperty);
    set => SetValue(MaxTitleFontWeightProperty, value);
  }

  public static readonly DependencyProperty MaxTitleFontWeightProperty = DependencyProperty.Register(
    nameof(MaxTitleFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion Title FontWeight

  #region Title Brush

  public Brush MaxTitleBrush {
    get => (Brush)GetValue(MaxTitleBrushProperty);
    set => SetValue(MaxTitleBrushProperty, value);
  }

  public static readonly DependencyProperty MaxTitleBrushProperty = DependencyProperty.Register(
    nameof(MaxTitleBrush),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  #endregion Title Brush

  /*
  public double MaxValue {
    get => (double)GetValue(MaxValueProperty);
    set => SetValue(MaxValueProperty, value);
  }

  public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
    nameof(MaxValueProperty),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(0.0));
  */

  [TypeConverter(typeof(FontSizeConverter))]
  [Localizability(LocalizationCategory.None)]
  public double MaxValueFontSize {
    get => (double)GetValue(MaxValueFontSizeProperty);
    set => SetValue(MaxValueFontSizeProperty, value);
  }

  public static readonly DependencyProperty MaxValueFontSizeProperty = DependencyProperty.Register(
    nameof(MaxValueFontSize),
    typeof(double),
    typeof(ValueIndicator),
    new PropertyMetadata(10.0));

  public Brush MaxValueBrush {
    get => (Brush)GetValue(MaxValueBrushProperty);
    set => SetValue(MaxValueBrushProperty, value);
  }

  public static readonly DependencyProperty MaxValueBrushProperty = DependencyProperty.Register(
    nameof(MaxValueBrush),
    typeof(Brush),
    typeof(ValueIndicator),
    new PropertyMetadata(Brushes.Black));

  public FontWeight MaxValueFontWeight {
    get => (FontWeight)GetValue(MaxValueFontWeightProperty);
    set => SetValue(MaxValueFontWeightProperty, value);
  }

  public static readonly DependencyProperty MaxValueFontWeightProperty = DependencyProperty.Register(
    nameof(MaxValueFontWeight),
    typeof(FontWeight),
    typeof(ValueIndicator),
    new PropertyMetadata(FontWeights.Regular));

  #endregion Max Dependency Properties
}
