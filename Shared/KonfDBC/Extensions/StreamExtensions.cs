using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KonfDB.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static string ReadToEnd(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
