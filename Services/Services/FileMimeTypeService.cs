using Core.Attributes;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class FileMimeTypeService
    {
        public string GetMime(MimeType type)
        {
            return GetAttribute(type).Mime;
        }

        public IEnumerable<string> GetExtensions(MimeType type)
        {
            return GetAttribute(type).Extensions;
        }

        public MimeType? FromExtension(string extension)
        {
            extension = extension.TrimStart('.').ToLower();

            foreach (var type in Enum.GetValues(typeof(MimeType)).Cast<MimeType>())
            {
                var info = GetAttribute(type);
                if (info.Extensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    return type;
            }

            return null;
        }

        public MimeType? FromMime(string mime)
        {
            mime = mime.ToLower();

            foreach (var type in Enum.GetValues(typeof(MimeType)).Cast<MimeType>())
            {
                var info = GetAttribute(type);
                if (info.Mime.Equals(mime, StringComparison.OrdinalIgnoreCase))
                    return type;
            }

            return null;
        }

        private MimeInfoAttribute GetAttribute(MimeType type)
        {
            var member = type.GetType().GetMember(type.ToString()).First();

            return (MimeInfoAttribute)member
                .GetCustomAttributes(typeof(MimeInfoAttribute), false)
                .First();
        }
    }
}
