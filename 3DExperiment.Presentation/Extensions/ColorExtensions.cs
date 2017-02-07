using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace _3DExperiment.Presentation.Extensions
{
	public static class ColorExtensions
	{
		public static RawColor4 ToRawColor4(this Color c)
		{
			return new RawColor4(NormalizeColorComponent(c.R), NormalizeColorComponent(c.G), NormalizeColorComponent(c.B), NormalizeColorComponent(c.A));
		}

		private static float NormalizeColorComponent(byte val)
		{
			return (float) val/255;
		}
	}
}
