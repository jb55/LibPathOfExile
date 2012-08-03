using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PathOfExile.GGPK
{
  public class HeaderNode : GGPKNode {
    public IEnumerable<ulong> FileOffsets { get; set; }

    public static new HeaderNode Parse(BinaryReader reader) {
      var numOffsets = reader.ReadUInt32();
      var offsets = new ulong[numOffsets];

      for (var i = 0; i < numOffsets; ++i) {
        offsets[i] = reader.ReadUInt64();
      }

      return new HeaderNode {
        FileOffsets = offsets
      };
    }
  }
}
