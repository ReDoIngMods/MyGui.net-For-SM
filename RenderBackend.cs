using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using static MyGui.net.Util;

namespace MyGui.net
{
	public static class RenderBackend
	{

		#region Caches
		public static Dictionary<string, SKImage> _skinAtlasCache = new();
		public static Dictionary<string, SKTypeface> _fontCache = new();

		public static Dictionary<string, MyGuiResource> _allResources = new();
		public static Dictionary<string, MyGuiFontData> _allFonts = new();

		public static Dictionary<string, MyGuiResource> AllResources => _allResources;
		public static Dictionary<string, MyGuiFontData> AllFonts => _allFonts;

		public static MyGuiResource _nullSkinResource = new MyGuiResource();
		public static MyGuiResource NullSkinResource => _nullSkinResource;

		public static Dictionary<MyGuiWidgetData, WidgetHighlightType> _renderWidgetHighligths = new();
		public static Dictionary<string, SKColor> _widgetTypeColors = new(){
			{ "Button", new SKColor(0xDF7F00) },
			{ "EditBox", new SKColor(0xEFEF00) },
			{ "TextBox", new SKColor(0xFFFF00) },
			{ "ItemBox", new SKColor(0x00F7F7) },
			{ "ImageBox", new SKColor(0x0000F9) },
			{ "Widget", new SKColor(0x01F201) },
			{ "ScrollView", new SKColor(0xFF2F00) }
		};
		#endregion

		public struct WidgetHighlightType
		{
			public SKPoint position;
			public SKColor highlightColor;
			public SKPaintStyle style;
			public float width;
			public bool ignoreDrawOrder; //TODO

			public WidgetHighlightType(SKPoint position, SKColor highlightColor, SKPaintStyle? style = null, float width = 7, bool ignoreDrawOrder = true)
			{
				this.position = position;
				this.highlightColor = highlightColor;
				this.style = style ?? SKPaintStyle.Stroke;
				this.width = width;
				this.ignoreDrawOrder = ignoreDrawOrder;
			}
		}

