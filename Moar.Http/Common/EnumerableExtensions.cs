using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moar.Http
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Each<T>(this IEnumerable<T> seq, Action<T> fn)
		{
			foreach (var x in seq) fn(x);
			return seq;
		}
	}
}
