using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PathOfExile.GGPK
{
  // Stream newtype wrapper
  public class GGPKFileStream : Stream {
    private Stream Stream { get; set; }

    public GGPKFileStream(GGPKFile file) {
      Stream = new FileStream(file.FilePath, FileMode.Open);
    }

    protected override void Dispose(bool disposing) {
      Stream.Dispose();
    }

    public override bool  CanRead {
    	get { return Stream.CanRead; }
    }

    public override bool  CanSeek {
    	get { return Stream.CanSeek; }
    }

    public override bool  CanWrite {
    	get { return Stream.CanWrite; }
    }

    public override void  Flush() {
     	Stream.Flush();
    }

    public override long  Length {
    	get { return Stream.Length; }
    }

    public override long  Position {
  	  get { 
    		return Stream.Position;
    	}
  	  set { 
    		Stream.Position = value;
    	}
    }

    public override int  Read(byte[] buffer, int offset, int count) {
      return Stream.Read(buffer, offset, count);
    }

    public override long  Seek(long offset, SeekOrigin origin) {
      return Stream.Seek(offset, origin);
    }

    public override void  SetLength(long value) {
      Stream.SetLength(value);
    }

    public override void  Write(byte[] buffer, int offset, int count) {
      Stream.Write(buffer, offset, count);
    }
  }
}
