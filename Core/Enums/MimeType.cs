using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Enums
{
    public enum MimeType
    {
        [MimeInfo("image/jpeg", "jpg", "jpeg")]
        Jpeg,

        [MimeInfo("image/png", "png")]
        Png,

        //[MimeInfo("application/pdf", "pdf")]
        //Pdf,

        //[MimeInfo("video/mp4", "mp4")]
        //Mp4,

        //[MimeInfo("application/zip", "zip")]
        //Zip,

        //[MimeInfo("text/plain", "txt")]
        //Text
    }

}
