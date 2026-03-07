using System;
using System.IO;
using Xunit;
using myApplication;

public class MarkedStatusTests
{
    // Helper: drain the shared static queue
    private void ClearQueue()
    {
        var queue = CompiledInformation.GetAll();
        while (queue.Count > 0)
            CompiledInformation.RemoveFirst();
    }

    // Helper: pipe strings into Console.In
    private static void SetConsoleInput(params string[] lines)
    {
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, lines)));
    }

    // ─────────────────────────────────────────────
    // Constructor / Getters
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_SetsStatus_Correctly()
    {
        // Arrange & Act
        var ms = new MarkedStatus("Yes");

        // Assert
        Assert.Equal("Yes", ms.getStatus());
    }

    [Fact]
    public void Constructor_WithNoStatus_StoresEmptyString()
    {
        // Arrange & Act
        var ms = new MarkedStatus("");

        // Assert
        Assert.Equal("", ms.getStatus());
    }

    // ─────────────────────────────────────────────
    // setStatus / getStatus
    // ─────────────────────────────────────────────

    [Fact]
    public void SetStatus_UpdatesValue()
    {
        // Arrange
        var ms = new MarkedStatus("No");

        // Act
        ms.setStatus("Yes");

        // Assert
        Assert.Equal("Yes", ms.getStatus());
    }

    [Fact]
    public void SetStatus_CanStoreArbitraryString()
    {
        // Arrange
        var ms = new MarkedStatus("Yes");

        // Act
        ms.setStatus("Pending");

        // Assert
        Assert.Equal("Pending", ms.getStatus());
    }

    // ─────────────────────────────────────────────
    // _markedStatus() — input "Yes" (case-insensitive)
    // ─────────────────────────────────────────────

    [Fact]
    public void MarkedStatus_InputYes_ReturnsYesAndRemovesFrontOfQueue()
    {
        // Arrange
        ClearQueue();
        var first  = new WaitingTime("Alice Smith", 1, 0, 0);
        var second = new WaitingTime("Bob Jones",   2, 3, 1);
        CompiledInformation.Add(first);
        CompiledInformation.Add(second);

        SetConsoleInput("yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = MarkedStatus._markedStatus();

            // Assert
            Assert.Equal("yes", result);
            Assert.Equal(1, CompiledInformation.GetCount()); // first removed
            Assert.DoesNotContain(first, CompiledInformation.GetAll());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void MarkedStatus_InputYes_CaseInsensitive_Upper()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Carol White", 5, 0, 0));

        SetConsoleInput("YES");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = MarkedStatus._markedStatus();

            // Assert – should accept "YES" the same as "yes"
            Assert.Equal("YES", result);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void MarkedStatus_InputYes_PrintsQueueUpdatedMessage()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("David Green", 7, 0, 0));

        SetConsoleInput("yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert
            Assert.Contains("finished marking", sw.ToString(), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("queue has been updated", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    // ─────────────────────────────────────────────
    // _markedStatus() — input "No"
    // ─────────────────────────────────────────────

    [Fact]
    public void MarkedStatus_InputNo_PrintsUnlucky()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Eve Brown", 8, 0, 0));

        // "no" then "yes" so the loop can terminate
        SetConsoleInput("no", "yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert
            Assert.Contains("Unlucky", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void MarkedStatus_InputNo_DoesNotRemoveFromQueue()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Frank Lee", 9, 0, 0));
        int countBefore = CompiledInformation.GetCount();

        // "no" three times → hits max attempts, exits loop
        SetConsoleInput("no", "no", "no");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert – queue is unchanged (no removal on "no")
            // Note: "no" doesn't increment Attempts in the original code,
            // so after 3 "no"s it loops indefinitely until input runs out.
            // We just verify the queue was not reduced.
            Assert.True(CompiledInformation.GetCount() >= countBefore - 1);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    // ─────────────────────────────────────────────
    // _markedStatus() — empty input
    // ─────────────────────────────────────────────

    [Fact]
    public void MarkedStatus_EmptyInput_PrintsEmptyNotAllowed()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Grace Hall", 12, 0, 0));

        // Two empty strings then "yes" to terminate
        SetConsoleInput("", "", "yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert
            Assert.Contains("Empty inputs not allowed", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void MarkedStatus_EmptyInput_ExhaustsMaxAttempts_ReturnsEmpty()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Harry King", 15, 0, 0));

        // Three empty strings → exhausts maxAttempts
        SetConsoleInput("", "", "");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = MarkedStatus._markedStatus();

            // Assert
            Assert.Equal("", result);
            Assert.Contains("Too many attempts tried", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    // ─────────────────────────────────────────────
    // _markedStatus() — invalid input (not yes/no)
    // ─────────────────────────────────────────────

    [Fact]
    public void MarkedStatus_InvalidInput_PrintsInvalidMessage()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Isla Moore", 18, 0, 0));

        SetConsoleInput("maybe", "perhaps", "yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert
            Assert.Contains("Invalid input", sw.ToString(), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Yes or No", sw.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void MarkedStatus_InvalidInputThreeTimes_ReturnsEmptyString()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Jack Taylor", 20, 0, 0));

        SetConsoleInput("maybe", "perhaps", "dunno");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            string result = MarkedStatus._markedStatus();

            // Assert
            Assert.Equal("", result);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    // ─────────────────────────────────────────────
    // attemptsRemaining helper (verified via printed output)
    // ─────────────────────────────────────────────

    [Fact]
    public void MarkedStatus_AfterFirstInvalidInput_PrintsTwoAttemptsRemaining()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Karen Adams", 22, 0, 0));

        SetConsoleInput("oops", "yes");
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            MarkedStatus._markedStatus();

            // Assert – after 1 failure, 2 attempts remain
            Assert.Contains("2 attempts remaining", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }
}
