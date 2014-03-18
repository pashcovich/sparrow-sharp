﻿using OpenTK;
using OpenTK.Graphics.ES20;
using Sparrow.Display;
using System;
using OpenTK.Graphics;
using SparrowSharp.Core;
using Sparrow.Core;
using OpenTK.Input;
using Sparrow;
using System.Diagnostics;

namespace Sparrow.Core
{
	public class DesktopViewController : GameWindow, IViewController
    {
		public delegate void OnLoadedAction(int viewWidth,int viewHeight);
		private OnLoadedAction _onLoadedAction;

		public DesktopViewController(OnLoadedAction onLoadedAction) : base()
        {
			this._onLoadedAction = onLoadedAction;

			Load += HandleLoad;

			Resize += HandleResize;

			UpdateFrame += HandleUpdateFrame;

			RenderFrame += HandleRenderFrame;

			// Run the game at 60 updates per second
			Run(60.0);
        }


		private void HandleRenderFrame (object sender, FrameEventArgs e)
        {
			//Console.WriteLine ("Number of draw calls: " + RenderSupport.NumDrawCalls);
			SwapBuffers();
        }

		private void HandleUpdateFrame (object sender, FrameEventArgs e)
        {
			SparrowSharpApp.Step(e.Time);
			// add game logic, input handling
			if (Keyboard[Key.Escape])
			{
				Exit();
			}
        }

		private void HandleResize (object sender, EventArgs e)
        {
			GL.Viewport(0, 0, Width, Height);
        }

		private void HandleLoad (object sender, EventArgs e)
        {
			// setup settings, load textures, sounds
			VSync = VSyncMode.On;
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);

			FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			_onLoadedAction(Size.Width, Size.Height);
        }

    }
}
