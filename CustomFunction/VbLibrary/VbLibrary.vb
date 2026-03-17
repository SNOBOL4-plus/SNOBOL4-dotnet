Namespace VbLibrary

    ''' <summary>
    ''' Single-method class: auto-prototype discovers the one public instance method.
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Reverser') → REVERSE(s) callable.
    ''' </summary>
    Public Class Reverser
        Public Function Reverse(s As String) As String
            Dim chars() As Char = s.ToCharArray()
            Array.Reverse(chars)
            Return New String(chars)
        End Function
    End Class

    ''' <summary>
    ''' Integer arithmetic — proves long args and return work from VB.
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Arithmetic::Factorial') → FACTORIAL(n).
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Arithmetic::Sum') → SUM(a,b).
    ''' </summary>
    Public Class Arithmetic
        Public Function Factorial(n As Long) As Long
            If n <= 1L Then Return 1L
            Return n * Factorial(n - 1L)
        End Function

        Public Function Sum(a As Long, b As Long) As Long
            Return a + b
        End Function
    End Class

    ''' <summary>
    ''' Real arithmetic — proves Double args and return work from VB.
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Geometry::CircleArea') → CIRCLEAREA(r).
    ''' </summary>
    Public Class Geometry
        Public Function CircleArea(r As Double) As Double
            Return Math.PI * r * r
        End Function
    End Class

    ''' <summary>
    ''' Predicate — proves :S/:F branch works from VB.
    ''' Returns Nothing (null) to signal failure, a non-null value for success.
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Predicate::IsEven') → ISEVEN(n) :S(yes)F(no).
    ''' Note: returns String so the reflect path pushes a StringVar on success.
    ''' On failure the function calls exec.NonExceptionFailure() — but since we
    ''' have no Executive access here, we instead return Nothing and let the
    ''' coercion null-path set Failure = true.
    ''' Simpler: return "1" for true, "" for false and let SNOBOL4 test SIZE.
    ''' Actually simplest: return Long 1 or 0 and test in SNOBOL4.
    ''' </summary>
    Public Class Predicate
        ''' <summary>Returns 1 if n is even, 0 if odd. SNOBOL4 tests the value.</summary>
        Public Function IsEven(n As Long) As Long
            If n Mod 2L = 0L Then Return 1L Else Return 0L
        End Function

        ''' <summary>Returns the input string if non-empty, Nothing (null) if empty.
        ''' Nothing → reflect path sees null → NonExceptionFailure → :F branch.</summary>
        Public Function NonEmptyOrFail(s As String) As String
            If String.IsNullOrEmpty(s) Then Return Nothing
            Return s
        End Function
    End Class

    ''' <summary>
    ''' Static method — proves static dispatch works from VB.
    ''' LOAD('VbLibrary.dll', 'VbLibrary.Formatter::Format') → FORMAT(label,n).
    ''' </summary>
    Public Class Formatter
        Public Shared Function Format(label As String, n As Long) As String
            Return label & "=" & n.ToString()
        End Function
    End Class

End Namespace
