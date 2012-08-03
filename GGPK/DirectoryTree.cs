using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathOfExile.Util;

namespace PathOfExile.GGPK
{
  public class DirectoryTreeNode {
    public DirectoryNode Node { get; set; }
    public IEnumerable<TreeNode> Children { get; set; }
  }

  public class TreeNode {
    public Either<FileNode, DirectoryTreeNode> Element { get; set; }

    public bool IsFileNode { get { return Element.IsLeft; } }
    public bool IsDirectoryTreeNode { get { return Element.IsRight; } }
    public FileNode FileNode { get { return Element.Left; } }
    public DirectoryTreeNode DirectoryTreeNode { get { return Element.Right; } }
  }
}
