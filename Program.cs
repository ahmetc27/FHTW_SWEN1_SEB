﻿using SEB.Models;
using SEB.Http;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server(10001);
            server.Start();
        }
    }
}