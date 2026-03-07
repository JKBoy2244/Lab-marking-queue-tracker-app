using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace myApplication;

class CompiledInformation 
{
  private static Queue<Information> CompiledInfo = new Queue<Information>();

  public static void Add(Information info)
  {
    CompiledInfo.Enqueue(info);
  }

  public static int GetCount()
  {
    return CompiledInfo.Count;
  }

  public static void RemoveFirst()
  {
      if (CompiledInfo.Count > 0)
      {
          CompiledInfo.Dequeue();
          int i = 0;
          foreach (Information item in CompiledInfo)
          {
              WaitingTime entry = (WaitingTime) item;
              entry.setIndex(i);
              entry.setWaitingTime(i * 3);
              Console.WriteLine(entry.getFullName() + " - Updated Queue Position : " + (i + 1) + ", Updated Waiting Time : " + (i * 3) + " minutes");
              i++;
          }
      }
  }

  public static Queue<Information> GetAll()
  {
    return CompiledInfo;
  }
}
