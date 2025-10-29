using System;

public partial class Vector
{
    public Vector Add(int number)
    {
        int[] resultElements = new int[Size];
        for (int i = 0; i < Size; i++)
        {
            resultElements[i] = _elements[i] + number;
        }
        return new Vector(resultElements);
    }

    public Vector Multiply(int number)
    {
        int[] resultElements = new int[Size];
        for (int i = 0; i < Size; i++)
        {
            resultElements[i] = _elements[i] * number;
        }
        return new Vector(resultElements);
    }

    public double GetMagnitude()
    {
        double sumOfSquares = 0;
        foreach (int element in _elements)
        {
            sumOfSquares += Math.Pow(element, 2);
        }
        return Math.Sqrt(sumOfSquares);
    }
}