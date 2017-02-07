using SharpDX.Mathematics.Interop;
using System.Runtime.InteropServices;

namespace _3DExperiment.Presentation
{
	/// <summary>
	/// Struct for a vertex with both position and color.
	/// 
	/// LayoutKind.Sequential: 
	/// This make sure that the values are laid out in memory in the same order as they are specified in the struct. 
	/// This is important as we later need to tell this to the GPU.
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential)]
	public struct VertexPositionColor
	{
		public readonly RawVector3 Position;
		public readonly RawColor4 Color;

		public VertexPositionColor(RawVector3 position, RawColor4 color)
		{
			Position = position;
			Color = color;
		}
	}
}