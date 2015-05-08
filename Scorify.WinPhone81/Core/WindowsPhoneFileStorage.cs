using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Scorify.WinPhone81.Core;
using ScorifyApp.Core;
using Xamarin.Forms;

[assembly: Dependency(typeof(WindowsPhoneFileStorage))]
namespace Scorify.WinPhone81.Core
{
    
    public class WindowsPhoneFileStorage : IFileStorage
    {
         public string LoadText(string filename) {
            var task = LoadFromFileAsync(filename);
            task.Wait(); // HACK: to keep Interface return types simple (sorry!)
            return task.Result;
        }
        protected async Task<string> LoadFromFileAsync(string filename)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            if (local != null)
            {
                bool exists = false;
                try
                {
                    await local.GetFileAsync(filename);
                    exists = true;
                }
                catch (Exception ex)
                {
                    // ignored
                }
                if (exists)
                {
                    var file = await local.GetFileAsync(filename);
                    using (StreamReader streamReader = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        var text = streamReader.ReadToEnd();
                        return text;
                    }
                }
            }
            return "";
        }
        protected async Task<bool> SaveToFileAsync(string filename, string text)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            var writer = null as StreamWriter;
            bool success = true;
            try
            {
                writer = new StreamWriter(await file.OpenStreamForWriteAsync());
                writer.Write(text);
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (writer != null) writer.Dispose();
            }
            return success;
        }

        public async Task<bool> SaveToFile(string fileName, string content)
        {
            var isSaved = await SaveToFileAsync(fileName, content);
            return isSaved;
        }

        public async Task<string> LoadFromFile(string fileName)
        {
            try
            {
                var loadFromFile = await LoadFromFileAsync(fileName);
                return loadFromFile;
            }
            catch(Exception ex)
            {
                return "";
            }
            
        }
    }

}