		/// <summary>
		/// Repositions and adjsuts Widgets to be rendered by calling RenderWidget.
		/// </summary>
		/// <param name="canvas">Canvas to render onto</param>
		/// <param name="widget">Witget to render</param>
		/// <param name="parentOffset">Parent tree offset</param>
		/// <param name="parent">Parent Widget</param>
		/// <param name="widgetSecondaryData">Widget, from which other data should be pulled</param>
		/// <param name="adjustToParent">Enabled automatically when rendering a ResourceLayout</param>
		/// <param name="oldSize">Original size, used to render ResourceLayout skins</param>
		/// <param name="widgetTertiaryData">Widget, from which other, less important, data should be pulled</param>
		/// <param name="forceDebug">Forces debug drawing of slices and all</param>
		public static void DrawWidget(SKCanvas canvas, MyGuiWidgetData widget, SKPoint parentOffset, MyGuiWidgetData? parent = null, MyGuiWidgetData? widgetSecondaryData = null, bool adjustToParent = false, Point? oldSize = null, MyGuiWidgetData? widgetTertiaryData = null, bool forceDebug = false)
		{
			Point oldSizeParam = new(widget.size.X, widget.size.Y);

			if ((System.Object.ReferenceEquals(parent, widgetSecondaryData) || widget.name == "Root") && widgetSecondaryData != null) //This fixes the scaling issues that i had before, sets the scale 
			{
				widget.size = new(widgetSecondaryData.size.X, widgetSecondaryData.size.Y);
			}

			// Calculate widget position relative to parent
			SKPoint widgetPosition = new(parentOffset.X + widget.position.X, parentOffset.Y + widget.position.Y);

			// Create rectangle for this widget
			SKRect rect = new(widgetPosition.X, widgetPosition.Y, widgetPosition.X + widget.size.X, widgetPosition.Y + widget.size.Y);

			if (adjustToParent && parent != widgetSecondaryData)
			{
				rect = GetWidgetOffset(widget, parent, new((int)widgetPosition.X, (int)widgetPosition.Y), oldSize.Value);

				widget.position = new Point((int)rect.Location.X, (int)rect.Location.Y);
				widget.size = new Point((int)rect.Size.Width, (int)rect.Size.Height);

				widgetPosition = new(widget.position.X, widget.position.Y);

				/*var textPaint = new SKPaint
				{
					Color = SKColors.White,
					TextSize = 5,
					IsAntialias = false,
					StrokeWidth = 1
				};
				canvas.DrawText(parent.name ?? "null", rect.Left + 5, rect.Top + Util.rand.Next(0, 20), textPaint);*/
			}


			if (_allResources.TryGetValue(widget.skin, out var val) && val.resourceLayout != null)
			{
				//Debug.WriteLine(ExportLayoutToXmlString(val.resourceLayout, new(1, 1), true));
				var layoutCopy = DeepCopy(val.resourceLayout);

				var subWidget = layoutCopy[0];
				subWidget.position = new(0, 0);

				DrawWidget(canvas, subWidget, widgetPosition, widget, widget, true, new Point(subWidget.size.X, subWidget.size.Y), widgetSecondaryData != null ? widgetSecondaryData : widget, forceDebug);
				//}
				//return;
			}
			string skinPath = widget.skin != null && _allResources.ContainsKey(widget.skin) ? _allResources[widget.skin]?.path : "";
			skinPath ??= "";


			//Debug.WriteLine(skinPath);
			if (!_skinAtlasCache.ContainsKey(skinPath))
			{
				if (widget.skin != null && skinPath != null && skinPath != "")
				{
					//Debug.WriteLine($"caching skin from dir {skinPath}");
					SKBitmap? cachedBitmap = SKBitmap.Decode(skinPath);
					if (cachedBitmap != null)
					{
						_skinAtlasCache[skinPath] = SKImage.FromBitmap(cachedBitmap);
					}
					else
					{
						_skinAtlasCache[skinPath] = null;
					}
				}
				else
				{
					_skinAtlasCache[""] = SKImage.FromBitmap(LoadBitmap(_nullSkinResource.path));
				}
			}

			// Save canvas state for clipping
			var saveBeforeAll = canvas.Save();

			// Apply clipping for the widget's bounds
			canvas.ClipRect(rect);

			// Generate a random color
			//var color = new SKColor((byte)Util.rand.Next(256), (byte)Util.rand.Next(256), (byte)Util.rand.Next(256));

			// Draw rectangle for the widget
			/*var paint = new SKPaint
			{
				//Color = color,
				Style = SKPaintStyle.Fill,
				IsAntialias = false
			};
			canvas.DrawRect(rect, paint);*/
			if (skinPath != null && skinPath != "" && _skinAtlasCache.ContainsKey(skinPath))
			{
				RenderWidget(canvas, _skinAtlasCache[skinPath], _allResources[widget.skin], rect, null, widget, widgetSecondaryData, widgetTertiaryData, forceDebug);
			}
			else
			{
				RenderWidget(canvas, _skinAtlasCache[""], _allResources.TryGetValue(widget.skin, out val) ? val : _nullSkinResource, rect, _widgetTypeColors.ContainsKey(widget.type) ? _widgetTypeColors[widget.type] : null, widget, widgetSecondaryData, widgetTertiaryData, forceDebug);
				//canvas.DrawRect(rect, paint);
			}

			// Draw the widget's name (optional)
			if (!adjustToParent)
			{
				if (Settings.Default.RenderWidgetNames && !string.IsNullOrEmpty(widget.name))
				{
					var textPaint = new SKPaint
					{
						Color = SKColors.White,
						TextSize = 16,
						IsAntialias = false,
						Style = SKPaintStyle.StrokeAndFill,
						StrokeWidth = 1
					};
					canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaint);
					var textPaintStroke = new SKPaint
					{
						Color = SKColors.Black,
						TextSize = 16,
						IsAntialias = true
					};
					canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaintStroke);
				}

				// Temporarily ignore clipping to draw selection box
				if (widget == Form1._currentSelectedWidget)
				{
					_renderWidgetHighligths[widget] = new WidgetHighlightType(widgetPosition, SKColors.Green.WithAlpha(128));
				}
				else
				{
					//WIP!!!
					SKRect localRect = new SKRect(widget.position.X, widget.position.Y, widget.position.X + widget.size.X, widget.position.Y + widget.size.Y);
					SKRect parentRect = parent != null ? new(1, 1, parent.size.X - 2, parent.size.Y - 2) : new(1, 1, Form1.ProjectSize.Width - 2, Form1.ProjectSize.Height - 2);
					if (!Util.RectsOverlap(localRect, parentRect))
					{
						_renderWidgetHighligths.TryAdd(widget, new WidgetHighlightType(widgetPosition, SKColors.Red.WithAlpha(192)));
					}
				}
			}

