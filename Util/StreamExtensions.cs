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

    public static string ReadGGPKString(this BinaryReader reader, int len) {
      len = (len - 1) * 2;
      var name = Encoding.Unicode.GetString(reader.ReadBytes(len));
      // skip the null terminator
      reader.BaseStream.Skip(2);
      return name;
    }
  }
}
