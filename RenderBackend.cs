using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using System.Text;
using System.Web;
using static MyGui.net.Util;

namespace MyGui.net
{
	public static class RenderBackend
	{

		#region Caches
		public static Dictionary<string, SKImage> _skinAtlasCache = new();
		public static Dictionary<string, SKTypeface> _fontCache = new();

		public static Dictionary<string, MyGuiResource> _allResources = new();
		public static Dictionary<string, MyGuiResourceImageSet> _allImageResources = new();
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
			public bool ignoreDrawOrder;

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

				var selfHighlights = _renderWidgetHighligths.Where(item => item.Key == widget && !item.Value.ignoreDrawOrder);

				if (selfHighlights != null && selfHighlights.Any())
				{
					foreach (var highlight in selfHighlights)
					{
						var rect2 = new SKRect(highlight.Value.position.X, highlight.Value.position.Y,
										  highlight.Value.position.X + highlight.Key.size.X, highlight.Value.position.Y + highlight.Key.size.Y);
						// Draw selection highlight without any clipping
						var selectionRect = new SKRect(
							rect2.Left - highlight.Value.width / 2,  // Expand left
							rect2.Top - highlight.Value.width / 2,   // Expand top
							rect2.Right + highlight.Value.width / 2, // Expand right
							rect2.Bottom + highlight.Value.width / 2 // Expand bottom
						);
						var highlightPaint = new SKPaint
						{
							Color = highlight.Value.highlightColor, // Semi-transparent green for highlight
							Style = highlight.Value.style,
							StrokeWidth = highlight.Value.width,
							IsAntialias = true
						};
						canvas.DrawRect(selectionRect, highlightPaint);
						_renderWidgetHighligths.Remove(widget);
					}
				}


