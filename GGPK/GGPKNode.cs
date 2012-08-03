using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PathOfExile.GGPK
{
  public class GGPKNode {
  }

  public class HeaderNode : GGPKNode {
    public IEnumerable<ulong> FileOffsets { get; set; }
  }

  public class FileNode : GGPKNode {
    public string FileName { get; set; }
    public byte[] SHA256 { get; set; }
    public long DataSize { get; set; }
    public long DataOffset { get; set; }

    public void SaveAs(GGPKFileStream stream, string path, string fileName = null) {
      GGPKFile.ExportFileNode(stream, this, path, fileName);
    }
  }

  public class FileNodeEntry {
    public byte[] Unknown { get; set; }
    public ulong Offset { get; set; }
  }

}
