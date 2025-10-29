public partial class Array 
{
    static public Array operator *(Array left, Array right)
    {
        int minSize = Math.Min(left.Size, right.Size);
        Array longestArray = left;
        if (minSize == left.Size)
        {
            longestArray = right;
        }
        Array result = new Array(longestArray.Size);
        for (int i = 0; i < longestArray.Size; i++) 
        {
            if (i < minSize)
            {
                result.Elements[i] = left.Elements[i] * right.Elements[i];
            }
            else
            {
                result.Elements[i] = longestArray.Elements[i];
            }
        }
        return result;
    }

    static public bool operator true(Array arr)
    {
        for (int i = 0; i < arr.Size; i++)
        {
            if (arr.Elements[i] < 0)
            {
                return false;
            }
        }
        return true;
    }
    static public bool operator false(Array arr)
    {
        for (int i = 0; i < arr.Size; i++)
        {
            if (arr.Elements[i] < 0)
            {
                return true;
            }
        }
        return false;
    }
    static public bool operator ==(Array left, Array right)
    {
        if (left.Size != right.Size)
        {
            return false;
        }
        for (int i = 0; i < right.Size; i++)
        {
            if (left.Elements[i] != right.Elements[i])
            {
                return false;
            }
        }
        return true;
    }
    static public bool operator !=(Array left, Array right)
    {
        if (left.Size != right.Size)
        {
            return true;
        }
        for (int i = 0; i < right.Size; i++)
        {
            if (left.Elements[i] != right.Elements[i])
            {
                return true;
            }
        }
        return false;
    }
    static public bool operator >(Array left, Array right)
    {
        int left_weight = 0;
        int right_weight = 0;
        for (int i = 0; i < right.Size; i++)
        {
            right_weight += right.Elements[i];
        }
        for (int i = 0; i < left.Size; i++)
        {
            left_weight += left.Elements[i];
        }
        return left_weight > right_weight;
    }
    static public bool operator <(Array left, Array right)
    {
        int left_weight = 0;
        int right_weight = 0;
        for (int i = 0; i < right.Size; i++)
        {
            right_weight += right.Elements[i];
        }
        for (int i = 0; i < left.Size; i++)
        {
            left_weight += left.Elements[i];
        }
        return left_weight < right_weight;
    }

    public static explicit operator int(Array array)
    {
        return array.Size;
    }
}