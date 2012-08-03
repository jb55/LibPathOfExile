using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PathOfExile.Util;

namespace PathOfExile.GGPK
{

  public class FileNode : GGPKNode {
    public string FileName { get; set; }
    public byte[] SHA256 { get; set; }
    public long DataSize { get; set; }
    public long DataOffset { get; set; }

    public static FileNode Parse(long size, BinaryReader reader) {
      var offset = reader.BaseStream.Position;

      var fileNameLen = reader.ReadInt32();
      var hash = reader.ReadBytes(32);
      var fileName = reader.ReadGGPKString(fileNameLen);

      var dataOffset = reader.BaseStream.Position;
      var nonDataLen = dataOffset - offset;
      var remaining = size - nonDataLen;

      return new FileNode {
        FileName = fileName,
        SHA256 = hash,
        DataSize = remaining,
        DataOffset = dataOffset,
      };
    }

    public void SaveAs(GGPKFileStream stream, string path, string fileName = null) {
      GGPKFile.ExportFileNode(stream, this, path, fileName);
    }
  }

}
