using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApi.Handlers.Helpers
{
    public static class FileHelper
    {
        public static async Task<bool> AddOrOverwriteFile(string relativePath, string fileName, string jsonInput)
        {
            var path = Directory.GetCurrentDirectory();
            if (!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }
            try
            {
                if (CheckRelativePath(relativePath))
                {
                    path += $@"\{relativePath}" + fileName;
                    using (FileStream stream = File.Create(path))
                    {
                        byte[] input = new UTF8Encoding(true).GetBytes(jsonInput);
                        await stream.WriteAsync(input);
                        stream.Dispose(); //Ensuring that open stream won't influence checking file existing
                    }
                    if (File.Exists(path))
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    throw new Exception($"Unable to access proper directory. Relative path: {relativePath}");
                }
            }catch (Exception e)
            {
                throw new Exception("Unable to save data to a file", e);
            }
        }

        public static async Task<T> GetObjectFromFile<T> (string relativePath, string filename)
        {
            try
            {
                var content = await GetFilesByName(relativePath, filename);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                throw new ApiException(System.Net.HttpStatusCode.NotFound, "Could not find files for specific order");
            }
        }

        public static async Task<string> GetFilesByName(string relativePath, string fileName, bool useRelativePath = true)
        {
            if (!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }
            var path = Directory.GetCurrentDirectory() + $@"\{relativePath}" + fileName;
            if (!useRelativePath)
            {
                path = relativePath;
            }
            string text = await File.ReadAllTextAsync(path);
            return text;
        }

        //Assumes that all files in specific directory represents specific object Type
        public static async Task<List<T>>GetAllDataForDirectory<T>(string relativePath)
        {
            var path = Directory.GetCurrentDirectory() + $@"\{relativePath}";
            var files = Directory.GetFiles(path);
            var results = new List<T>();

            foreach(string filename in files)
            {
                results.Add(JsonConvert.DeserializeObject<T>(await GetFilesByName(filename, "", false)));
            }

            return results;
        }

        public static int GetLastId(string relativePath)
        {
            var path = Directory.GetCurrentDirectory() + $@"\{relativePath}";
            var fileNames = Directory.GetFiles(path);
            var IdsList = new List<int>();

            foreach(string fileName in fileNames)
            {
                var Id = fileName;
                int cutIndex = fileName.IndexOf(".");
                if(cutIndex >= 0)
                {
                    Id = fileName.Substring(0, cutIndex);
                }
                IdsList.Add(Int16.Parse(Id));
            }
            if(IdsList.Count == 0)
            {
                return 1000;
            }
            else
            {
                IdsList.Sort();
                return IdsList.Last();
            }
        }

        private static bool CheckRelativePath(string relativePath)
        {
            var current = Directory.GetCurrentDirectory();
            var folders = relativePath.Split(@"\").ToList();
            try
            {
                foreach (string folder in folders)
                {
                    current += @$"\{folder}";
                    if (!Directory.Exists(current))
                    {
                        Directory.CreateDirectory(current);
                    }
                }
                if(Directory.Exists(Directory.GetCurrentDirectory() + $@"\{relativePath}"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }catch (Exception e)
            {
                throw new Exception("Exception occured when creating files directory", e);
            }
        }
    }
}
