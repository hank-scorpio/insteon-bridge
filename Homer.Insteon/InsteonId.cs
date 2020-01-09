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
	[StructLayout(LayoutKind.Sequential)]
    public struct InsteonId : IEnumerable<byte>, IEquatable<InsteonId>,  IEquatable<string>, IEquatable<uint>
    {
    	public static InsteonId Zero        { get; } = new InsteonId(0x00, 0x00, 0x00);
    	public static InsteonId Broadcast 	{ get; } = new InsteonId(0xFF, 0xFF, 0xFF);

        public uint Value { get; }

        public byte A1 => (byte)(Value >> 16);
        public byte A2 => (byte)(Value >> 8);
        public byte A3 => (byte)(Value);
    	public byte[] Bytes => new[] { A1, A2, A3 };


        #region GetValue

        public static uint GetValue(byte a1, byte a2, byte a3) 
            => (uint) ((a1 << 16) | (a2 << 8) | (a3));

        public static uint GetValue(byte[] arr, int i = 0)
        {
            if (arr?.Length < 3)
                throw new ArgumentException(nameof(arr), $"Array must be non-null and contain at least 3 elements.");
            if (i < 0 || i > arr.Length - 3)
                throw new ArgumentOutOfRangeException(nameof(i), $"Offset must be between 0 and {arr.Length - 3}.");

            return GetValue(arr[i], arr[i + 1], arr[i + 2]);
        }

        #endregion


        #region Constructors

        public InsteonId(uint value) 
            => Value = value & 0x00FFFFFF;

        public InsteonId(byte a1, byte a2, byte a3) 
            : this(GetValue(a1, a2, a3))
        { }
        
        public InsteonId(byte[] arr, int i = 0)    
            : this(GetValue(arr, i))
        { }

        public InsteonId(string s)
            : this(ParseValue(s))
    	{ }

        public InsteonId(InsteonId id) 
            : this(id.Value)
    	{ }

    	#endregion
    

    	#region Cast Operators
         
    	public static implicit operator InsteonId(uint val)
    	    => new InsteonId(val);

        public static implicit operator InsteonId(Enum val)
    	    => (InsteonId)Convert.ToUInt32(val);

        public static implicit operator InsteonId(ulong val)
    	    => (InsteonId)(uint)val;

        public static implicit operator InsteonId(long val)
    	    => (InsteonId)(uint)val;
    	
    	public static implicit operator InsteonId(string val)
    	    => Parse(val);
    	
    	public static implicit operator uint(InsteonId addr)
    	    => addr.Value;
    	
    	public static implicit operator string(InsteonId addr)
    	    => addr.ToString();
    	
    	public static implicit operator byte[](InsteonId addr)
    	    => addr.Bytes;

        #endregion


        #region Parse

        public static InsteonId Parse(string s)
            => ParseValue(s);

        public static bool TryParse(string s, out InsteonId id)
        {
            try
            {
                id = ParseValue(s);
                return true;
            }
            catch
            {
                id = Zero;
                return false;
            }
        }

        static Regex ParseValueRegex { get; } = new Regex(@"(?ix) 
                ^ \s* 0? 0?
                (?<A>[0-9A-F]{2}) (?<D>[ :_,\-\.]?)
                (?<A>[0-9A-F]{2}) \k<D>
                (?<A>[0-9A-F]{2}) \s* $",
                RegexOptions.Compiled);

        static uint ParseValue(string s)
        {
            Match m = ParseValueRegex.Match(s);

            if (!m.Success)
                 throw new FormatException("Address format is invalid. Expected 6 hex digits with optional delimiter.");
           
            return GetValue(m.Groups["A"]
                .Captures.OfType<Capture>()
                .Select(x => byte.Parse(x.Value, NumberStyles.HexNumber))
                .ToArray());
        }

        #endregion


        #region Format
         
        public string ToShortString()
            => ToString("");
    	
    	public string ToString(string delimiter = "-", string byteFormat = "X2")
    	    => String.Join(delimiter, Bytes.Select(x => x.ToString(byteFormat)));
    	
    
    	#endregion
        

    	#region Object Overrides
    
    	public override string ToString()
    	    => ToString(null);
    
    	public override int GetHashCode()
    	    => Value.GetHashCode();
    	
    	public override bool Equals(object obj)
    	{
    		if (obj is InsteonId)
                return Equals((InsteonId) obj);
            if (obj is uint || obj is long || obj is ulong)
                return Equals((uint)obj);
    		if (obj is string)
                return Equals((string) obj);

    		return false;
    	}
    
    	#endregion
    

    	#region IEquatable<T> Implementation
    
    	public bool Equals(string other)
    	    => Equals(Parse(other));
        public bool Equals(InsteonId other)
            => Equals(other.Value);
        public bool Equals(uint other)
    	    => Value == other;

    	#endregion
   

    	#region IEnumerable<T> Implementation
    
    	public IEnumerator<byte> GetEnumerator()
    	    => (Bytes as IEnumerable<byte>).GetEnumerator();
 
    	IEnumerator IEnumerable.GetEnumerator()
            => (Bytes as IEnumerable).GetEnumerator();
    	
    	#endregion
    }
}