				if (Settings.Default.RenderWidgetNames && !string.IsNullOrEmpty(widget.name))
				{
					var textPaint = new SKPaint
					{
						Color = SKColors.White,
						TextSize = 16,
						IsAntialias = Settings.Default.UseViewportFontAntiAliasing,
						Style = SKPaintStyle.StrokeAndFill,
						StrokeWidth = 1
					};
					canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaint);
					var textPaintStroke = new SKPaint
					{
						Color = SKColors.Black,
						TextSize = 16,
						IsAntialias = Settings.Default.UseViewportFontAntiAliasing
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
					//LISTEN UP YOU DUMBASS TRB; 720p is 1x, 1080p is 1.5x, 1440p is 2x, !!!!720p IS NOT HALF OF 1080p!!!! - Left this comment here for anyone that checks out the source to find lol - The Red Builder
					//Ima be honest here; i kinda gave up making it work by myself, so a lot of this code is AI generated, though it does work really well actually.
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

							float screenSizeMultiplier = 1 + Settings.Default.ReferenceResolution * 0.5f;
							float defaultFontSize = (float)fontData.size * screenSizeMultiplier * 1.33f;
							float actualFontSize = widgetTertiaryData.properties.TryGetValue("FontHeight", out string fontSizeVal) ? (ProperlyParseFloat(fontSizeVal) * 0.85f) : defaultFontSize;

							_baseFontPaint.TextSize = actualFontSize;
							_baseFontPaint.IsAntialias = Settings.Default.UseViewportFontAntiAliasing;
							_baseFontPaint.Typeface = _fontCache[fontPath];
							_baseFontPaint.FilterQuality = (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

							SKFontMetrics metrics = _baseFontPaint.FontMetrics;
							string captionText = Util.ReplaceLanguageTagsInString(
								widgetTertiaryData.properties["Caption"],
								Settings.Default.ReferenceLanguage,
								Form1.ScrapMechanicPath
							);

							// Build lines and calculate their widths while incorporating letter spacing.
							List<string> lines = new List<string>();
							List<float> lineWidths = new List<float>();


							float currentLineWidth = 0;
							StringBuilder currentLineBuilder = new();

							int skipNext = 0;
							bool allowsAllChars = fontData.allowedChars == "ALL CHARACTERS";
							// Get letter spacing value, if available.
							float actualFontLetterSpacing = (float)(fontData.letterSpacing ?? 0);

							float widgetWidth = destRect.Right - destRect.Left;
							float widgetHeight = destRect.Bottom - destRect.Top;

							if (widgetTertiaryData.type == "EditBox" && (!widgetTertiaryData.properties.TryGetValue("MultiLine", out string ml) || ml != "true"))
							{
								captionText = captionText.Replace("\\n", " ");
							}

							if (widgetTertiaryData.type == "EditBox" && widgetTertiaryData.properties.TryGetValue("WordWrap", out string ww) && ww == "true")
							{
								captionText = WordWrap(captionText, destRect.Width, _baseFontPaint, actualFontLetterSpacing, screenSizeMultiplier, defaultFontSize, actualFontSize);
							}

							char prevChar = '\0';

							for (int j = 0; j < captionText.Length; j++)
							{
								char character = captionText[j];

								if (skipNext > 0)
								{
									skipNext--;
									if (character != 'n')
									{
										currentLineBuilder.Append(character);
									}
									prevChar = character;
									continue;
								}

								if (character == '\\' && j + 1 < captionText.Length && captionText[j + 1] == 'n')
								{
									lines.Add(currentLineBuilder.ToString());
									lineWidths.Add(currentLineWidth);
									currentLineBuilder.Clear();
									currentLineWidth = 0;
									prevChar = '\0';
									skipNext = 1; // Skip the 'n'
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
									}
									currentLineBuilder.Append(character);
									prevChar = character;
									continue;
								}

								currentLineBuilder.Append(character);
								float charWidth = _baseFontPaint.MeasureText(allowsAllChars ? character.ToString() : ReplaceInvalidChars(character.ToString(), fontData.allowedChars));
								float kerningAdjustment = prevChar != '\0'
									? _baseFontPaint.MeasureText(prevChar.ToString() + character.ToString()) - (_baseFontPaint.MeasureText(prevChar.ToString()) + charWidth)
									: 0;

								// Incorporate letter spacing adjustment into the total width.
								float fontSpacing = charWidth + (actualFontLetterSpacing * (actualFontSize / defaultFontSize) * screenSizeMultiplier) + kerningAdjustment;
								currentLineWidth += fontSpacing;
								prevChar = character;
							}

							if (currentLineBuilder.Length > 0)
							{
								lines.Add(currentLineBuilder.ToString());
								lineWidths.Add(currentLineWidth);
							}

							float lineHeight = Math.Abs(metrics.Ascent) + Math.Abs(metrics.Descent);
							float totalTextHeight = lines.Count * lineHeight;

							float offsetX = 0;
							float offsetY = 0;

							if (widgetTertiaryData.properties.TryGetValue("TextAlign", out string textAlign))
							{
								if (textAlign.Contains("VCenter") || textAlign == "Center")
								{
									offsetY = (widgetHeight - totalTextHeight) / 2f;
								}
								else if (textAlign.Contains("Bottom"))
								{
									offsetY = widgetHeight - totalTextHeight;
								}

								if (textAlign.Contains("HCenter") || textAlign == "Center")
								{
									offsetX = (widgetWidth - lineWidths.Max()) / 2f;
								}
								else if (textAlign.Contains("Right"))
								{
									offsetX = widgetWidth - lineWidths.Max();
								}
							}

							float spacingY = destRect.Top + offsetY - metrics.Top;
							SKColor runningTextColor = textColor.GetValueOrDefault(Color.White).ToSKColor();

							// Draw each line with letter spacing incorporated.
							for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
							{
								string lineText = lines[lineIndex];
								float thisLineWidth = lineWidths[lineIndex];

								float spacingX = destRect.Left;
								if (!string.IsNullOrEmpty(textAlign))
								{
									if (textAlign.Contains("HCenter") || textAlign == "Center")
									{
										spacingX += (widgetWidth - thisLineWidth) / 2f;
									}
									else if (textAlign.Contains("Right"))
									{
										spacingX += widgetWidth - thisLineWidth;
									}
								}

								skipNext = 0;
								prevChar = '\0';

								for (int j = 0; j < lineText.Length; j++)
								{
									char character = lineText[j];

									if (skipNext > 0)
									{
										skipNext--;
										prevChar = character;
										continue;
									}

									if (character == '\\' && j + 1 < lineText.Length && lineText[j + 1] == 'n')
									{
										skipNext = 1;
										continue;
									}

									if (character == '#')
									{
										if (j + 1 < lineText.Length && lineText[j + 1] == '#')
										{
											skipNext = 1;
										}
										else
										{
											skipNext = 6;
											if (j + 6 < lineText.Length)
											{
												runningTextColor = Util.ParseColorFromString(lineText.Substring(j, 7), false)?.ToSKColor() ?? runningTextColor;
											}
											prevChar = character;
											continue;
										}
									}

									if (runningTextColor != _baseFontPaint.Color)
									{
										_baseFontPaint.Color = runningTextColor;
									}

									// Measure current character and compute kerning adjustment.
									string actualChar = allowsAllChars ? character.ToString() : ReplaceInvalidChars(character.ToString(), fontData.allowedChars);
									float charWidth = _baseFontPaint.MeasureText(actualChar == " " ? "X" : actualChar);
									float kerningAdjustment = prevChar != '\0'
										? _baseFontPaint.MeasureText(prevChar.ToString() + actualChar) - (_baseFontPaint.MeasureText(prevChar.ToString()) + charWidth)
										: 0;

									// Total spacing: character width + letter spacing adjustment + kerning.
									float fontSpacing = charWidth + (actualFontLetterSpacing * (actualFontSize / defaultFontSize) * screenSizeMultiplier) + kerningAdjustment;

									canvas.DrawText(actualChar, spacingX, spacingY, _baseFontPaint);
									spacingX += fontSpacing;

									prevChar = character;
								}

								spacingY += lineHeight;
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

							float alpha = widgetTertiaryData.properties.TryGetValue("Alpha", out string alphaVal) ? Util.ProperlyParseFloat(alphaVal) : 1f;

							float[] colorMatrix = new float[]
							{
								selectedColor.R / 255f, 0, 0, 0, 0,
								0, selectedColor.G / 255f, 0, 0, 0,
								0, 0, selectedColor.B / 255f, 0, 0,
								0, 0, 0, Math.Clamp(alpha, 0f, 1f), 0
							};

							// Create a color filter using the color matrix
							SKColorFilter colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);

							// Clone the paint object and set the color filter
							drawPaint.ColorFilter = colorFilter;

							//int beforeClipSave = canvas.Save();
							//canvas.ClipRect(destRect);
							//canvas.Clear();

							if (widgetTertiaryData.properties.TryGetValue("ImageCoord", out string imgCoord))
							{
								var offsets = Util.GetWidgetPosAndSize(true, imgCoord, new(1, 1));
								var pos = offsets.Item1;
								var size = offsets.Item2;

								canvas.DrawImage(image, new(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y), clientRect, drawPaint);
							}
							else
							{
								canvas.DrawImage(image, clientRect, drawPaint);
							}
							colorFilter.Dispose();
							continue;
						}
						else if (widgetTertiaryData.properties.TryGetValue("ImageResource", out string imageResourceRel) && !string.IsNullOrEmpty(imageResourceRel) && _allImageResources.ContainsKey(imageResourceRel))
						{
							string imageResourceGroup = widgetTertiaryData.properties.TryGetValue("ImageGroup", out string iRG) ? iRG : "";
							var imageResource = _allImageResources[imageResourceRel];

							var currentGroup = imageResource.groups.TryGetValue(imageResourceGroup, out MyGuiResourceImageSetGroup cG) ? cG : (string.IsNullOrEmpty(iRG) ? imageResource.groups.First().Value : null);

							if (currentGroup == null)
							{
								continue;
							}

							if (!_skinAtlasCache.ContainsKey("RESOURCE_" + currentGroup.path))
							{
								SKBitmap? cachedBitmap = SKBitmap.Decode(currentGroup.path);
								if (cachedBitmap != null)
								{
									_skinAtlasCache["RESOURCE_" + currentGroup.path] = SKImage.FromBitmap(cachedBitmap);
								}
								else
								{
									continue;
								}
							}
							var image = _skinAtlasCache["RESOURCE_" + currentGroup.path];
							drawPaint.FilterQuality = resource == _nullSkinResource ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;
							drawPaint.IsAntialias = Settings.Default.UseViewportAntiAliasing;
							drawPaint.IsDither = true;

							Color selectedColor = widgetTertiaryData.properties.TryGetValue("Colour", out string colorVal) ? (Util.ParseColorFromString(colorVal) ?? Color.White) : Color.White;
							
							float alpha = widgetTertiaryData.properties.TryGetValue("Alpha", out string alphaVal) ? Util.ProperlyParseFloat(alphaVal) : 1f;

							float[] colorMatrix = new float[]
							{
								selectedColor.R / 255f, 0, 0, 0, 0,
								0, selectedColor.G / 255f, 0, 0, 0,
								0, 0, selectedColor.B / 255f, 0, 0,
								0, 0, 0, Math.Clamp(alpha, 0f, 1f), 0
							};

							// Create a color filter using the color matrix
							SKColorFilter colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);

