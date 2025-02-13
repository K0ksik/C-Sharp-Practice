using System;
class Rectangle
{
    public int Width { get; set; }
    public int Height { get; set; }
    public static string Text;

    public Rectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public Rectangle(Rectangle rectangle)
    {
        Width = rectangle.Width;
        Height = rectangle.Height;
    }
    public Rectangle()
    {
        Width = 10;
        Height = 10;
    }
    static Rectangle()
    {
        Text = "Прямоугольник";
        Console.WriteLine(Text);
    }

    ~Rectangle()
    {
        Console.WriteLine($"Деструктор.");
    }

    public int Area()
    {
        return Width * Height;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Rectangle rectangle = new Rectangle(5,10);
        Console.WriteLine(Rectangle.Text);
        Console.WriteLine($"Площадь: {rectangle.Area()}");

    }
}
