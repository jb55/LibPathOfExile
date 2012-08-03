using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PathOfExile.Util;
using Data.Maybe;

namespace PathOfExile.GGPK
{

  public class GGPKFile
  {
    public string FilePath { get; private set; }
    public HeaderNode Header { get; private set; }
    public IEnumerable<GGPKNode> Nodes { get; private set; }

    public GGPKFile(string path=null, bool autoload=true) {
      if (path == null) {
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
          @"Grinding Gear Games\Path of Exile\Content.ggpk");
      }

      FilePath = path;

      if (autoload)
        Load();
    }

    private static FileNodeEntry ParseFileNodeEntry(BinaryReader reader) {
      var unk = reader.ReadBytes(4);
      var offset = reader.ReadUInt32();

      return new FileNodeEntry {
        Unknown = unk,
        Offset = offset,
      };
    }

    private static FileNode ParseDirectoryNode(BinaryReader reader) {
      return null;

//    var entries = Enumerable.Range(1, numEntries)
//                            .Select(n => ParseFileNodeEntry(reader))
//                            .ToArray();
    }

    private static FileNode ParseFileNode(long size, BinaryReader reader) {
      var offset = reader.BaseStream.Position;

      var fileNameLen = (reader.ReadInt32() - 1) * 2;
      var hash = reader.ReadBytes(32);
      var fileName = Encoding.Unicode.GetString(reader.ReadBytes(fileNameLen));

      // skip the null terminator
      reader.BaseStream.Skip(2);

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

    public static void ExportFileNode(GGPKFileStream source, FileNode fileNode, string outPath, string fileName=null) {
      var srcReader = new BinaryReader(source);
      var original = source.Position;
      var size = (int)fileNode.DataSize;
      var dir = Path.GetDirectoryName(outPath);
      var fullPath = Path.Combine(dir, fileName ?? fileNode.FileName);

      source.Seek(fileNode.DataOffset, SeekOrigin.Begin);

      if (dir != "")
        Directory.CreateDirectory(dir);

        using (var file = File.Create(fullPath))
        using (var outWriter = new BinaryWriter(file)) {
          if (fileNode.DataSize != 0) {
            outWriter.Write(srcReader.ReadBytes(size));
          }
        }

      source.Seek(original, SeekOrigin.Begin);
    }

    private static HeaderNode ParseHeaderNode(BinaryReader reader) {
      var numOffsets = reader.ReadUInt32();
      var offsets = new ulong[numOffsets];

      for (var i = 0; i < numOffsets; ++i) {
        offsets[i] = reader.ReadUInt64();
      }

      return new HeaderNode {
        FileOffsets = offsets
      };
    }

    public GGPKFileStream GetStream() {
      return new GGPKFileStream(this);
    }

    public static Maybe<T> ParseSpecificNode<T>(BinaryReader reader) where T : GGPKNode {
      return ParseNode(reader).As<GGPKNode, T>();
    }

    public static Maybe<GGPKNode> ParseNode(BinaryReader reader) {
      var startPos = reader.BaseStream.Position;
      var len = reader.ReadUInt32();
      var tag = ASCIIEncoding.ASCII.GetString(reader.ReadBytes(4));

      GGPKNode node = null;

      switch (tag) {
        case "GGPK": node = ParseHeaderNode(reader); break;
        case "FILE": node = ParseFileNode(len - 8, reader); break;
      }

      reader.BaseStream.Seek(startPos + len, SeekOrigin.Begin);
      return node.ToMaybe();
    }

    public static bool IsValidMagicCode(string code) {
      return code == "GGPK";
    }

    public static bool IsValidMagicCode(byte[] code) {
      return IsValidMagicCode(ASCIIEncoding.ASCII.GetString(code));
    }

    public void Load(BinaryReader reader = null) {
      using (reader = reader ?? new BinaryReader(new FileStream(FilePath, FileMode.Open))) {

        var maybeHeader = ParseSpecificNode<HeaderNode>(reader);
        Header = maybeHeader.FromMaybe(() => new Exception("Could not parse header, invalid format"));

        var nodes = new List<GGPKNode>();
        while (reader.BaseStream.Position != reader.BaseStream.Length) {
          var maybeNode = ParseSpecificNode<FileNode>(reader);
          if (maybeNode.HasValue) {
            nodes.Add(maybeNode.Value);
          }
        }

        Nodes = nodes;
      }
    }

  }
}
