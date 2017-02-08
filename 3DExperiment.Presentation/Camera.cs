using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Mathematics.Interop;

namespace _3DExperiment.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	public class Camera
	{
		private RawVector3 _position;
		private RawVector3 _rotation;

		public Camera(float x = 0.0f, float y = 0.0f, float z = 0.0f, 
			float xRotation = 0.0f, float yRotation = 0.0f, float zRotation = 0.0f)
		{
			_position = new RawVector3(x,y,z);
			_rotation = new RawVector3(xRotation, yRotation, zRotation);
		}

		public Camera(RawVector3 position, RawVector3 rotation)
		{
			_position = position;
			_rotation = rotation;
		}

		public RawVector3 Position
		{
			get { return _position; }
		}

		public RawVector3 Rotation
		{
			get {return _rotation; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		public void MoveCameraTo(RawVector3 position)
		{
			_position = new RawVector3(position.X, position.Y, position.Z);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rotation"></param>
		public void RotateCameraTo(RawVector3 rotation)
		{
			_rotation = new RawVector3(rotation.X, rotation.Y, rotation.Z);
		}

		/// <summary>
		/// 
		/// </summary>
		public void RenderMatrix()
		{
			var up = new RawVector3(0.0f, 1.0f, 0.0f);
		}
	}
}