			// Recursively draw child widgets
			foreach (var child in widget.children)
			{
				var widgetBounds = new SKRect(widgetPosition.X - 1, widgetPosition.Y - 1,
							  widgetPosition.X + widget.size.X - 2, widgetPosition.Y + widget.size.Y - 2);
				if (canvas.LocalClipBounds.IntersectsWith(widgetBounds)) DrawWidget(canvas, child, widgetPosition, widget, widgetSecondaryData, adjustToParent, oldSizeParam, widgetTertiaryData);
			}

			// Restore the canvas to its previous state (removes clipping for this widget)
			canvas.RestoreToCount(saveBeforeAll);
		}

		public static SKRect GetWidgetOffset(MyGuiWidgetData current, MyGuiWidgetData parent, Point currentPosition, Point parentOriginalSize)
		{
			int x = currentPosition.X, y = currentPosition.Y, width = current.size.X, height = current.size.Y;
			int widthOffset = parentOriginalSize.X - (current.position.X + current.size.X);
			int heightOffset = parentOriginalSize.Y - (current.position.Y + current.size.Y);
			switch (current.align)
			{
				case "[DEFAULT]":
				case "Default":
				case "Left Top":
				case null:
					break;

				case "Stretch":
				case "HStretch VStretch":
					width = parent.size.X - current.position.X - widthOffset;//parent.size.X - current.position.X - (parentOriginalSize.X - (current.position.X + current.size.X));
					height = parent.size.Y - current.position.Y - heightOffset;//parent.size.Y - current.position.Y - (parentOriginalSize.Y - (current.position.Y + current.size.Y));
					break;

				case "Center":
				case "HCenter VCenter":
					x = (parent.size.X - current.size.X) / 2 + currentPosition.X - current.position.X;
					y = (parent.size.Y - current.size.Y) / 2 + currentPosition.Y - current.position.Y;
					break;

				case "Left Bottom":
					y += parent.size.Y - current.size.Y - heightOffset - current.position.Y;
					break;

				case "Left VStretch":
					height = parent.size.Y - current.position.Y - heightOffset;
					break;

				case "Left VCenter":
					y = (parent.size.Y - current.size.Y) / 2 + currentPosition.Y - current.position.Y;
					break;

				case "Right Top":
					x += parent.size.X - current.size.X - widthOffset - current.position.X;
					break;

				case "Right Bottom":
					x += parent.size.X - current.size.X - widthOffset - current.position.X;
					y += parent.size.Y - current.size.Y - heightOffset - current.position.Y;
					break;

				case "Right VStretch":
					x += parent.size.X - current.size.X - widthOffset - current.position.X;
					height = parent.size.Y - current.position.Y - heightOffset;
					break;

				case "Right VCenter":
					x += parent.size.X - current.size.X - widthOffset - current.position.X;
					y = (parent.size.Y - current.size.Y) / 2 + currentPosition.Y - current.position.Y;
					break;

				case "HStretch Top":
					width = parent.size.X - current.position.X - widthOffset;
					break;

				case "HCenter Top":
					x = (parent.size.X - current.size.X) / 2 + currentPosition.X - current.position.X;
					break;

				case "HStretch Bottom":
					y += parent.size.Y - current.size.Y - heightOffset - current.position.Y;
					width = parent.size.X - current.position.X - widthOffset;
					break;

				case "HCenter Bottom":
					x = (parent.size.X - current.size.X) / 2 + currentPosition.X - current.position.X;
					y += parent.size.Y - current.size.Y - heightOffset - current.position.Y;
					break;

				default:
					break;
					//Debug.WriteLine($"Unknown align type: {align}");
			}
			return new SKRect(x, y, x + width, y + height);
		}

		static SKPaint _baseFontPaint = new SKPaint { };

		public static void RenderWidget(SKCanvas canvas, SKImage atlasImage, MyGuiResource resource, SKRect clientRect, SKColor? drawColor = null, MyGuiWidgetData? widget = null, MyGuiWidgetData? widgetSecondaryData = null, MyGuiWidgetData? widgetTertiaryData = null, bool forceDebug = false)
		{
			widgetSecondaryData ??= widget;
			widgetTertiaryData ??= widgetSecondaryData;
			if (resource == null || resource.basisSkins == null)
			{
				return; // Nothing to render if essential data is missing
			}

			//Debug.WriteLine($"Rendering widget with skin {resource.name}.");

			// Iterate through skins in reverse order
			var renderedBasisSkins = resource.basisSkins ?? new();
			if (Settings.Default.RenderInvisibleWidgets && (resource.path ?? "") == "" && (_allResources[widgetSecondaryData.skin].resourceLayout == null || widget == _allResources[widgetSecondaryData.skin].resourceLayout[0]))
			{
				RenderWidget(canvas, _skinAtlasCache[""], _nullSkinResource, clientRect, _widgetTypeColors.ContainsKey(widgetSecondaryData.type) ? _widgetTypeColors[widgetSecondaryData.type] : null, widget, widgetSecondaryData, null, forceDebug); //Ik, it is kinda terrible but it works, k?
			}
			for (int i = 0; i < renderedBasisSkins.Count; i++)
			{
				var skin = renderedBasisSkins[i];
				if (skin.type == "EffectSkin")
				{
					continue; // Skip rendering for invalid skins
				}


				var tileOffset = Util.GetWidgetPosAndSize(false, skin.offset, new(1, 1));

				// Find the normal state of the skin
				var normalState = widgetTertiaryData.properties.TryGetValue("StateSelected", out var val) && val == "true" && skin.states.FirstOrDefault(state => state.name == "pushed") != null ? skin.states.FirstOrDefault(state => state.name == "pushed") : skin.states.FirstOrDefault(state => state.name == "normal");
				if (normalState == null)
				{
					normalState = new() { name = "normal", offset = skin.offset };
					//continue; // Skip if no normal state is found
				}
				var correctOffset = normalState.offset;
				if (normalState.offset == null)
				{
					correctOffset = skin.offset;
				}

				//Debug.WriteLine($"Rendering skin with alignment: {skin.align}");

				var posSize = Util.GetWidgetPosAndSize(false, correctOffset, new(1, 1));
				var tileRect = new SKRect(
					posSize.Item1.X,
					posSize.Item1.Y,
					posSize.Item1.X + Math.Max(posSize.Item2.X, 1), //Size must be atleast 1 px (As SkiaSharp doesnt render anything if the size is 0)
					posSize.Item1.Y + Math.Max(posSize.Item2.Y, 1)
				);

				var maxSizePoint = Util.GetWidgetPos(false, resource.tileSize, new(1, 1));

				// Calculate destination rectangle based on alignment
				var destRect = GetAlignedRectangle(skin.align, clientRect, new(tileOffset.Item2.X, tileOffset.Item2.Y),
					new SKRect(
					tileOffset.Item1.X,
					tileOffset.Item1.Y,
					tileOffset.Item1.X + tileOffset.Item2.X,
					tileOffset.Item1.Y + tileOffset.Item2.Y
				), new(maxSizePoint.X, maxSizePoint.Y));

				var drawPaint = new SKPaint();

				if (widget != null)
				{
					if (skin.type == "SimpleText" || skin.type == "EditText")
					{
						if (widgetTertiaryData.properties.ContainsKey("Caption"))
						{
							int beforeTextClip = canvas.Save();
							canvas.ClipRect(destRect);

							var fontData = widgetTertiaryData.properties.TryGetValue("FontName", out string value) && _allFonts.ContainsKey(value) ? _allFonts[value] : _allFonts["DeJaVuSans"];
							string fontPath = Path.Combine(Settings.Default.ScrapMechanicPath, "Data\\Gui\\Fonts", fontData.source);
							if (!_fontCache.ContainsKey(fontPath))
							{
								_fontCache[fontPath] = SKTypeface.FromFile(fontPath);
							}

							Color? textColor;
							if (widgetTertiaryData.properties.ContainsKey("TextColour"))
							{
								textColor = ParseColorFromString(widgetTertiaryData.properties["TextColour"]);
							}
							else if (!string.IsNullOrEmpty(normalState.color))
							{
								textColor = ParseColorFromString(normalState.color);
							}
							else
							{
								textColor = Color.White;
							}

							var fontCurrentSizeNoScaling = widgetTertiaryData.properties.ContainsKey("FontHeight") && ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) != double.NaN ? (float)ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) : (float)fontData.size;

							_baseFontPaint.Color = new(textColor.Value.R, textColor.Value.G, textColor.Value.B);
							_baseFontPaint.TextSize = widgetTertiaryData.properties.ContainsKey("FontHeight") && ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) != double.NaN ? (float)ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) : (float)fontData.size * ((Settings.Default.ReferenceResolution + 1) * 1.25f);
							_baseFontPaint.IsAntialias = Settings.Default.UseViewportFontAntiAliasing;
							_baseFontPaint.Typeface = _fontCache[fontPath];
							_baseFontPaint.FilterQuality = (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

							_baseFontPaint.TextSize = _baseFontPaint.TextSize * 0.8f;

							string captionText = Util.ReplaceLanguageTagsInString(widgetTertiaryData.properties["Caption"], Settings.Default.ReferenceLanguage, Form1.ScrapMechanicPath);

							float offsetX = 0;
							float offsetY = 0;

							// TODO: Write better system for this, reason: doesnt support /n, uses outdates width calculations
							float textWidth = 0;
							foreach (char character in fontData.allowedChars == "ALL CHARACTERS" ? captionText : ReplaceInvalidChars(captionText, fontData.allowedChars))
							{
								textWidth += _baseFontPaint.MeasureText(character.ToString()) - (0.025f * _baseFontPaint.TextSize) + ((float)(fontData.letterSpacing ?? 0) / (float)fontData.size * _baseFontPaint.TextSize);
							}

							float widgetWidth = destRect.Right - destRect.Left;
							float widgetHeight = destRect.Bottom - destRect.Top;

							switch (widgetTertiaryData.properties.TryGetValue("TextAlign", out var alignment) ? alignment : (widget.properties.TryGetValue("TextAlign", out var alignmentDefault) ? alignmentDefault : "") )
							{
								case "HCenter VCenter":
								case "Center":
									offsetX = (widgetWidth / 2) - (textWidth / 2);
									offsetY = (widgetHeight / 2) - (_baseFontPaint.TextSize / 2) - _baseFontPaint.TextSize * 0.15f;
									break;
								case "Left Bottom":
									offsetY = destRect.Bottom - destRect.Top - _baseFontPaint.TextSize * 1.25f;
									break;
								case "Left VCenter":
									offsetY = (widgetHeight / 2) - (_baseFontPaint.TextSize / 2) - _baseFontPaint.TextSize * 0.15f;
									break;
								case "Right Top":
									offsetX = widgetWidth - textWidth;
									break;
								case "Right Bottom":
									offsetX = widgetWidth - textWidth;
									offsetY = destRect.Bottom - destRect.Top - _baseFontPaint.TextSize * 1.25f;
									break;
								case "Right VCenter":
									offsetX = widgetWidth - textWidth;
									offsetY = (widgetHeight / 2) - (_baseFontPaint.TextSize / 2) - _baseFontPaint.TextSize * 0.15f;
									break;
								case "HCenter Top":
									offsetX = (widgetWidth / 2) - (textWidth / 2);
									break;
								case "HCenter Bottom":
									offsetX = (widgetWidth / 2) - (textWidth / 2);
									offsetY = destRect.Bottom - destRect.Top - _baseFontPaint.TextSize * 1.25f;
									break;
								default:
								case "Left Top":
									break;
							}
							//End of the system improvement thing

							float spacingX = destRect.Left + offsetX;
							float spacingY = destRect.Top + _baseFontPaint.TextSize + offsetY;

							int skipNext = 0;
							bool allowsAllChars = fontData.allowedChars == "ALL CHARACTERS";
							SKColor runningTextColor = textColor.GetValueOrDefault(Color.White).ToSKColor();

							char prevChar = '\0';

							for (int j = 0; j < captionText.Length; j++)
							{
								char character = captionText[j];
								float actualFontLetterSpacing = (float)(fontData.letterSpacing ?? 0);
								float fontDefaultSize = (float)fontData.size * (Settings.Default.ReferenceResolution + 1);

								// Replace or validate the character
								string actualChar = allowsAllChars
													  ? character.ToString()
													  : ReplaceInvalidChars(character.ToString(), fontData.allowedChars);

								// Measure this character
								float charWidth = _baseFontPaint.MeasureText(actualChar);

								// Calculate extra spacing (letter spacing multiplier)
								float extraSpacing = actualFontLetterSpacing * (Settings.Default.ReferenceResolution + 1) * (fontCurrentSizeNoScaling / (float)fontData.size) * 0.75f;

								// Compute kerning adjustment between previous char and current char.
								// This is done by measuring the pair’s width and subtracting the sum of individual widths.
								float kerningAdjustment = 0;
								if (prevChar != '\0')
								{
									// Note: If your captionText might contain surrogate pairs, you’d want to be careful here.
									string pair = prevChar.ToString() + actualChar;
									float pairWidth = _baseFontPaint.MeasureText(pair);
									float prevWidth = _baseFontPaint.MeasureText(prevChar.ToString());
									kerningAdjustment = pairWidth - (prevWidth + charWidth);
								}

								// Total spacing for this glyph: the measured width, plus letter spacing and kerning
								float fontSpacing = charWidth + extraSpacing + kerningAdjustment;

								// Handle special cases: newline (represented as "\n") and color changes as before.
								if (skipNext > 0)
								{
									skipNext--;
									prevChar = character;
									continue;
								}

								if (character == '\\' && j + 1 < captionText.Length && captionText[j + 1] == 'n')
								{
									_baseFontPaint.GetFontMetrics(out SKFontMetrics metrics);
									float MAJORSKILLISSUE = widgetTertiaryData.properties.ContainsKey("FontHeight") && ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) != double.NaN ? (float)ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) :
										metrics.CapHeight + metrics.Descent + 3; //Default size newline stuff, i need a better name for this var, i literally wrote this at 4 AM or something
									spacingX = destRect.Left + offsetX; // Reset X position

									//spacingY += metrics.Descent - metrics.Ascent; // Move down by line height, works really well
									//spacingY += metrics.Descent - metrics.Ascent + (_baseFontPaint.TextSize / fontDefaultSize); //Only works for SM_SearchText for some reason

									spacingY += MAJORSKILLISSUE;
									/*if (!captionText.EndsWith("cantorada"))
									{
										captionText += " :" + _baseFontPaint.TextSize.ToString() +" cantorada";
									}*/

									skipNext = 1;
									prevChar = character;
									continue;
								}
								else if (character == '#')
								{
									if (j + 1 < captionText.Length && captionText[j + 1] == '#')
									{
										skipNext = 1;
									}
									else
									{
										skipNext = 6;
										if (j + 6 < captionText.Length)
										{
											runningTextColor = Util.ParseColorFromString(captionText.Substring(j, 7), false)?.ToSKColor() ?? runningTextColor;
										}
										prevChar = character;
										continue;
									}
								}

								if (runningTextColor != _baseFontPaint.Color)
								{
									_baseFontPaint.Color = runningTextColor;
								}

								// Draw the character at the current spacing position
								canvas.DrawText(actualChar, spacingX, spacingY, _baseFontPaint);

								// Advance the x position by the computed spacing
								spacingX += fontSpacing;

								// Update previous character for the next iteration
								prevChar = character;
							}
							canvas.RestoreToCount(beforeTextClip);
						}
						continue;
					}
					else if (skin.type == "MainSkin" && widget.type == "ImageBox")
					{
						if (widgetTertiaryData.properties.TryGetValue("ImageTexture", out string imagePathRel) && !string.IsNullOrEmpty(imagePathRel))
						{
							if (!_skinAtlasCache.ContainsKey("CUSTOMIMAGE_" + imagePathRel))
							{
								string imagePath = Util.ConvertToSystemPath(imagePathRel, Settings.Default.ScrapMechanicPath, Form1.ModUuidPathCache);
								SKBitmap? cachedBitmap = SKBitmap.Decode(imagePath);
								if (cachedBitmap != null)
								{
									_skinAtlasCache["CUSTOMIMAGE_" + imagePathRel] = SKImage.FromBitmap(cachedBitmap);
								}
								else
								{
									continue;
								}
							}
							var image = _skinAtlasCache["CUSTOMIMAGE_" + imagePathRel];
							drawPaint.FilterQuality = resource == _nullSkinResource ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;
							drawPaint.IsAntialias = Settings.Default.UseViewportAntiAliasing;
							drawPaint.IsDither = true;

							Color selectedColor = widgetTertiaryData.properties.TryGetValue("Colour", out string colorVal) ? (Util.ParseColorFromString(colorVal) ?? Color.White) : Color.White;
							
							float[] colorMatrix = new float[]
							{
								selectedColor.R / 255f, 0, 0, 0, 0,
								0, selectedColor.G / 255f, 0, 0, 0,
								0, 0, selectedColor.B / 255f, 0, 0,
								0, 0, 0, 1, 0
							};

							// Create a color filter using the color matrix
							SKColorFilter colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);

							// Clone the paint object and set the color filter
							drawPaint.ColorFilter = colorFilter;

							//int beforeClipSave = canvas.Save();
							//canvas.ClipRect(destRect);
							//canvas.Clear();
							canvas.DrawImage(image, clientRect, drawPaint);
							colorFilter.Dispose();
							continue;
						}
					}
				}


				//DEBUG
				if (forceDebug)
				{
					var debugPaint = new SKPaint
					{
						Color = new SKColor(255, 0, 0).WithAlpha(128),
						Style = SKPaintStyle.Stroke,
						StrokeWidth = 2
					};
					canvas.DrawRect(destRect, debugPaint);
				}
				//Debug.WriteLine(tileRect);
				// Draw the atlas texture
				if (resource == _nullSkinResource && !Settings.Default.RenderInvisibleWidgets)
				{
					continue;
				}
				if (atlasImage == null)
				{
					continue;
				}
				if (drawColor != null)
				{
					float[] colorMatrix = new float[]
					{
						drawColor.Value.Red / 255f, 0, 0, 0, 0,
						0, drawColor.Value.Green / 255f, 0, 0, 0,
						0, 0, drawColor.Value.Blue / 255f, 0, 0,
						0, 0, 0, 1, 0
					};

					// Create a color filter using the color matrix
					SKColorFilter colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);

					// Clone the paint object and set the color filter
					drawPaint.ColorFilter = colorFilter;

					//int beforeClipSave = canvas.Save();
					//canvas.ClipRect(destRect);
					//canvas.Clear();
					canvas.DrawImage(atlasImage, tileRect, destRect, drawPaint);
					colorFilter.Dispose();
					//canvas.RestoreToCount(beforeClipSave);
				}
				else
				{
					//int beforeClipSave = canvas.Save();
					//canvas.ClipRect(destRect);
					//canvas.Clear();
					drawPaint.FilterQuality = resource == _nullSkinResource ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;
					drawPaint.IsAntialias = Settings.Default.UseViewportAntiAliasing;
					drawPaint.IsDither = true;
					canvas.DrawImage(atlasImage, tileRect, destRect, drawPaint);
					//canvas.RestoreToCount(beforeClipSave);
				}
				drawPaint.Dispose();
			}
		}


		public static SKRect GetAlignedRectangle(string? align, SKRect container, SKSize tileSize, SKRect subskinOffset, SKSize maxSize)
		{
			float x = container.Left, y = container.Top, width = tileSize.Width, height = tileSize.Height;
			switch (align)
			{
				case "[DEFAULT]":
				case "Default":
				case null:
					// Default: no scaling or positioning adjustment
					return new SKRect(container.Left, container.Top, container.Left + tileSize.Width, container.Top + tileSize.Height);

				case "Stretch":
					x += subskinOffset.Left;
					y += subskinOffset.Top;
					width = container.Width - (maxSize.Width - tileSize.Width);
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Center":
					x += (container.Width - tileSize.Width) / 2;
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "Left Top":
					// No changes needed; already at top-left
					break;

				case "Left Bottom":
					y = container.Bottom - tileSize.Height;
					break;

				case "Left VStretch":
					y += subskinOffset.Top;
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Left VCenter":
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "Right Top":
					x = container.Right - tileSize.Width;
					break;

				case "Right Bottom":
					x = container.Right - tileSize.Width;
					y = container.Bottom - tileSize.Height;
					break;

				case "Right VStretch":
					x = container.Right - tileSize.Width;
					y += subskinOffset.Top;
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Right VCenter":
					x = container.Right - tileSize.Width;
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "HStretch Top":
					x += subskinOffset.Left;
					width = container.Width - (maxSize.Width - tileSize.Width);
					break;

				case "HCenter Top":
					x += (container.Width - tileSize.Width) / 2;
					break;

				case "HStretch Bottom":
					y = container.Bottom - subskinOffset.Height;
					x += subskinOffset.Left;
					width = container.Width - (maxSize.Width - tileSize.Width);
					break;

				case "HCenter Bottom":
					x += (container.Width - tileSize.Width) / 2;
					y = container.Bottom - tileSize.Height;
					break;

				default:
					//Debug.WriteLine($"Unknown align type: {align}");
					break;
			}

			return new SKRect(x, y, x + width, y + height);
		}
	}
}
