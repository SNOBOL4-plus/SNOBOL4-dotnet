// See https://aka.ms/new-console-template for more information
using System.Net.NetworkInformation;
using System.Text;

PrintChars("Hello");
PrintChars("你好");
PrintChars("𐓏𐓘𐓻𐓘𐓻𐓟 𐒻𐓟");

//Print();

void PrintChars(string s)
{
    Console.OutputEncoding = Encoding.UTF8;
    Console.WriteLine($"\"{s}\".Length = {s.Length}");
    for (int i = 0; i < s.Length; i++)
    {
        Console.WriteLine($"s[{i}] = '{s[i]}' ('\\u{(int)s[i]:x4}')");
    }
    Console.WriteLine();
}

void Print()
{
    // Set the console's output encoding to UTF-8
    Console.OutputEncoding = Encoding.UTF8;

    // Example Unicode characters
    Console.WriteLine("Hello, World!");
    Console.WriteLine("你好 (Nǐ hǎo) - Chinese");
    Console.WriteLine("😊 (Smiling Face with Smiling Eyes) - Emoji");
    Console.WriteLine("λ (Lambda) - Greek letter");

    Console.ReadKey(); // Keep the console window open
}

