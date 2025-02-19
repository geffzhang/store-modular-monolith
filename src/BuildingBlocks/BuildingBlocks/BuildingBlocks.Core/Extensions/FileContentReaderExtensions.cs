﻿using System;
using System.IO;
using System.Text.Json;

namespace BuildingBlocks.Core.Extensions
{
    public static class FileContentReaderExtensions
    {
        public static TData ReadData<TData>(this string fileName, string rootFolder)
        {
            var seedData = Path.GetFullPath(fileName, rootFolder);
            Console.WriteLine(seedData);
            using var sr = new StreamReader(seedData);
            var readData = sr.ReadToEnd();
            var models = JsonSerializer.Deserialize<TData>(
                readData,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            return models;
        }

        public static string SerializeObject(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}