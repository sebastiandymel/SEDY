using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID.LSP
{
    public class Rectangle: Shape
    {
        public Rectangle(float w, float h)
        {
            Width = w;
            Heigth = h;
        }

        public Rectangle()
        {
            
        }

        private float Heigth { get; set; }
        private float Width { get; set; }

        public override float Area
        {
            get { return Heigth * Width; }
        }
    }

    public abstract class Shape
    {
        public abstract float Area { get; }
    }

    public class Square : Shape
    {
        private readonly float x;

        public Square(float x)
        {
            this.x = x;
        }

        public override float Area => this.x * this.x;
    }
}
