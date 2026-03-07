using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace myApplication;

class MarkedStatus
{

  private const int maxAttempts = 3;
  private String status;

  public MarkedStatus(String status) 
  {
    this.status = status;
  }

  public String getStatus() 
  {
    return status;
  }

  public void setStatus(String status)
  {
    this.status = status;
  }

  private static void attemptsRemaining(int Attempts, int maxAttempts) 
  {
    Console.WriteLine("You have " + (maxAttempts - Attempts) + " attempts remaining.");
  }

  public static String _markedStatus() 
  {

    int Attempts = 0;

    while (Attempts != maxAttempts) {
      Console.WriteLine("Marked yet: Yes/No?");
      String status = (Console.ReadLine() ?? "").Trim();

      if (string.IsNullOrEmpty(status)) 
      {
        Attempts++;
        if (Attempts == maxAttempts) 
        {
          Console.WriteLine("Too many attempts tried, please try again later!");
          break;
        }

        Console.WriteLine("Empty inputs not allowed!");
        attemptsRemaining(Attempts, maxAttempts);
        continue;
     }

      if (!status.Equals("yes", StringComparison.OrdinalIgnoreCase) && !status.Equals("no", StringComparison.OrdinalIgnoreCase))
      {

        Attempts++;
        if (Attempts == maxAttempts) 
        {
          Console.WriteLine("Too many attempts tried, please try again later!");
          break;
        }

        Console.WriteLine("Invalid input! Please enter Yes or No.");
        attemptsRemaining(Attempts, maxAttempts);
        continue;
     }

      if (status.Equals("no", StringComparison.OrdinalIgnoreCase))
      {

        Console.WriteLine("Unlucky!");
        continue;
      }

      CompiledInformation.RemoveFirst();
      Console.WriteLine("The person at the front of the queue has finished marking. The queue has been updated!");
      return status;
   }
   return "";
  }
}
