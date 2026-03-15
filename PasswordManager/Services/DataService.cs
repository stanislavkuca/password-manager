using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PasswordManager.Models;
using System.IO;

namespace PasswordManager.Services
{
    public static class DataService
    {
        private static string path = "Data/data.json";

        public static void Save(AppData data)
        {
            Directory.CreateDirectory("Data");

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        public static AppData Load()
        {
            if (!File.Exists(path))
                return new AppData();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
        }
    }
    
}
