using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homer.Insteon
{
    public class Bitey
    {
    	public static byte[] ToBytes(object st)
    	{
    		int size = Marshal.SizeOf(st);
    		byte[] arr = new byte[size];
    		IntPtr ptr = Marshal.AllocHGlobal(size);
    		Marshal.StructureToPtr(st, ptr, true);
    		Marshal.Copy(ptr, arr, 0, size);
    		Marshal.FreeHGlobal(ptr);
    		return arr;
    	}
    
    	public static S ToStruct<S>(byte[] arr) where S : new()
    	{
    		int size = Marshal.SizeOf(typeof(S));
    		IntPtr ptr = Marshal.AllocHGlobal(size);
    		Marshal.Copy(arr, 0, ptr, size);
    		var ret = Marshal.PtrToStructure(ptr, typeof(S));
    		Marshal.FreeHGlobal(ptr);
    		return (S) ret;
    	}


    }
}
