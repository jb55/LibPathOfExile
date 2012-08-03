using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PathOfExile.Util;
using Data.Maybe;

namespace PathOfExile.GGPK
{
  public class ParseError {
    public enum ParseErrorReason {
      UnknownNode,
      WrongNode
    }

    public ParseErrorReason Reason { get; set; }
  }

  public class GGPKNode {
    public ulong Offset;

    public static Either<ParseError, T> ParseSpecificNode<T>(BinaryReader reader) where T : GGPKNode {
      return GGPKNode.Parse(reader).As<ParseError, GGPKNode, T>(() =>
        new ParseError { Reason = ParseError.ParseErrorReason.WrongNode });
    }

    public static Either<ParseError, GGPKNode> Parse(BinaryReader reader) {
      var startPos = reader.BaseStream.Position;
      var len = reader.ReadUInt32();
      var tag = ASCIIEncoding.ASCII.GetString(reader.ReadBytes(4));

      GGPKNode node = null;

      switch (tag) {
        case "GGPK": node = HeaderNode.Parse(reader); break;
        case "FILE": node = FileNode.Parse(len - 8, reader); break;
        case "PDIR": node = DirectoryNode.Parse(reader); break;
      }

      if (node != null)
        node.Offset = (ulong)startPos;

      reader.BaseStream.Seek(startPos + len, SeekOrigin.Begin);
      return node.ToRight<ParseError, GGPKNode>();
    }

  }
}
