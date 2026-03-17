namespace ReflectFunction;

/// <summary>
/// Single-method class: auto-prototype discovers the one public instance method.
/// LOAD('ReflectLibrary.dll', 'ReflectFunction.Doubler') → DOUBLE(n) callable.
/// </summary>
public class Doubler
{
    public long Double(long n) => n * 2;
}

/// <summary>
/// Single static method: auto-prototype discovers it without instantiation.
/// LOAD('ReflectLibrary.dll', 'ReflectFunction.Greeter') → GREET(s) callable.
/// </summary>
public class Greeter
{
    public static string Greet(string name) => "Hello, " + name + "!";
}

/// <summary>
/// Explicit binding target: has two public methods — auto-prototype must fail
/// without ::MethodName; explicit ::Square succeeds.
/// LOAD('ReflectLibrary.dll', 'ReflectFunction.Calculator::Square') → SQUARE(n).
/// LOAD('ReflectLibrary.dll', 'ReflectFunction.Calculator::Cube')   → CUBE(n).
/// </summary>
public class Calculator
{
    public double Square(double x) => x * x;
    public double Cube(double x)   => x * x * x;
}

/// <summary>
/// Mixed-type method: (string, long) → string.
/// LOAD('ReflectLibrary.dll', 'ReflectFunction.Formatter') → FORMAT(s,n) callable.
/// </summary>
public class Formatter
{
    public string Format(string label, long count) => label + "=" + count;
}
