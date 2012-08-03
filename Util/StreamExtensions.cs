using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PathOfExile.Util
{
  public static class Extensions
  {
    public static long Skip(this Stream stream, long len) {
      return stream.Seek(len, SeekOrigin.Current);
    }

    public static string ReadGGPKString(this BinaryReader reader) {
      var size  = reader.ReadInt32();
      var bytes = reader.ReadBytes(size);
      return Encoding.Unicode.GetString(bytes);
    }
  }
}
