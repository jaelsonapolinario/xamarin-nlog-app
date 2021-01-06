using AppNLog.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace AppNLog.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetDocumentPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        }
    }
}