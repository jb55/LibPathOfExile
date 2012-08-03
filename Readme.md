
# LibPathOfExile

Tools for parsing Path of Exile's GGPK files and the data files contained therein

WIP

## Examples

### Export all csvs

```cs
var file = new GGPKFile();
var fileNodes = file.Nodes.OfType<FileNode>();

using (var fs = file.GetStream()) {
	foreach (var fileNode in fileNodes) {
		if (fileNode.FileName.EndsWith(".csv")) {
			fileNode.SaveAs(fs, ".");
		}
	}
}
```

