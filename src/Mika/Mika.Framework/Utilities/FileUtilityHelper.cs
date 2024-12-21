using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mika.Framework.Models;
namespace Mika.Framework.Utilities
{
    public static class FileUtilityHelper
    {
        public static OperationResult<object> CheckImage(this IFormFile file, int maxLengthInMB)
        {
            OperationResult<object> OP = new("CheckImage");
            if (file == null || file.Length == 0)
            {
                return OP.Failed("تصویری برای ذخیره ارسال نشده است");
            }
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var fileExtention = System.IO.Path.GetExtension(file.FileName);
            var forbiddenExtensions = new List<string>
            {
                ".asax",".ascx",".ashx",".asmx",".aspx",".axd",".browser",".cd",".php",".compile",".config",".cs",".jsl",".vb",".csproj",".vbproj",".vjsproj",".disco",".vsdisco",".dsdgm",".dsprototype",".dll",".licx",".webinfo",".master",".mdb",".ldb",".mdf",".msgx",".svc",".rem",".resources",".resx",".sdm",".sdmDocument",".sitemap",".skin",".sln",".soap",".asa",".asp",".cdx",".cer",".idc",".shtm",".shtml",".stm",".css",".htm",".html",".perl"
            };
            if (!fileName.IsNotNull())
            {
                return OP.Failed("نام تصویر نادرست است");
            }
            if (forbiddenExtensions.Any(x => fileName.ToLower().Contains(x.ToLower()) || fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (fileExtention != ".jpeg" && fileExtention != ".jpg" && fileExtention != ".png" && fileExtention != ".gif" && fileExtention != ".svg")
            {
                return OP.Failed("فرمت های مجاز تصویر jpeg ، jpg ، svg و png است.");
            }
            if (file.Length > (maxLengthInMB * 1024 * 1024))
            {
                return OP.Failed($"حداکثر اندازه مجاز عکس ، {maxLengthInMB.ToPersianNumber()} مگابایت است.");
            }
            return OP.Succeed("تصویر مجاز به ذخیره است");
        }
        public static OperationResult<object> CheckImage(this IFormFile file)
        {
            OperationResult<object> OP = new("CheckImage");
            if (file == null || file.Length == 0)
            {
                return OP.Failed("تصویری برای ذخیره ارسال نشده است");
            }
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var fileExtention = System.IO.Path.GetExtension(file.FileName);
            var forbiddenExtensions = new List<string>
            {
                ".asax",".ascx",".ashx",".asmx",".aspx",".axd",".browser",".cd",".php",".compile",".config",".cs",".jsl",".vb",".csproj",".vbproj",".vjsproj",".disco",".vsdisco",".dsdgm",".dsprototype",".dll",".licx",".webinfo",".master",".mdb",".ldb",".mdf",".msgx",".svc",".rem",".resources",".resx",".sdm",".sdmDocument",".sitemap",".skin",".sln",".soap",".asa",".asp",".cdx",".cer",".idc",".shtm",".shtml",".stm",".css",".htm",".html",".perl"
            };
            if (!fileName.IsNotNull())
            {
                return OP.Failed("نام تصویر نادرست است");
            }
            if (forbiddenExtensions.Any(x => fileName.ToLower().Contains(x.ToLower()) || fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (fileExtention != ".jpeg" && fileExtention != ".jpg" && fileExtention != ".png" && fileExtention != ".gif" && fileExtention != ".svg")
            {
                return OP.Failed("فرمت های مجاز تصویر jpeg ، jpg ، svg و png است.");
            }
            if (file.Length > (3 * 1024 * 1024))
            {
                return OP.Failed($"حداکثر اندازه مجاز عکس ، {3.ToPersianNumber()} مگابایت است.");
            }
            return OP.Succeed("تصویر مجاز به ذخیره است");
        }
        public static OperationResult<object> CheckFile(this IFormFile file, int maxLengthInMB)
        {
            OperationResult<object> OP = new("CheckFile");
            if (file == null || file.Length == 0)
            {
                return OP.Failed("فایلی برای ذخیره ارسال نشده است");
            }
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var fileExtention = System.IO.Path.GetExtension(file.FileName);
            var forbiddenExtensions = new List<string>
            {
                ".asax",".ascx",".ashx",".asmx",".aspx",".axd",".browser",".cd",".php",".compile",".config",".cs",".jsl",".vb",".csproj",".vbproj",".vjsproj",".disco",".vsdisco",".dsdgm",".dsprototype",".dll",".licx",".webinfo",".master",".mdb",".ldb",".mdf",".msgx",".svc",".rem",".resources",".resx",".sdm",".sdmDocument",".sitemap",".skin",".sln",".soap",".asa",".asp",".cdx",".cer",".idc",".shtm",".shtml",".stm",".css",".htm",".html",".perl"
            };
            var acceptableExtensions = new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".csv" };
            if (!fileName.IsNotNull())
            {
                return OP.Failed("نام تصویر نادرست است");
            }
            if (forbiddenExtensions.Any(x => fileName.ToLower().Contains(x.ToLower()) || fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (!acceptableExtensions.Any(x => fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (file.Length > (maxLengthInMB * 1024 * 1024))
            {
                return OP.Failed($"حداکثر اندازه مجاز عکس ، {maxLengthInMB.ToPersianNumber()} مگابایت است.");
            }
            return OP.Succeed("تصویر مجاز به ذخیره است");
        }
        public static OperationResult<object> CheckFile(this IFormFile file)
        {
            OperationResult<object> OP = new("CheckFile");
            if (file == null || file.Length == 0)
            {
                return OP.Failed("فایلی برای ذخیره ارسال نشده است");
            }
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var fileExtention = System.IO.Path.GetExtension(file.FileName);
            var forbiddenExtensions = new List<string>
            {
                ".asax",".ascx",".ashx",".asmx",".aspx",".axd",".browser",".cd",".php",".compile",".config",".cs",".jsl",".vb",".csproj",".vbproj",".vjsproj",".disco",".vsdisco",".dsdgm",".dsprototype",".dll",".licx",".webinfo",".master",".mdb",".ldb",".mdf",".msgx",".svc",".rem",".resources",".resx",".sdm",".sdmDocument",".sitemap",".skin",".sln",".soap",".asa",".asp",".cdx",".cer",".idc",".shtm",".shtml",".stm",".css",".htm",".html",".perl"
            };
            var acceptableExtensions = new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".csv" };
            if (!fileName.IsNotNull())
            {
                return OP.Failed("نام تصویر نادرست است");
            }
            if (forbiddenExtensions.Any(x => fileName.ToLower().Contains(x.ToLower()) || fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (!acceptableExtensions.Any(x => fileExtention.ToLower().Contains(x.ToLower()) || fileExtention.ToLower() == x.ToLower()))
            {
                return OP.Failed("فرمت فایل مجاز نمیباشد");
            }
            if (file.Length > (3 * 1024 * 1024))
            {
                return OP.Failed($"حداکثر اندازه مجاز عکس ، {3.ToPersianNumber()} مگابایت است.");
            }
            return OP.Succeed("تصویر مجاز به ذخیره است");
        }
        public static string GetUniqeFileName(this IFormFile file)
        {
            return string.Concat(DateTime.Now.Ticks.ToString(), "_", System.IO.Path.GetFileName(file.FileName));
        }
        public static string GetContentType(this string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private static Dictionary<string, string> GetMimeTypes() => new()
        {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
        };
    }
}
