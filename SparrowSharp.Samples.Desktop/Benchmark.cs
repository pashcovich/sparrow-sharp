using System;
using System.Collections.Generic;
using Sparrow.Display;
using Sparrow;
using OpenTK.Graphics.ES20;
using Sparrow.Textures;
using SparrowSharp.Display;
using Sparrow.ResourceLoading;
using Sparrow.Core;
using SparrowSharp.Filters;

namespace Sparrow.Samples
{
	public class Benchmark : Sprite
	{
		private Sprite _container;
		private int _frameCount = 0;
		private float _elapsed = 0;
		private bool _started = false;
		private int _failCount = 0;
		private int _waitFrames = 0;
		private Texture[] textures;

		public Benchmark()
		{
			GLTexture star = SimpleTextureLoader.LoadLocalImage("../../bigstar.png");
			GLTexture bird = SimpleTextureLoader.LoadLocalImage("../../benchmark_object.png");
			GLTexture bigstar = SimpleTextureLoader.LoadLocalImage("../../bigstar.png");
			textures = new Texture[] { bird, bigstar, star };

			// the container will hold all test objects
			_container = new Sprite();
			AddChild(_container);

			EnterFrame += EnterFrameHandler;
			AddedToStage += AddedToStageHandler;
		}

		private void AddTestObjects(int numObjects)
		{
			int border = 15;

			Random r = new Random();
			for (int i = 0; i < numObjects; ++i)
			{   
				Image egg = new Image(textures[0]);
                if (i < 5) {
					//ColorMatrixFilter cf = new ColorMatrixFilter ();
                    egg.Filter = new BlurFilter (4, 1.1f);
                    egg.Filter.Cache ();
				}
				//MovieClip egg = new MovieClip (textures, 3);
				//SP.DefaultJuggler.Add (egg);
				egg.X = r.Next(border, (int)Stage.Width - border);
				egg.Y = r.Next(border, (int)Stage.Height - border);
				egg.Rotation = (float)(r.Next(0, 100) / 100.0f * Math.PI);
				_container.AddChild(egg);
			}
		}

		private void BenchmarkComplete()
		{
			Console.WriteLine("benchmark complete!");
			Console.WriteLine("number of objects: " + _container.NumChildren);

			_started = false;
			_container.RemoveAllChildren();
		}

		private void AddedToStageHandler(DisplayObject target, DisplayObject currentTarget)
		{
			_started = true;
			_waitFrames = 3;
			AddTestObjects(4000);
		}

		private void EnterFrameHandler(DisplayObject target, DisplayObject currentTarget, float passedTime)
		{
			if (!_started)
				return;

			_elapsed += passedTime / 1000;
			++_frameCount;
            /*
            if (_frameCount % _waitFrames == 0)
            {
                float targetFPS = 60;
                float realFPS = _waitFrames / _elapsed;
                //Console.WriteLine ("FPS: " + realFPS);
                if (realFPS >= targetFPS)
                {
                    int numObjects = _failCount != 0 ? 5 : 25;
                    AddTestObjects(numObjects);
                    _failCount = 0;
                }
                else
                {
                    ++_failCount;

                    if (_failCount > 15)
                        _waitFrames = 5; // slow down creation process to be more exact
                    if (_failCount > 20)
                        _waitFrames = 10;
                    if (_failCount == 25)
                        BenchmarkComplete(); // target fps not reached for a while
                }

                _elapsed = _frameCount = 0;
            }
*/
			for (int i = 0; i < _container.NumChildren; i++)
			{
				DisplayObject child = _container.GetChild(i);
				child.Rotation += 0.05f;    
			} 
		}
	}
}