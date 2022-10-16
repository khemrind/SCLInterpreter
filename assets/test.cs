public static class test
{
    const string default_message = "hello ";
    const int default_size = 5;

    public static void displayMessage(string word)
    {
        string message;
        message = default_message + word;
        Console.WriteLine(message + word + "!");
    }

    public static int main()
    {
        double result;
        int size;
        Console.WriteLine("test.scl main function begins..");
        result = 4 / 5;
        Console.WriteLine("value of 4 / 5" + result);
        size = default_size;
        while (size > 0)
        {
            displayMessage("world");
            size = size - 1;
        }
        return 0;
    }
}