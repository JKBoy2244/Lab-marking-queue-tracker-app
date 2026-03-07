using System;
using System.IO;
using Xunit;
using myApplication;

public class CompiledInformationTests
{
    // Helper: drain the shared static queue to a clean state
    private void ClearQueue()
    {
        var queue = CompiledInformation.GetAll();
        while (queue.Count > 0)
            CompiledInformation.RemoveFirst();
    }

    // Helper: suppress console output during queue operations
    private StringWriter SuppressConsole(out TextWriter originalOut)
    {
        var sw = new StringWriter();
        originalOut = Console.Out;
        Console.SetOut(sw);
        return sw;
    }

    // ─────────────────────────────────────────────
    // Add / GetCount
    // ─────────────────────────────────────────────

    [Fact]
    public void Add_OneItem_GetCountReturnsOne()
    {
        // Arrange
        ClearQueue();
        var entry = new WaitingTime("Alice Smith", 1, 0, 0);

        // Act
        CompiledInformation.Add(entry);

        // Assert
        Assert.Equal(1, CompiledInformation.GetCount());

        ClearQueue();
    }

    [Fact]
    public void Add_MultipleItems_GetCountReflectsAll()
    {
        // Arrange
        ClearQueue();

        // Act
        CompiledInformation.Add(new WaitingTime("Alice Smith", 1, 0, 0));
        CompiledInformation.Add(new WaitingTime("Bob Jones",   2, 3, 1));
        CompiledInformation.Add(new WaitingTime("Carol White", 3, 6, 2));

        // Assert
        Assert.Equal(3, CompiledInformation.GetCount());

        ClearQueue();
    }

    [Fact]
    public void GetCount_WhenQueueIsEmpty_ReturnsZero()
    {
        // Arrange
        ClearQueue();

        // Act & Assert
        Assert.Equal(0, CompiledInformation.GetCount());
    }

    // ─────────────────────────────────────────────
    // GetAll
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAll_ReturnsTheSameQueueInstance()
    {
        // Arrange
        ClearQueue();
        var entry = new WaitingTime("David Green", 10, 0, 0);
        CompiledInformation.Add(entry);

        // Act
        var queue = CompiledInformation.GetAll();

        // Assert – the queue is not null and contains our entry
        Assert.NotNull(queue);
        Assert.Contains(entry, queue);

        ClearQueue();
    }

    [Fact]
    public void GetAll_WhenEmpty_ReturnsEmptyQueue()
    {
        // Arrange
        ClearQueue();

        // Act
        var queue = CompiledInformation.GetAll();

        // Assert
        Assert.Empty(queue);
    }

    [Fact]
    public void GetAll_PreservesInsertionOrder_FIFO()
    {
        // Arrange
        ClearQueue();
        var first  = new WaitingTime("Eve Brown",  1, 0, 0);
        var second = new WaitingTime("Frank Lee",  2, 3, 1);
        var third  = new WaitingTime("Grace Hall", 3, 6, 2);

        CompiledInformation.Add(first);
        CompiledInformation.Add(second);
        CompiledInformation.Add(third);

        // Act
        var items = new System.Collections.Generic.List<Information>(CompiledInformation.GetAll());

        // Assert – queue is FIFO, first added should be first in list
        Assert.Equal(first,  items[0]);
        Assert.Equal(second, items[1]);
        Assert.Equal(third,  items[2]);

        ClearQueue();
    }

    // ─────────────────────────────────────────────
    // RemoveFirst
    // ─────────────────────────────────────────────

    [Fact]
    public void RemoveFirst_ReducesCountByOne()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Harry King", 5, 0, 0));
        CompiledInformation.Add(new WaitingTime("Isla Moore", 6, 3, 1));

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act
            CompiledInformation.RemoveFirst();

            // Assert
            Assert.Equal(1, CompiledInformation.GetCount());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void RemoveFirst_RemovesTheFirstEntrant_NotTheSecond()
    {
        // Arrange
        ClearQueue();
        var first  = new WaitingTime("Jack Taylor", 10, 0, 0);
        var second = new WaitingTime("Karen Adams", 11, 3, 1);
        CompiledInformation.Add(first);
        CompiledInformation.Add(second);

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act
            CompiledInformation.RemoveFirst();

            // Assert – first is gone, second remains
            Assert.DoesNotContain(first,  CompiledInformation.GetAll());
            Assert.Contains(second, CompiledInformation.GetAll());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void RemoveFirst_UpdatesRemainingEntries_IndexAndWaitingTime()
    {
        // Arrange
        ClearQueue();
        var first  = new WaitingTime("Leo Baker",  1, 0, 0);
        var second = new WaitingTime("Mia Clark",  2, 3, 1);
        var third  = new WaitingTime("Noah Davis", 3, 6, 2);
        CompiledInformation.Add(first);
        CompiledInformation.Add(second);
        CompiledInformation.Add(third);

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act — remove the first person; second and third should be re-indexed
            CompiledInformation.RemoveFirst();

            // Assert – second is now index 0, waiting time 0
            Assert.Equal(0, second.getIndex());
            Assert.Equal(0, second.getWaitingTime());

            // Assert – third is now index 1, waiting time 3
            Assert.Equal(1, third.getIndex());
            Assert.Equal(3, third.getWaitingTime());
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void RemoveFirst_PrintsUpdatedPositionAndWaitingTime_ForRemainingStudents()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Olivia Evans", 20, 0, 0));
        CompiledInformation.Add(new WaitingTime("Peter Ford",   21, 3, 1));

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            // Act
            CompiledInformation.RemoveFirst();

            // Assert – the console output mentions the remaining student's updated info
            string output = sw.ToString();
            Assert.Contains("Peter Ford", output);
            Assert.Contains("Updated Queue Position : 1", output);
            Assert.Contains("Updated Waiting Time : 0", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            ClearQueue();
        }
    }

    [Fact]
    public void RemoveFirst_WhenQueueIsEmpty_DoesNotThrow()
    {
        // Arrange
        ClearQueue();

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act & Assert – should not throw
            var exception = Record.Exception(() => CompiledInformation.RemoveFirst());
            Assert.Null(exception);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void RemoveFirst_WhenQueueIsEmpty_CountRemainsZero()
    {
        // Arrange
        ClearQueue();

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act
            CompiledInformation.RemoveFirst();

            // Assert
            Assert.Equal(0, CompiledInformation.GetCount());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void RemoveFirst_CalledRepeatedly_DrainsFully()
    {
        // Arrange
        ClearQueue();
        CompiledInformation.Add(new WaitingTime("Quinn Harris", 30, 0, 0));
        CompiledInformation.Add(new WaitingTime("Rachel Ivy",   31, 3, 1));
        CompiledInformation.Add(new WaitingTime("Sam James",    32, 6, 2));

        var sw = SuppressConsole(out TextWriter originalOut);

        try
        {
            // Act
            CompiledInformation.RemoveFirst();
            CompiledInformation.RemoveFirst();
            CompiledInformation.RemoveFirst();

            // Assert
            Assert.Equal(0, CompiledInformation.GetCount());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
