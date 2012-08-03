using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PathOfExile.Util;

namespace PathOfExile.GGPK
{
  public class FileNodeEntry {
    public byte[] Unknown { get; set; }
    public ulong Offset { get; set; }

    public static FileNodeEntry Parse(BinaryReader reader) {
      var unk = reader.ReadBytes(4);
      var offset = reader.ReadUInt64();

      return new FileNodeEntry {
        Unknown = unk,
        Offset = offset,
      };
    }

  }

  public class DirectoryNode : GGPKNode {
    public IEnumerable<FileNodeEntry> FileNodeEntries { get; set; }
    public string Name { get; set; }
    public byte[] SHA256 { get; set; }

    public static new DirectoryNode Parse(BinaryReader reader) {
      var nameLen = reader.ReadInt32();
      var numEntries = reader.ReadInt32();
      var hash = reader.ReadBytes(32);
      var name = reader.ReadGGPKString(nameLen);

      var entries = Enumerable.Range(1, numEntries)
                              .Select(n => FileNodeEntry.Parse(reader))
                              .ToArray();

      return new DirectoryNode {
        SHA256 = hash,
        FileNodeEntries = entries,
        Name = name,
      };
    }
  }
}
