using System.Collections.Generic;
using System.IO;

namespace Moar.Http
{
    public static class MimeType
    {
		

		static IReadOnlyDictionary<string, string> MimeTypes = new Dictionary<string, string> 
        {
            {"PDF",     Pdf},
            {"JPG",     Jpeg},
            {"JPEG",    Jpeg},
            {"PNG",     Png},
            {"GIF",     Gif},
            {"TIFF",    Tiff},
            {"TIF",     Tiff},
            {"JS",      Javascript},
            {"XML",     Xml},
            {"XSL",     Xslt},
            {"XSLT",    Xslt},
            {"CSS",     Less},	
            {"LESS ",   Css},
            {"TXT",     Text},	
            {"HTML",    Html},
            {"HTM",     Html},	
            {"JSON",    Json}
        };

        public const string Binary     = "application/octet-stream";
        public const string Text       = "text/plain";
        public const string Html       = "text/html";
        public const string Xml        = "text/xml";
        public const string Css        = "text/css";
        public const string Javascript = "application/x-javascript";
        public const string Json       = "application/json";
        public const string Pdf        = "application/pdf";
        public const string Jpeg       = "image/jpeg";
        public const string Png        = "image/png";
        public const string Gif        = "image/gif";
        public const string Tiff       = "image/tiff";
        public const string AutoDetect = "text/plain";

		public const string Less = Css;
		public const string Xslt = Xml;


		public const string DefaultBinaryMimeType = Binary;
		public const string DefaultTextMimeType   = Text;

		public static string GetFromPath(string path)
		{
			string ext = Path.GetExtension(path).ToUpper().TrimStart('.');
			string ret = null;
			if (MimeTypes.TryGetValue(ext, out ret))
				return ret;
			return Binary;
		}




    }
}
