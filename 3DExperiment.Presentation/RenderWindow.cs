using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using _3DExperiment.Presentation.Extensions;
using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Mathematics.Interop;
using SharpDX.D3DCompiler;
using D3D11 = SharpDX.Direct3D11;

namespace _3DExperiment.Presentation
{
	public class RenderWindow : IDisposable
	{
		private const int Width = 800;
		private const int Height = 600;
		private RawViewportF _viewPort;

		private RenderForm _renderForm;
		private D3D11.Device _d3DDevice;
		private D3D11.DeviceContext _d3DDeviceContext;
		private SwapChain _swapChain;
		private D3D11.RenderTargetView _renderTargetView;

		private VertexPositionColor[] _vertices =
		{
			new VertexPositionColor(new RawVector3(-0.5f, 0.5f, 0.0f), Color.Purple.ToRawColor4()),
			new VertexPositionColor(new RawVector3(0.5f, 0.5f, 0.0f), Color.Green.ToRawColor4()),
			new VertexPositionColor(new RawVector3(0.0f, -0.5f, 0.0f), Color.Blue.ToRawColor4()),
			new VertexPositionColor(new RawVector3(0.0f, 0.0f, 0.5f), Color.Yellow.ToRawColor4())
		};

		private D3D11.Buffer _triangleVertexBuffer;
		private D3D11.VertexShader _vertexShader;
		private D3D11.PixelShader _pixelShader;

		//describing each element in a vertex. the 12 is because the position (3 32-bit floats = 3 4-byte floats)
		//so we need a 12 byte offset when referencing the color of each vertex
		private D3D11.InputElement[] _inputElements = 
		{
			new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
			new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0)
		};

		// the input shader from the compiled vertex shader.
		private ShaderSignature _inputSignature;
		//input layout from the array of InputElement and the input signature
		private D3D11.InputLayout _inputLayout;

		public RenderWindow()
		{
			_renderForm = new RenderForm("test window")
			{
				ClientSize = new Size(Width, Height),
				AllowUserResizing = false
			};
			_renderForm.MouseMove += RenderFormOnMouseMove;
			InitializeDeviceResources();
			InitializeShaders();
			InitializeTriangle();
		}

		private void RenderFormOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
			//throw new NotImplementedException();
		}

		private void InitializeDeviceResources()
		{
			//format is rgba with 32 but unsigned ints
			var backBufferDescription = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
			var swapChainDesc = new SwapChainDescription
			{
				ModeDescription = backBufferDescription,
				SampleDescription = new SampleDescription(1, 0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				OutputHandle = _renderForm.Handle,
				IsWindowed = true
			};

			D3D11.Device.CreateWithSwapChain(
				DriverType.Hardware, 
				D3D11.DeviceCreationFlags.None,
				swapChainDesc, 
				out _d3DDevice,
				out _swapChain);

			_d3DDeviceContext = _d3DDevice.ImmediateContext;
			using (D3D11.Texture2D backBuffer = _swapChain.GetBackBuffer<D3D11.Texture2D>(0))
			{
				_renderTargetView = new D3D11.RenderTargetView(_d3DDevice, backBuffer);
			}

			// Set viewport
			_viewPort = new RawViewportF
			{
				Height = Height,
				Width = Width,
				MaxDepth = 1.0f,
				MinDepth = 0.0f,
				X = 0,
				Y = 0
			};
			_d3DDeviceContext.Rasterizer.SetViewport(_viewPort);
		}

		private void InitializeTriangle()
		{
			_triangleVertexBuffer = D3D11.Buffer.Create<VertexPositionColor>(_d3DDevice, D3D11.BindFlags.VertexBuffer, _vertices);
		}

		private void InitializeShaders()
		{
			using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile(
				Path.Combine(Properties.Resources.ShadersFolder, "vertexShader.hlsl"), 
				"main",
				"vs_5_0", 
				ShaderFlags.Debug))
			{
				_inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
				_vertexShader = new D3D11.VertexShader(_d3DDevice, vertexShaderByteCode.Bytecode);
			}
			using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(
				Path.Combine(Properties.Resources.ShadersFolder, "pixelShader.hlsl"),
				"main", 
				"ps_5_0",
				ShaderFlags.Debug))
			{
				_pixelShader = new D3D11.PixelShader(_d3DDevice, pixelShaderByteCode);
			}
			// Set as current vertex and pixel shaders
			_d3DDeviceContext.VertexShader.Set(_vertexShader);
			_d3DDeviceContext.PixelShader.Set(_pixelShader);

			_d3DDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			_inputLayout = new D3D11.InputLayout(_d3DDevice, _inputSignature, _inputElements);
			_d3DDeviceContext.InputAssembler.InputLayout = _inputLayout;
		}

		public void Run()
		{
			RenderLoop.Run(_renderForm, RenderCallback);
		}

		private void RenderCallback()
		{
			Draw();
		}

		/// <summary>
		/// This code first sets the active render target view to the one we just created. 
		/// Then it clears the render target view (currently our back buffer) 
		/// and then swaps the back with the front buffer, making the back buffer visible.
		///  By specifying 1 as the first parameter to Present(…) we wait for vertical sync of the monitor before we present.
		///  This will limit the FPS to the update frequency of the monitor.
		/// </summary>
		private void Draw()
		{
			_d3DDeviceContext.OutputMerger.SetRenderTargets(_renderTargetView);
			_d3DDeviceContext.ClearRenderTargetView(_renderTargetView, new RawColor4(0.0f, 0.0f, 0.0f, 0.0f));

			//The first method tells the device context to use the vertex buffer holding the triangle vertex data, 
			//with the second parameter specifying the size (in bytes) for the data of each vertex. 
			//To get this size we use a nice helper method available in SharpDX.
			_d3DDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(_triangleVertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));

			//The Draw() method on the device context draws vertices.Count() many vertices from our vertex buffer.
			//The second parameter specifies the offset in our vertex buffer, 
			//by settings this to 1 for example, the first vertex would be skipped.
			_d3DDeviceContext.Draw(_vertices.Count(), 0);

			_swapChain.Present(1, PresentFlags.None);
		}

		public void Dispose()
		{
			_inputLayout.Dispose();
			_inputSignature.Dispose();
			_triangleVertexBuffer.Dispose();
			_vertexShader.Dispose();
			_pixelShader.Dispose();
			_renderTargetView.Dispose();
			_swapChain.Dispose();
			_d3DDevice.Dispose();
			_d3DDeviceContext.Dispose();
			_renderForm.Dispose();
		}
	}
}
