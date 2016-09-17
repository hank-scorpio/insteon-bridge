using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Moar.Http
{
    public static class StreamTools
    {
    	public static void CopyStream(Stream from, Stream to)
    	{
    		BinaryReader reader = new BinaryReader(from);
    		BinaryWriter writer = new BinaryWriter(to);
    		byte[] buf = new byte[4096];
    		int len = 0;
    		while ((len = reader.Read(buf, 0, buf.Length)) > 0)
    		{
    			writer.Write(buf, 0, len);
    		}
    		reader.Close();
    		writer.Close();
    	}

    }
}
