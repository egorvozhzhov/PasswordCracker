using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordCracker
{
  class Program
  {
    public static bool foundFlag = false;
    static bool flag = true;
    static void Main(string[] args)
    {
      List<string> targetHashes = new List<string>
            {
                "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad",
                "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
                "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f"
            };
      while (flag)
      {
        Console.WriteLine("По какому хеш значению выполнить подбор пароля? ");
        Console.WriteLine("1. 1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad");
        Console.WriteLine("2. 3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b");
        Console.WriteLine("3. 74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f");
        int selection = int.Parse(Console.ReadLine());
       

     
        Console.Write("Количество потоков: ");
        int countStream = int.Parse(Console.ReadLine());
        Console.WriteLine("Выполняется подбор пароля...");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        if (countStream == 1)
        {
          RunSingleThreaded(targetHashes[selection - 1]);
        }
        else
        {
          RunMultiThreaded(targetHashes[selection - 1], countStream);
        }
        stopwatch.Stop();
        Console.WriteLine($"Подбор завершен. Затраченное время: {stopwatch.Elapsed}");
        Console.WriteLine("Введите ENTER, чтобы продожить");
        Console.WriteLine();
        Console.ReadKey();
        foundFlag = false;
      }




      static void RunSingleThreaded(string targetHashes)
      {
        char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        foreach (char c1 in chars)
        {
          foreach (char c2 in chars)
          {
            foreach (char c3 in chars)
            {
              foreach (char c4 in chars)
              {
                foreach (char c5 in chars)
                {
                  if (foundFlag!=true)
                  {
                    string password = new string(new[] { c1, c2, c3, c4, c5 });
                    string hash = GetSHA256Hash(password);
                    if (targetHashes.Contains(hash))
                    {
                      Console.WriteLine($"Найден пароль: {password}, соответствующий хэшу : {hash}");
                      foundFlag = true;
                      break;
                    }
                  }
                  else
                  {
                    return;
                  }
                }
              }
            }
          }
        }
      }

      static void RunMultiThreaded(string targetHashes, int numThreads)
      {
        char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        int chunkSize = chars.Length / numThreads;

        List<Task> tasks = new List<Task>();

        // Создаем задачи для каждого потока
        for (int i = 0; i < numThreads; i++)
        {
          int startIndex = i * chunkSize;
          int endIndex = (i == numThreads - 1) ? chars.Length : startIndex + chunkSize;
          char[] threadChars = new char[endIndex - startIndex];
          Array.Copy(chars, startIndex, threadChars, 0, endIndex - startIndex);

          tasks.Add(Task.Run(() =>
          {
            foreach (char c1 in threadChars)
            {
              foreach (char c2 in chars)
              {
                foreach (char c3 in chars)
                {
                  foreach (char c4 in chars)
                  {
                    foreach (char c5 in chars)
                    {
                      if (foundFlag != true)
                      {
                        string password = new string(new[] { c1, c2, c3, c4, c5 });
                        string hash = GetSHA256Hash(password);
                        if (targetHashes.Contains(hash))
                        {
                          Console.WriteLine($"Найден пароль: {password}, соответствующий хэшу SHA-256: {hash}");
                          foundFlag = true;
                          break;
                        }
                      }
                      else
                      {
                        return;
                      }
                        
                    }
                  }
                }
              }
            }
          }));
        }

        // Ожидаем завершения всех задач
        Task.WaitAll(tasks.ToArray());
      }

      static string GetSHA256Hash(string password)
      {
        using (SHA256 sha256 = SHA256.Create())
        {
          byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
          StringBuilder builder = new StringBuilder();
          for (int i = 0; i < hashBytes.Length; i++)
          {
            builder.Append(hashBytes[i].ToString("x2")); // 16-ое представление
          }
          return builder.ToString();
        }
      }
    }
  }
}