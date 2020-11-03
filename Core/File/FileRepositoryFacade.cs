using System.Collections.Generic;


namespace Parsia.Core.File
{
    public static class FileRepositoryFacade
    {
        public static readonly Dictionary<string, string> MimType
            = new Dictionary<string, string>()
            {
                {"doc", "application/msword"},
                {"dot", "application/msword"},
                {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {"docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {"dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {"xls", "application/vnd.ms-excel"},
                {"xlt", "application/vnd.ms-excel"},
                {"xla", "application/vnd.ms-excel"},
                {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {"xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {"xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {"xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {"xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {"xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {"ppt", "application/vnd.ms-powerpoint"},
                {"pot", "application/vnd.ms-powerpoint"},
                {"pps", "application/vnd.ms-powerpoint"},
                {"ppa", "application/vnd.ms-powerpoint"},
                {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {"potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {"ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {"ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {"pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {"potm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {"ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {"odt", "application/vnd.oasis.opendocument.text"},
                {"ott", "application/vnd.oasis.opendocument.text-template"},
                {"oth", "application/vnd.oasis.opendocument.text-web"},
                {"odm", "application/vnd.oasis.opendocument.text-master"},
                {"odg", "application/vnd.oasis.opendocument.graphics"},
                {"otg", "application/vnd.oasis.opendocument.graphics-template"},
                {"odp", "application/vnd.oasis.opendocument.presentation"},
                {"otp", "application/vnd.oasis.opendocument.presentation-template"},
                {"ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {"ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
                {"odc", "application/vnd.oasis.opendocument.chart"},
                {"odf", "application/vnd.oasis.opendocument.formula"},
                {"odb", "application/vnd.oasis.opendocument.database"},
                {"odi", "application/vnd.oasis.opendocument.image"},
                {"oxt", "application/vnd.openofficeorg.extension"},
                {"txt", "text/plain"},
                {"rtf", "application/rtf"},
                {"pdf", "application/pdf"},
                {"rar", "application/x-rar-compressed"},
                {"zip", "application/x-zip-compressed"},
                {"gz", "application/gzip-compressed"},
                {"7z", "application/x-7z-compressed"},
                {"tar", "application/x-tar"},
                {"jpg","image/jpg" }
            };

        public static string ReplaceValidCharacter(string text)
        {
            return text.Replace("@d@", "/")
                .Replace("@p@", "+")
                .Replace("@i@", "-")
                .Replace("@m@", "*");
        }
        public static string ReplaceInValidCharacter(string text)
        {
            return text.Replace("/", "@d@")
                .Replace("+", "@p@")
                .Replace("-", "@i@")
                .Replace("*", "@m@");
        }
    }
}
