using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace myApplication;

class Program 
{
  public static void Main (string[] args) 
  {

    while (true) 
    {

      String name = Information._fullName();
      if (string.IsNullOrEmpty(name)) continue;
      int seat = Information._seatNumber();
      if (seat == 0) continue;
      WaitingTime time = new WaitingTime(name, seat, 0, CompiledInformation.GetCount());
      time._Time();
      time._Index();

      if (time.getIndex() == 0) 
      {
        Thread.Sleep(50000);

        MarkedStatus._markedStatus();
     }
   }
  }
}
