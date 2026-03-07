using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace myApplication;

abstract class Information : IStudentInfo
{

  private const int maxAttempts = 3;
  protected String fullName;
  protected int seatNumber;

  public Information(String fullName, int seatNumber) 
  {
    this.fullName = fullName;
    this.seatNumber = seatNumber;
  }

  public String getFullName() 
  {
    return fullName;
  }

  public void setFullName(String fullName)
  {
    this.fullName = fullName;
  }

  public int getseatNumber() 
  {
    return seatNumber;
  }

  public void setSeatNumber(int seatNumber)
  {
    this.seatNumber = seatNumber;
  }

  private static void attemptsRemaining(int Attempts, int maxAttempts) 
  {
    Console.WriteLine("You have " + (maxAttempts - Attempts) + " attempts remaining.");
  }

  public static String _fullName() {

    int Attempts = 0;
    while (Attempts < maxAttempts) 
    {
      Console.WriteLine("Enter your full name(both first name and last name): ");
      Console.WriteLine("Both first name and last name (only both) must have between 2 and 14 characters");
      String fullName = (Console.ReadLine() ?? "").Trim();
      String[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      bool valid = Regex.IsMatch(fullName, @"^[A-Za-z\- ]+$");

      if (string.IsNullOrEmpty(fullName)) 
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

      if (parts.Length != 2) 
      {

        Attempts++;
        if (Attempts == maxAttempts) 
          {
            Console.WriteLine("Too many attempts tried, please try again later!");
            break;
          }

          Console.WriteLine("This is invalid. There should be both first name and last name!");
          attemptsRemaining(Attempts, maxAttempts);
          continue;
        }

      String firstName = parts[0];
      String lastName = parts[1];

      if (!valid) 
      {

        Attempts++;
        if (Attempts == maxAttempts) 
          {
            Console.WriteLine("Too many attempts tried, please try again later!");
            break;
          }

          Console.WriteLine("There's invalid characters present in the input!");
          attemptsRemaining(Attempts, maxAttempts);
          continue;
        }

      if ((firstName.Length < 2 || firstName.Length > 14) || ((lastName.Length < 2 || lastName.Length > 14)))
      {

       Attempts++;
       if (Attempts == maxAttempts) 
         {
           Console.WriteLine("Too many attempts tried, please try again later!");
           break;
         }

         Console.WriteLine("Either first name or last name has character length out of bounds!");
         attemptsRemaining(Attempts, maxAttempts);
         continue;
       }

      return fullName;
      }

    return "";
  }


  public static int _seatNumber() 
  {

      int Attempts = 0;
      while (Attempts < maxAttempts) 
      {

        try {

          Console.WriteLine("There are currently 500 seats in the room. Enter the seat number from 1 to 500!");
          String seatInput = (Console.ReadLine() ?? "");

          if (string.IsNullOrEmpty(seatInput)) 
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
          int seatNumber = Convert.ToInt32(seatInput.Trim());

          if (seatNumber < 1 || seatNumber > 500) 
          {

            Attempts++;
            if (Attempts == maxAttempts) 
             {
              Console.WriteLine("Too many attempts tried, please try again later!");
              break;
            }

              Console.WriteLine("Seat Numbers are out of range!");
              attemptsRemaining(Attempts, maxAttempts);
              continue;
          }

          bool duplicate = false;
          foreach (var Info in CompiledInformation.GetAll()) 
          {
            WaitingTime entry = (WaitingTime) Info;
            if (entry.getseatNumber() == seatNumber) 
            {
              Console.WriteLine("Your seat number hence name is on the queue already!");
              Console.WriteLine(entry.getWaitingTime());
              Console.WriteLine(entry.getIndex());
              duplicate = true;
              break;
            }
          }
          if (duplicate)
          {
            Attempts++;
            continue;
          }

          return seatNumber;
        }
        catch (Exception)
        {

          Attempts++;
          if (Attempts == maxAttempts) 
          {
            Console.WriteLine("Too many attempts tried, please try again later!");
            break;
          }

            Console.WriteLine("Invalid input! Please enter a number.");
            attemptsRemaining(Attempts, maxAttempts);
            continue;
         }        
        }

      return 0;
      }

     abstract public void _Time();
     abstract public void _Index();
  }
