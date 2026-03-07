using System;
using System.IO;
using Xunit;
using myApplication;

/// <summary>
/// Tests for the abstract Information class, exercised through its
/// concrete subclass WaitingTime (the only concrete implementation).
/// All input-validation logic lives in the static helper methods
/// _fullName() and _seatNumber(), tested here via simulated console input.
/// </summary>
public class InformationTests
{
    // Helper: drain the shared static queue to a clean state
    private void ClearQueue()
    {
        var queue = CompiledInformation.GetAll();
        while (queue.Count > 0)
            CompiledInformation.RemoveFirst();
    }

    // Helper: feed strings as simulated console input
    private static void SetConsoleInput(params string[] lines)
    {
        var input = string.Join(Environment.NewLine, lines);
        Console.SetIn(new StringReader(input));
    }

    // ─────────────────────────────────────────────
    // getFullName / setFullName
    // ─────────────────────────────────────────────

    [Fact]
    public void GetFullName_ReturnsNamePassedToConstructor()
    {
        // Arrange & Act
        var info = new WaitingTime("Alice Smith", 10, 0, 0);

        // Assert
        Assert.Equal("Alice Smith", info.getFullName());
    }

    [Fact]
    public void SetFullName_OverwritesPreviousName()
    {
        // Arrange
        var info = new WaitingTime("Bob Jones", 20, 0, 0);

        // Act
        info.setFullName("Bobby Jones");

        // Assert
        Assert.Equal("Bobby Jones", info.getFullName());
    }

    // ─────────────────────────────────────────────
    // getseatNumber / setSeatNumber
    // ─────────────────────────────────────────────

    [Fact]
    public void GetSeatNumber_ReturnsNumberPassedToConstructor()
    {
        // Arrange & Act
        var info = new WaitingTime("Carol White", 77, 0, 0);

        // Assert
        Assert.Equal(77, info.getseatNumber());
    }

    [Fact]
    public void SetSeatNumber_OverwritesPreviousSeat()
    {
        // Arrange
        var info = new WaitingTime("David Green", 50, 0, 0);

        // Act
        info.setSeatNumber(250);

        // Assert
        Assert.Equal(250, info.getseatNumber());
    }

    // ─────────────────────────────────────────────
    // _fullName() — static input validation
    // ─────────────────────────────────────────────

    [Fact]
    public void FullName_ValidInput_ReturnsFullName()
    {
        // Arrange
        SetConsoleInput("Eve Brown");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("Eve Brown", result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_OnlyFirstName_ReturnsEmptyAfterMaxAttempts()
    {
        // Arrange – provide only one word three times, exhausting all attempts
        SetConsoleInput("Alice", "Bob", "Carol");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("", result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_EmptyInput_ReturnsEmptyAfterMaxAttempts()
    {
        // Arrange – three empty lines
        SetConsoleInput("", "", "");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("", result);
            Assert.Contains("Empty inputs not allowed!", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_NumbersInName_ReturnsEmptyAfterMaxAttempts()
    {
        // Arrange – names with digits are invalid
        SetConsoleInput("Alice 123", "Bob 456", "Carol 789");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("", result);
            Assert.Contains("invalid characters", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_FirstNameTooShort_ReturnsEmptyAfterMaxAttempts()
    {
        // Arrange – first name "A" is only 1 character (< 2)
        SetConsoleInput("A Brown", "A Green", "A White");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("", result);
            Assert.Contains("out of bounds", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_FirstNameTooLong_ReturnsEmptyAfterMaxAttempts()
    {
        // Arrange – first name of 15 characters (> 14)
        SetConsoleInput("Abcdefghijklmno Smith", "Abcdefghijklmno Jones", "Abcdefghijklmno Brown");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("", result);
            Assert.Contains("out of bounds", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_InvalidThenValid_ReturnsValidName()
    {
        // Arrange – one bad attempt then a valid name
        SetConsoleInput("BadName", "Alice Smith");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = Information._fullName();

            // Assert
            Assert.Equal("Alice Smith", result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void FullName_MaxAttemptsMessage_PrintedOnFinalAttempt()
    {
        // Arrange – three invalid inputs
        SetConsoleInput("X Y", "X Y", "X Y"); // length 1 < 2
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            Information._fullName();

            // Assert
            Assert.Contains("Too many attempts tried", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    // ─────────────────────────────────────────────
    // _seatNumber() — static input validation
    // ─────────────────────────────────────────────

    [Fact]
    public void SeatNumber_ValidInput_ReturnsSeatNumber()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("100");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(100, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_BoundaryLow_Returns1()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("1");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(1, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_BoundaryHigh_Returns500()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("500");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(500, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_BelowRange_ReturnsZeroAfterMaxAttempts()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("0", "0", "0");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(0, result);
            Assert.Contains("out of range", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_AboveRange_ReturnsZeroAfterMaxAttempts()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("501", "999", "600");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(0, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_NonNumericInput_ReturnsZeroAfterMaxAttempts()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("abc", "xyz", "!!!");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(0, result);
            Assert.Contains("Invalid input", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_EmptyInput_ReturnsZeroAfterMaxAttempts()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("", "", "");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(0, result);
            Assert.Contains("Empty inputs not allowed", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }

    [Fact]
    public void SeatNumber_DuplicateSeat_ReturnsZeroAfterMaxAttempts()
    {
        // Arrange – seat 42 is already in the queue
        ClearQueue();
        var existing = new WaitingTime("Frank Lee", 42, 0, 0);
        CompiledInformation.Add(existing);

        // Three attempts all with seat 42
        SetConsoleInput("42", "42", "42");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(0, result);
            Assert.Contains("on the queue already", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
            ClearQueue();
        }
    }

    [Fact]
    public void SeatNumber_InvalidThenValid_ReturnsValidSeat()
    {
        // Arrange
        ClearQueue();
        SetConsoleInput("0", "250"); // first attempt invalid, second valid
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            int result = Information._seatNumber();

            // Assert
            Assert.Equal(250, result);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(new StringReader(string.Empty));
        }
    }
}