							// Clone the paint object and set the color filter
							drawPaint.ColorFilter = colorFilter;


							string imageResourceName = widgetTertiaryData.properties.TryGetValue("ImageName", out string iI) ? iI : "";

							Point? currPoint = currentGroup.points.TryGetValue(imageResourceName, out Point cP) ? cP : (string.IsNullOrEmpty(imageResourceName) ? currentGroup.points.First().Value : null);

							if (currPoint == null)
							{
								continue;
							}

							var pos = currPoint.Value;
							var size = Util.GetWidgetPos(true, currentGroup.size);

							canvas.DrawImage(image, new(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y), clientRect, drawPaint);
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

					Color selectedColor = widgetTertiaryData.properties.TryGetValue("Colour", out string colorVal) ? (Util.ParseColorFromString(colorVal) ?? Color.White) : Color.White;

					float alpha = widgetTertiaryData.properties.TryGetValue("Alpha", out string alphaVal) ? Util.ProperlyParseFloat(alphaVal) : 1f;

					float[] colorMatrix = new float[]
					{
							selectedColor.R / 255f, 0, 0, 0, 0,
							0, selectedColor.G / 255f, 0, 0, 0,
							0, 0, selectedColor.B / 255f, 0, 0,
							0, 0, 0, Math.Clamp(alpha, 0f, 1f), 0
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

				drawPaint.Dispose();
			}
		}

