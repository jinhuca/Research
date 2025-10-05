using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static Module.Infrastructure.Controls.VisualStates;

namespace Module.Infrastructure.Controls
{
	[TemplateVisualState(Name = StateHidden, GroupName = GroupVisibility)]
	[TemplateVisualState(Name = StateVisible, GroupName = GroupVisibility)]
	public class BusyMask : ContentControl
	{
		//[Description("Gets or sets whether the indicator is busy.")]
		//public bool IsBusy
		//{
		//	get => (bool)GetValue(IsBusyProperty);
		//	set => SetValue(IsBusyProperty, value);
		//}

		//public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy",
		//				typeof(bool),
		//				typeof(BusyMask),
		//				new PropertyMetadata(false, OnIsBusyChanged));

		//[Category(nameof(BusyIndicator))]
		[Description("Gets or sets the indicator type.")]
		public IndicatorType IndicatorType
		{
			get => (IndicatorType)GetValue(IndicatorTypeProperty);
			set => SetValue(IndicatorTypeProperty, value);
		}

		public static readonly DependencyProperty IndicatorTypeProperty =
				DependencyProperty.Register("IndicatorType",
						typeof(IndicatorType),
						typeof(BusyMask),
						new PropertyMetadata(IndicatorType.Grid));

		//[Category(nameof(BusyIndicator))]
		[Description("Gets or sets the control which gets focused after the wait is over.")]
		public Control FocusAfterBusy
		{
			get => (Control)GetValue(FocusAfterBusyProperty);
			set => SetValue(FocusAfterBusyProperty, value);
		}

		public static readonly DependencyProperty FocusAfterBusyProperty =
				DependencyProperty.Register("FocusAfterBusy",
						typeof(Control),
						typeof(BusyMask),
						new PropertyMetadata(null));

		static BusyMask()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyMask),
					new FrameworkPropertyMetadata(typeof(BusyMask)));
		}

		//private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		//{
		//	((BusyMask)d).OnIsBusyChanged(e);
		//}

		//protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
		//{
		//	if (!(bool)e.NewValue)
		//	{
		//		if (FocusAfterBusy != null)
		//		{
		//			FocusAfterBusy.Dispatcher.Delay(100, (_) =>
		//			{
		//				FocusAfterBusy.Focus();
		//			});
		//		}
		//	}

		//	ChangeVisualState((bool)e.NewValue);
		//}

		//public override void OnApplyTemplate()
		//{
		//	ChangeVisualState();
		//}

		//protected virtual void ChangeVisualState(bool isBusyContentVisible = false)
		//{
		//	VisualStateManager.GoToState(this, isBusyContentVisible ? "Visible" : "Hidden", true);
		//}
	}
}
