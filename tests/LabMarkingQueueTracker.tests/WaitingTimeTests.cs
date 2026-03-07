using System;
using System.IO;
using Xunit;
using myApplication;

public class WaitingTimeTests
{
    // Helper: clear the shared queue before each test
    private void ClearQueue()
    {
        var queue = CompiledInformation.GetAll();
        while (queue.Count > 0)
            CompiledInformation.RemoveFirst();
    }

    // ─────────────────────────────────────────────
    // Constructor / Getters
    // ─────────────────────────────────────────────

    [Fact]
    public void Constructor_SetsAllFields_Correctly()
    {
        // Arrange & Act
        var wt = new WaitingTime("Alice Smith", 42, 9, 3);

        // Assert
        Assert.Equal("Alice Smith", wt.getFullName());
        Assert.Equal(42,            wt.getseatNumber());
        Assert.Equal(9,             wt.getWaitingTime());
        Assert.Equal(3,             wt.getIndex());
    }

    [Fact]
    public void Constructor_WithZeroWaitingTimeAndZeroIndex_StoresZeros()
    {
        // Arrange & Act
        var wt = new WaitingTime("Bob Jones", 1, 0, 0);

        // Assert
        Assert.Equal(0, wt.getWaitingTime());
        Assert.Equal(0, wt.getIndex());
    }

    // ─────────────────────────────────────────────
    // Setters
    // ─────────────────────────────────────────────

    [Fact]
    public void SetWaitingTime_UpdatesValue()
    {
        // Arrange
        var wt = new WaitingTime("Carol White", 10, 0, 0);

        // Act
        wt.setWaitingTime(15);

        // Assert
        Assert.Equal(15, wt.getWaitingTime());
    }

    [Fact]
    public void SetIndex_UpdatesValue()
    {
        // Arrange
        var wt = new WaitingTime("David Green", 20, 0, 0);

        // Act
        wt.setIndex(4);

        // Assert
        Assert.Equal(4, wt.getIndex());
    }

    [Fact]
    public void SetFullName_UpdatesValue()
    {
        // Arrange
        var wt = new WaitingTime("Eve Brown", 30, 0, 0);

        // Act
        wt.setFullName("Eve Black");

        // Assert
        Assert.Equal("Eve Black", wt.getFullName());
    }

    [Fact]
    public void SetSeatNumber_UpdatesValue()
    {
        // Arrange
        var wt = new WaitingTime("Frank Lee", 50, 0, 0);

        // Act
        wt.setSeatNumber(99);

        // Assert
        Assert.Equal(99, wt.getseatNumber());
    }

    // ─────────────────────────────────────────────
    // _Time() — calculates waiting time and enqueues
    // ─────────────────────────────────────────────

    [Fact]
    public void Time_WhenQueueIsEmpty_SetsWaitingTimeToZeroAndPrintsIt()
    {
        // Arrange
        ClearQueue();
        var wt = new WaitingTime("Grace Hall", 5, 0, 0);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            wt._Time();

            // Assert – waiting time = 0 * 3 = 0
            Assert.Equal(0, wt.getWaitingTime());
            Assert.Contains("Waiting Time : 0", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void Time_WhenOnePersonAlreadyInQueue_SetsWaitingTimeTo3()
    {
        // Arrange
        ClearQueue();
        var first = new WaitingTime("Harry King", 1, 0, 0);
        first._Time();  // index=0 → waitingTime=0, added to queue → count becomes 1

        var second = new WaitingTime("Isla Moore", 2, 0, 1);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            second._Time(); // index=1 → waitingTime = 1*3 = 3

            // Assert
            Assert.Equal(3, second.getWaitingTime());
            Assert.Contains("Waiting Time : 3", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void Time_WhenTwoPeopleAlreadyInQueue_SetsWaitingTimeTo6()
    {
        // Arrange
        ClearQueue();
        new WaitingTime("P1 One",   1, 0, 0)._Time();
        new WaitingTime("P2 Two",   2, 0, 1)._Time();
        var third = new WaitingTime("P3 Three", 3, 0, 2);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            third._Time(); // index=2 → waitingTime = 2*3 = 6

            // Assert
            Assert.Equal(6, third.getWaitingTime());
            Assert.Contains("Waiting Time : 6", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void Time_AddsStudentToCompiledInformationQueue()
    {
        // Arrange
        ClearQueue();
        int countBefore = CompiledInformation.GetCount();
        var wt = new WaitingTime("Jack Taylor", 77, 0, 0);

        var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            // Act
            wt._Time();

            // Assert
            Assert.Equal(countBefore + 1, CompiledInformation.GetCount());
        }
        finally
        {
            Console.SetOut(Console.Out);
            ClearQueue();
        }
    }

    // ─────────────────────────────────────────────
    // _Index() — prints queue position message
    // ─────────────────────────────────────────────

    [Fact]
    public void Index_PrintsQueuePositionAsOneBased()
    {
        // Arrange
        var wt = new WaitingTime("Karen Adams", 15, 0, 2); // index=2 → position 3

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            wt._Index();

            // Assert
            string output = sw.ToString();
            Assert.Contains("Queue Position : 3", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Index_WhenIndexIsZero_PrintsPositionOne()
    {
        // Arrange
        var wt = new WaitingTime("Leo Baker", 8, 0, 0);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            wt._Index();

            // Assert
            Assert.Contains("Queue Position : 1", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Index_PrintsCannotExitQueueWarning()
    {
        // Arrange
        var wt = new WaitingTime("Mia Clark", 25, 0, 0);

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            wt._Index();

            // Assert
            Assert.Contains("You can't exit the queue now", sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
