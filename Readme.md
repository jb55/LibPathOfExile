
# LibPathOfExile

Tools for parsing Path of Exile's GGPK files and the data files contained therein

WIP

## Examples

### Dump entire tree
```cs
var ggpk = new GGPKFile();
ggpk.Dump("dump/to/path");
```

### Dump subtree
var ggpk = new GGPKFile();

var skillIcons =	(from dir in ggpk.Nodes.OfType<DirectoryNode>()
									where dir.Name == "SkillIcons"
									select ggpk.NodeToTreeNode(dir)).First();

ggpk.DumpTreeNode(skillIcons, "tmp");

### Export all csvs

```cs
var ggpk = new GGPKFile();
var fileNodes = ggpk.Nodes.OfType<FileNode>();

using (var fs = ggpk.GetStream()) {
	foreach (var fileNode in fileNodes) {
		if (fileNode.FileName.EndsWith(".csv")) {
			fileNode.SaveAs(fs, ".");
		}
	}
}
```

### Explore directory tree

```cs
public static void PrintTree(TreeNode node, string path) {
	if (node.IsFileNode) {
		var fileNode = node.FileNode;
		var filePath = Path.Combine(path, fileNode.FileName);
		Console.WriteLine(filePath);
	} else if (node.IsDirectoryTreeNode) {
		var dirNode = node.DirectoryTreeNode;
		var subPath = Path.Combine(path, dirNode.Node.Name);
		foreach (var child in dirNode.Children) {
			PrintTree(child, subPath);
		}
	}
}

var ggpk = new GGPKFile();
var root = PrintTree(ggpk.GetDirectoryTree(), "");
```