		public static string WordWrap(string text, float maxWidth, SKPaint paint, float baseLetterSpacing, float screenSizeMultiplier, float defaultFontSize, float actualFontSize)
		{
			StringBuilder wrappedText = new StringBuilder();
			StringBuilder currentLine = new StringBuilder();
			string[] words = text.Split(' ');

			foreach (string word in words)
			{
				// Prepare candidate line: if there's already content, add a space before the new word.
				string candidate = currentLine.Length > 0 ? currentLine + " " + word : word;
				float candidateWidth = 0;
				char prevChar = '\0';

				// Measure the candidate line character by character.
				foreach (char c in candidate)
				{
					string charStr = c.ToString();
					float charWidth = paint.MeasureText(charStr);
					float kerningAdjustment = 0;
					if (prevChar != '\0')
					{
						kerningAdjustment = paint.MeasureText(prevChar.ToString() + charStr) -
											(paint.MeasureText(prevChar.ToString()) + charWidth);
					}
					// Total spacing includes the letter spacing adjustment.
					float totalSpacing = charWidth + (baseLetterSpacing * (actualFontSize / defaultFontSize) * screenSizeMultiplier) + kerningAdjustment;
					candidateWidth += totalSpacing;
					prevChar = c;
				}

				// If the candidate exceeds maxWidth and there is already content, wrap.
				if (candidateWidth > maxWidth && currentLine.Length > 0)
				{
					wrappedText.Append(currentLine.ToString());
					wrappedText.Append("\\n");  // Add newline after current line.
					currentLine.Clear();
					currentLine.Append(word);
				}
				else
				{
					if (currentLine.Length > 0)
						currentLine.Append(' ');
					currentLine.Append(word);
				}
			}

			// Add the remaining content in currentLine to wrappedText.
			if (currentLine.Length > 0)
			{
				wrappedText.Append(currentLine.ToString());
			}

			return wrappedText.ToString();
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
