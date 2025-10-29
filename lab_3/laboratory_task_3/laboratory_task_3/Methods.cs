public partial class Array 
{
    public int getSize()
    {
        return Size;
    }
    public bool setSize(int newSize)
    {
        if (newSize < 0)
        {
            return false;
        }
        else
        {
            int[] newElements = new int[newSize];
            for (int i = 0; i < newSize; i++)
            {
                if (i >= Size)
                {
                    newElements[i] = 0;
                }
                else
                {
                    newElements[i] = Elements[i];
                }
            }
            Elements = newElements;
            Size = newSize;
            return true;
        }
    }
    public int[] getElements()
    {
        return Elements;
    }
    public bool setElements(int[] newElements)
    {
        if (newElements.Length < 0)
        {
            return false;
        }
        else
        {
            Size = newElements.Length;
            Elements = newElements;
            return true;
        }
    }
    public void append(int newElement)
    {
        int[] newElements = new int[Size + 1];
        for (int i = 0; i < Size; i++) 
        {
            newElements[i] = Elements[i];
        }
        newElements[Size] = newElement;
        Size++;
        Elements = newElements;
    }
    public void printArray()
    {
        Console.Write("{");
        for (int i = 0; i < Size; i++)
        {
            Console.Write(Elements[i]);
            if (i < Elements.Length - 1)
            {
                Console.Write("; ");
            }
        }
        Console.Write("}");
    }

    public bool removeNegativeElements() 
    {
        Array newArray = new Array(); 
        for (int i = 0; i < Size; i++)
        {
            if (Elements[i] >= 0) 
            {
                newArray.append(Elements[i]);
            }
        }
        Size = newArray.Size;
        Elements = newArray.Elements;
        return true;
    }

    public string GetOrganizationName()
    {
        return DeveloperProduction.GetOrganizationName();
    }
    public int GetId()
    {
        return DeveloperProduction.GetId();
    }
    public Production GetDeveloperProduction()
    {
        return DeveloperProduction;
    }
}