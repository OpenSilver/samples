using OpenSilverShowcase.Support.UI.Units;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenSilverShowcase.DragandDrop.Examples;

public partial class DragandDropItem : ShowcaseItem
{
    private bool _isPointerCaptured;
    private double _pointerX;
    private double _pointerY;
    private double _objectLeft;
    private double _objectTop;

    public DragandDropItem()
    {
        this.InitializeComponent();
        PlantList.ItemsSource = Planet.Planets;
    }

    void DragAndDropItem_PointerPressed(object sender, MouseButtonEventArgs e)
    {
        UIElement uielement = (UIElement)sender;
        _objectLeft = Canvas.GetLeft(uielement);
        _objectTop = Canvas.GetTop(uielement);

        _pointerX = e.GetPosition(null).X;
        _pointerY = e.GetPosition(null).Y;
        uielement.CaptureMouse();
        _isPointerCaptured = true;
    }

    void DragAndDropItem_PointerMoved(object sender, MouseEventArgs e)
    {
        UIElement uielement = (UIElement)sender;
        if (_isPointerCaptured)
        {
            double deltaH = e.GetPosition(null).X - _pointerX;
            double deltaV = e.GetPosition(null).Y - _pointerY;
            _objectLeft = deltaH + _objectLeft;
            _objectTop = deltaV + _objectTop;

            Canvas.SetLeft(uielement, _objectLeft);
            Canvas.SetTop(uielement, _objectTop);

            _pointerX = e.GetPosition(null).X;
            _pointerY = e.GetPosition(null).Y;
        }
    }

    void DragAndDropItem_PointerReleased(object sender, MouseButtonEventArgs e)
    {
        UIElement uielement = (UIElement)sender;
        _isPointerCaptured = false;
        uielement.ReleaseMouseCapture();
    }
}