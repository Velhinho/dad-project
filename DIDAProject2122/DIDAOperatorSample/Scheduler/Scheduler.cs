﻿using System;

namespace Scheduler
{
    public class Scheduler
    {
        static void Main(string[] args)
        {
            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            Console.ReadLine();
        }
    }
}
