﻿using SparrowSharp.Core;
using Sparrow.Geom;
using Sparrow.Display;
using System.Collections.Generic;
using System;
using Sparrow.Textures;
using Sparrow.Utils;
using Sparrow.Core;

namespace SparrowSharp.Fonts
{
    public class TextField : DisplayObjectContainer
    {
        private static readonly Dictionary<string, BitmapFont> bitmapFonts = new Dictionary<string, BitmapFont>();
        // its actually read from the font XML
        public static readonly string MiniFontName = "mini";
        public string _text;

        /// <summary>
        /// The displayed text.
        /// </summary>
        public string Text
        {
            set
            {
                if (value != _text)
                {
                    _text = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _text;
            }
        }

        private string _fontName;

        private string RegisterBitmapFont(BitmapFont bitmapFont, string fontName = null)
        {
            if (fontName == null)
            {
                fontName = bitmapFont.Name;
            }
            bitmapFonts[fontName] = bitmapFont;
            if (_fontName != null)
            {
                _isRenderedText = !bitmapFonts.ContainsKey(_fontName); 
            }
            return fontName;
        }

        /// <summary>
        /// The name of the font.
        /// </summary>
        public string FontName
        {
            set
            {
                if (value != _fontName)
                {
                    if (value == MiniFontName && !bitmapFonts.ContainsKey(value))
                    {
                        RegisterBitmapFont(new BitmapFont());
                    }
                    _fontName = value;
                    _requiresRedraw = true;        
                    _isRenderedText = !bitmapFonts.ContainsKey(_fontName);
                }
            }
            get
            {
                return _fontName;
            }
        }

        private float _fontSize;

        /// <summary>
        ///  The size of the font. For bitmap fonts, use 'NativeFontSize' for the original size.
        /// </summary>
        public float FontSize
        {
            set
            {
                if (value != _fontSize)
                {
                    _fontSize = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _fontSize;
            }
        }

        private HAlign _hAlign;

        /// <summary>
        /// The horizontal alignment of the text.
        /// </summary>
        public HAlign HAlign
        {
            set
            {
                if (value != _hAlign)
                {
                    _hAlign = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _hAlign;
            }
        }

        private VAlign _vAlign;

        /// <summary>
        /// The vertical alignment of the text.
        /// </summary>
        public VAlign VAlign
        {
            set
            {
                if (value != _vAlign)
                {
                    _vAlign = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _vAlign;
            }
        }

        private Sprite _border;

        /// <summary>
        /// Allows displaying a border around the edges of the text field. Useful for visual debugging.
        /// </summary>
        public bool Border
        {
            set
            {
                if (value && _border == null)
                {
                    _border = new Sprite();

                    for (int i = 0; i < 4; ++i)
                    {
                        _border.AddChild(new Quad(1.0f, 1.0f));
                    }
                    AddChild(_border); 
                    UpdateBorder();
                }
                else if (!value && _border != null)
                {
                    _border.RemoveFromParent();
                    _border = null;
                }
            }
            get
            {
                return (_border == null);
            }
        }

        private uint _color;

        /// <summary>
        /// The color of the text.
        /// </summary>
        public uint Color
        {
            set
            {
                if (value != _color)
                {
                    _color = value;
                    _requiresRedraw = true;
                    UpdateBorder();
                }
            }
            get
            {
                return _color;
            }
        }

        /// <summary>
        /// The bounds of the actual characters inside the text field.
        /// </summary>
        public Rectangle TextBounds
        {
            get
            {
                if (_requiresRedraw)
                {
                    Redraw();
                }
                if (_textBounds == null)
                {
                    _textBounds = _contents.BoundsInSpace(_contents);
                }
                return _textBounds.Copy();
            }
        }

        private bool _kerning;

        /// <summary>
        /// Allows using kerning information with a bitmap font (where available). Default is YES.
        /// </summary>
        public bool Kerning
        {
            set
            {
                if (value != _kerning)
                {
                    _kerning = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _kerning;
            }
        }

        private bool _autoScale;

        /// <summary>
        /// Indicates whether the font size is scaled down so that the complete text fits into the
        /// text field. Default is false.
        /// </summary>
        public bool AutoScale
        {
            set
            {
                if (_autoScale != value)
                {
                    _autoScale = value;
                    _requiresRedraw = true;
                }
            }
            get
            {
                return _kerning;
            }
        }

        private bool _requiresRedraw;
        private bool _isRenderedText;
        private QuadBatch _contents;
        private Rectangle _textBounds;
        private Quad _hitArea;

        public TextField(float width, float height, string text = "", string fontName = "mini", float fontSize = 14, uint color = 0x0)
        {
            _text = text;
            _fontSize = fontSize;
            _color = color;
            _hAlign = HAlign.Center;
            _vAlign = VAlign.Center;
            _autoScale = false;
            _kerning = true;
            _requiresRedraw = true;
            FontName = fontName;

            _hitArea = new Quad(width, height);
            _hitArea.Alpha = 0.0f;
            AddChild(_hitArea);

            _contents = new QuadBatch();
            _contents.Touchable = false;
            AddChild(_contents);

            //[self addEventListener:@selector(onFlatten:) atObject:self forType:SPEventTypeFlatten];
        }

        private void UpdateBorder()
        {
            if (_border == null)
            {
                return;
            }

            float width = _hitArea.Width;
            float height = _hitArea.Height;

            Quad topLine = (Quad)_border.GetChild(0);
            Quad rightLine = (Quad)_border.GetChild(1);
            Quad bottomLine = (Quad)_border.GetChild(2);
            Quad leftLine = (Quad)_border.GetChild(3);

            topLine.Width = width;
            topLine.Height = 1;
            bottomLine.Width = width;
            bottomLine.Height = 1;
            leftLine.Width = 1;
            leftLine.Height = height;
            rightLine.Width = 1;
            rightLine.Height = height;
            rightLine.X = width - 1;
            bottomLine.Y = height - 1;
            topLine.Color = rightLine.Color = bottomLine.Color = leftLine.Color = _color;

            _border.Flatten();
        }

        private void Redraw()
        {
            if (_requiresRedraw)
            {
                _contents.Reset();
                if (_isRenderedText)
                {
                    CreateRenderedContents();
                }
                else
                {
                    CreateComposedContents();
                }
                _requiresRedraw = false;
            }
        }

        private void CreateRenderedContents()
        {
            throw new Exception("Can not render with non registered font " + _fontName);
            // TODO: render text with built in font
            /*
            float width = _hitArea.Width;
            float height = _hitArea.Height; 

            float fontSize = _fontSize == NativeFontSize ? DefaultFontSize : _fontSize;
         
            CGSize textSize;

             if (_autoScale)
            {
                CGSize maxSize = CGSizeMake(width, float.MaxValue);
                fontSize += 1.0f;

                do
                {
                    fontSize -= 1.0f;
                    textSize = _text sizeWithFont:[UIFont fontWithName:_fontName size:fontSize]
                        constrainedToSize:maxSize lineBreakMode:lbm];
                } while (textSize.height > height);
            }
            else
            {
                textSize = [_text sizeWithFont:[UIFont fontWithName:_fontName size:fontSize]
                    constrainedToSize:CGSizeMake(width, height) lineBreakMode:lbm];
            }

            float xOffset = 0;
            if (_hAlign == HAlign.Center)
                xOffset = (width - textSize.width) / 2.0f;
            else if (_hAlign == HAlign.Right)
                xOffset = width - textSize.width;

            float yOffset = 0;
            if (_vAlign == VAlign.Center)
                yOffset = (height - textSize.height) / 2.0f;
            else if (_vAlign == VAlign.Bottom)
                yOffset = height - textSize.height;

            if (_textBounds == null)
                _textBounds = new Rectangle(xOffset, yOffset, textSize.width, textSize.height)

            RenderTexture texture = new RenderTexture(width, height);
            texture.DrawBundled(delegate
            {
                float red = ColorUtil.GetR(_color) / 255.0f;
                float green = ColorUtil.GetG(_color) / 255.0f;
                float blue = ColorUtil.GetB(_color) / 255.0f;

                CGContextSetRGBFillColor(context, red, green, blue, 1.0f);

                [_text drawInRect:CGRectMake(0, yOffset, width, height)
                    withFont:[UIFont fontWithName:_fontName size:fontSize] 
                    lineBreakMode:lbm alignment:(NSTextAlignment)_hAlign];
            });

            Image image = new Image(texture);
           
            _contents.AddQuad(image);*/
        }

        private void CreateComposedContents()
        {
            BitmapFont bitmapFont;
            bitmapFonts.TryGetValue(_fontName, out bitmapFont);
            if (bitmapFont == null)
            {
                throw new InvalidOperationException("bitmap font " + _fontName + " not registered!");
            }

            bitmapFont.FillQuadBatch(_contents, _hitArea.Width, _hitArea.Height, _text, 
                _fontSize, _color, _hAlign, _vAlign, _autoScale, _kerning);

            _textBounds = null; // will be created on demand
        }

        override public void Render(RenderSupport support)
        {
            if (_requiresRedraw)
            {
                Redraw();
            }
            base.Render(support);
        }

        override public Rectangle BoundsInSpace(DisplayObject targetSpace)
        {
            return _hitArea.BoundsInSpace(targetSpace);
        }

        override public float Width
        {
            set
            {
                // other than in SPDisplayObject, changing the size of the object should not change the scaling;
                // changing the size should just make the texture bigger/smaller,
                // keeping the size of the text/font unchanged. (this applies to setHeight:, as well.)
                _hitArea.Width = value;
                _requiresRedraw = true;
                UpdateBorder();
            }
        }

        override public float Height
        {
            set
            {
                _hitArea.Height = value;
                _requiresRedraw = true;
                UpdateBorder();
            }

        }
    }
}

