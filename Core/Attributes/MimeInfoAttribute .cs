using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MimeInfoAttribute : Attribute
    {
        public string Mime { get; }
        public string[] Extensions { get; }

        public MimeInfoAttribute(string mime, params string[] extensions)
        {
            Mime = mime;
            Extensions = extensions;
        }
    }

}
