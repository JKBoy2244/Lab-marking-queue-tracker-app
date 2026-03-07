using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace myApplication;

class WaitingTime : Information
 {

   private int waitingTime;
   private int index;

   public WaitingTime(String fullName, int seatNumber, int waitingTime, int index)
     : base(fullName, seatNumber)
   {
     this.waitingTime = waitingTime;
     this.index = index;
   }

   public int getWaitingTime() 
   {
     return waitingTime;
   }

   public void setWaitingTime(int waitingTime)
   {
     this.waitingTime = waitingTime;
   }

   public int getIndex() 
   {
     return index;
   }

   public void setIndex(int index)
   {
     this.index = index;
   }

   public override void _Time() 
   {

    int Index = CompiledInformation.GetCount();
    int waitingTime = (Index)*3;
    this.waitingTime = waitingTime;
    CompiledInformation.Add(this);
    Console.WriteLine("Waiting Time : " + waitingTime);

   }

   public override void _Index() 
   {

     Console.WriteLine("Queue Position : " + (index+1));
     Console.WriteLine("You can't exit the queue now, otherwise if you want to mark later, you can tell them so but be sure before joining the queue!");
   }
 }
