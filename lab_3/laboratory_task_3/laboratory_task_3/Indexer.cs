public partial class Array
{
    public int this[int index]
    {
        get
        {
            if (index >= 0 && index < Size)
            {
                return Elements[index];
            }
            else
            {
                throw new IndexOutOfRangeException("Index is outside the bounds of the array.");
            }
        }

        set
        {
            if (index >= 0 && index < Size)
            {
                Elements[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("Index is outside the bounds of the array.");
            }
        }
    }
}