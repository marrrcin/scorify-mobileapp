using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ScorifyApp.Core;
using ScorifyApp.Droid.Core;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(AndroidFileStorage))]
namespace ScorifyApp.Droid.Core
{
    public class AndroidFileStorage : IFileStorage
    {
        public async Task<bool> SaveToFile(string fileName, string content)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, fileName);
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            bool isSuccess = true;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Create);
                streamWriter = new StreamWriter(fileStream);
                await streamWriter.WriteAsync(content);
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
            return isSuccess;
        }

        public async Task<string> LoadFromFile(string fileName)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, fileName);
            if (!File.Exists(filePath))
            {
                return "";
            }

            FileStream fileStream = null;
            StreamReader streamReader = null;
            string content = "";
            try
            {
                fileStream = new FileStream(filePath,FileMode.Open);
                streamReader=new StreamReader(fileStream);
                content = await streamReader.ReadToEndAsync();
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Dispose();
                }

                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
            return content;
        }
    }
}