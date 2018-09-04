using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace SOLID.SRP
{
    //public class DrawingService
    //{
    //    public void MarkPixel(IShapeRendered renderer)
    //    {
    //        renderer.Render(new WindowDrawer());
    //    }
    //}

    //public class WindowDrawer : IRenderContext
    //{
    //    public void Draw(int x, int y)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public interface ICanRender
    //{
    //    void Render(IRenderContext context);
    //}

    //public interface IRenderContext
    //{
    //    void Draw(int x, int y);
    //}
    
    //public interface IAreaCalculator
    //{
    //    double CalcArea();
    //}

    //public class Rectangle: IAreaCalculator
    //{
    //    private readonly IAreaCalculator areaCalculator;
    //    private readonly IShapeRendered shapeRenderer;
    //    private Point _leftTopCorner;
    //    private Point _rightBottomCorner;

    //    public Rectangle(IAreaCalculator areaCalculator, IShapeRendered shapeRenderer)
    //    {
    //        this.areaCalculator = areaCalculator;
    //        this.shapeRenderer = shapeRenderer;
    //    }

    //    public double CalcArea()
    //    {
    //        return this.areaCalculator.CalcArea(this._leftTopCorner, this._rightBottomCorner);
    //    }

    //    public void Render(IRenderContext context)
    //    {
    //        this.shapeRenderer.Render(context);
    //    }
    //}

    //public class RectangleAreaCalculator : IAreaCalculator
    //{

    //}

    //public class RectangleRendered : IShapeRendered
    //{
    //    public void Render(IRenderContext context);
    //}

    //public interface IShapeRendered
    //{
    //    void Render(IRenderContext context);
    //}
}
