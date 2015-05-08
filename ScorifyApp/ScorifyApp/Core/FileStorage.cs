using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScorifyApp.Core
{
    public class FileStorage
    {
        public static async Task<bool> SaveToFile(string fileName, string content)
        {
            var storage = DependencyService.Get<IFileStorage>();
            return storage != null && await storage.SaveToFile(fileName, content);
        }

        public static async Task<string> LoadFromFile(string fileName)
        {
            var storage = DependencyService.Get<IFileStorage>();
            if (storage != null)
            {
                return await storage.LoadFromFile(fileName);
            }
            return "";
        }
    }

    public interface IFileStorage
    {
        Task<bool> SaveToFile(string fileName, string content);

        Task<string> LoadFromFile(string fileName);
    }
}
