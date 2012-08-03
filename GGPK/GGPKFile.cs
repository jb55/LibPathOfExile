using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PathOfExile.Util;
using Data.Maybe;
using System.Diagnostics;

namespace PathOfExile.GGPK
{

  public class GGPKFile
  {
    public string FilePath { get; private set; }
    public HeaderNode Header { get; private set; }
    public IEnumerable<GGPKNode> Nodes { get; private set; }
    private IDictionary<ulong, GGPKNode> OffsetsToNodes = new Dictionary<ulong, GGPKNode>();

    public GGPKFile(string path=null, bool autoload=true) {
      if (path == null) {
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
          @"Grinding Gear Games\Path of Exile\Content.ggpk");
      }

      FilePath = path;

      if (autoload)
        Load();
    }

    public static void ExportFileNode(GGPKFileStream stream, FileNode fileNode, string outPath, string fileName=null) {
      var srcReader = new BinaryReader(stream);
      var original = stream.Position;
      var size = (int)fileNode.DataSize;
      var dir = Path.GetDirectoryName(outPath);
      var fullPath = Path.Combine(dir, fileName ?? fileNode.FileName);

      stream.Seek(fileNode.DataOffset, SeekOrigin.Begin);

      if (dir != "")
        Directory.CreateDirectory(dir);

        using (var file = File.Create(fullPath))
        using (var outWriter = new BinaryWriter(file)) {
          if (fileNode.DataSize != 0) {
            outWriter.Write(srcReader.ReadBytes(size));
          }
        }

      stream.Seek(original, SeekOrigin.Begin);
    }

    public void DumpTreeNode(TreeNode n, string path, bool skipExisting = false) {
      using (var fs = GetStream()) {
        DumpTreeNode(fs, n, path, skipExisting);
      }
    }

    public static void DumpTreeNode(GGPKFileStream stream, TreeNode n, string path, bool skipExisting=false) {
      if (n.IsFileNode) {
        var fileNode = n.Element.Left;
        var filePath = Path.Combine(path, fileNode.FileName);
        if (skipExisting && File.Exists(filePath))
          return;
        ExportFileNode(stream, n.Element.Left, filePath);
      }
      else if (n.IsDirectoryTreeNode) {
        var dir = n.Element.Right;
        var subPath = Path.Combine(path, dir.Node.Name);
        Directory.CreateDirectory(subPath);

        foreach (var child in dir.Children) {
          DumpTreeNode(stream, child, subPath, skipExisting);
        }
      }
    }

    public void Dump(string path, bool skipExisting=false) {
      var tree = GetDirectoryTree();

      using (var stream = GetStream()) {
        DumpTreeNode(stream, tree, path, skipExisting);
      }
    }

    public GGPKFileStream GetStream() {
      return new GGPKFileStream(this);
    }

    public TreeNode NodeToTreeNode(GGPKNode node) {
      return NodeToTreeNode(node, this.OffsetsToNodes);
    }

    public static TreeNode NodeToTreeNode(GGPKNode node, IDictionary<ulong, GGPKNode> offsets) {
        if (node is FileNode) {
          return new TreeNode { 
            Element = (node as FileNode).ToLeft<FileNode, DirectoryTreeNode>()
          };
        }
        else if (node is DirectoryNode) {
          var dnode = node as DirectoryNode;

          var children = dnode.FileNodeEntries.Select(nodeEntry => {
            var offset = offsets[nodeEntry.Offset];
            var newNode = NodeToTreeNode(offset, offsets);
            return newNode;
          }).Where(tn => tn != null);

          return new TreeNode {
            Element = new DirectoryTreeNode {
              Node = dnode,
              Children = children
            }.ToRight<FileNode, DirectoryTreeNode>()
          };
        }

        return null;
    }

    public TreeNode GetDirectoryTree() {
      return BuildDirectoryTree(Nodes, OffsetsToNodes);
    }

    private static TreeNode BuildDirectoryTree(IEnumerable<GGPKNode> nodes, IDictionary<ulong, GGPKNode> offsets) {
      foreach (var node in nodes.OfType<DirectoryNode>()) {
        if (node.Name == "")
          return NodeToTreeNode(node, offsets);
      }
      return null;
    }

    public void Load(BinaryReader reader = null) {
      using (reader = reader ?? new BinaryReader(new FileStream(FilePath, FileMode.Open))) {

        var maybeHeader = GGPKNode.ParseSpecificNode<HeaderNode>(reader);
        Header = maybeHeader.FromEither(() => new Exception("Could not parse header, invalid format"));

        var nodes = new List<GGPKNode>();
        while (reader.BaseStream.Position != reader.BaseStream.Length) {
          var eitherNode = GGPKNode.Parse(reader);

          eitherNode.Run(node => {
            if (node is FileNode || node is DirectoryNode)
              OffsetsToNodes[node.Offset] = node;

            nodes.Add(node);
          });
        }

        Nodes = nodes;
      }
    }

  }
}
