using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static MyGui.net.Util;

namespace MyGui.net
{
	public static class RenderBackend
	{

		#region Caches
		public static Dictionary<string, SKImage> _skinAtlasCache = new(StringComparer.Ordinal);
		public static Dictionary<string, SKTypeface> _fontCache = new(StringComparer.Ordinal);

		public static Dictionary<string, MyGuiResource> _allResources = new(StringComparer.Ordinal);
		public static Dictionary<string, MyGuiResourceImageSet> _allImageResources = new(StringComparer.Ordinal);
        public static Dictionary<string, MyGuiFontData> _allFonts = new(StringComparer.Ordinal);

		public static Dictionary<string, MyGuiResource> AllResources => _allResources;
		public static Dictionary<string, MyGuiFontData> AllFonts => _allFonts;

		public static MyGuiResource _nullSkinResource = new MyGuiResource();
		public static MyGuiResource NullSkinResource => _nullSkinResource;

		public static Dictionary<MyGuiWidgetData, WidgetHighlightType> _renderWidgetHighligths = new();
		public static Dictionary<string, SKColor> _widgetTypeColors = new(7, StringComparer.Ordinal){
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
		/// Repositions and adjusts Widgets to be rendered by calling RenderWidget.
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
			_renderInvisibleWidgets = Settings.Default.RenderInvisibleWidgets;
			_useViewportAntiAliasing = Settings.Default.UseViewportAntiAliasing;
			_useViewportFontAntiAliasing = Settings.Default.UseViewportFontAntiAliasing;
			_screenSizeMultiplier = 1 + Settings.Default.ReferenceResolution * 0.5f;
			_filterQuality = (SKFilterQuality)Settings.Default.ViewportFilteringLevel;
			_scrapMechanicPath = Settings.Default.ScrapMechanicPath;
			_referenceLanguage = Settings.Default.ReferenceLanguage;

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
			}
			string skinPath = widget.skin != null && _allResources.TryGetValue(widget.skin, out MyGuiResource sPRes) ? sPRes?.path : "";
			skinPath ??= "";


			//Debug.WriteLine(skinPath);
			if (!_skinAtlasCache.TryGetValue(skinPath, out SKImage? skinImage))
			{
				if (!string.IsNullOrEmpty(skinPath) && widget.skin != null)
				{
					SKBitmap? cachedBitmap = SKBitmap.Decode(skinPath);
					skinImage = cachedBitmap != null ? SKImage.FromBitmap(cachedBitmap) : null;
					_skinAtlasCache[skinPath] = skinImage;
				}
				else
				{
					skinImage = SKImage.FromBitmap(LoadBitmap(_nullSkinResource.path));
					_skinAtlasCache[""] = skinImage;
				}
			}

			var saveBeforeAll = canvas.Save();
			canvas.ClipRect(rect);

			// Use the cached image we just ensured is available
			if (!string.IsNullOrEmpty(skinPath) && skinImage != null)
			{
				RenderWidget(canvas, skinImage, _allResources[widget.skin], rect, null, widget, widgetSecondaryData, widgetTertiaryData, forceDebug);
			}
			else
			{
				_allResources.TryGetValue(widget.skin, out var fallbackResource);
				_widgetTypeColors.TryGetValue(widget.type, out var debugColor);
				RenderWidget(canvas, _skinAtlasCache[""], fallbackResource ?? _nullSkinResource, rect, debugColor, widget, widgetSecondaryData, widgetTertiaryData, forceDebug);
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
						IsAntialias = _useViewportFontAntiAliasing,
						Style = SKPaintStyle.StrokeAndFill,
						StrokeWidth = 1
					};
					canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaint);
					textPaint.Color = SKColors.Black;
					textPaint.Style = SKPaintStyle.Fill;
					canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaint);
				}

				// Temporarily ignore clipping to draw selection box
				if (widget == Form1._currentSelectedWidget)
				{
					_renderWidgetHighligths[widget] = new WidgetHighlightType(widgetPosition, SKColors.Green.WithAlpha(128), null, Form1.SelectionBorderSize);
				}
				else
				{
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

		static SKPaint _baseFontPaint = new SKPaint { SubpixelText = false, HintingLevel = SKPaintHinting.NoHinting, IsAutohinted = false };
		static bool _renderInvisibleWidgets;
		static bool _useViewportAntiAliasing;
		static bool _useViewportFontAntiAliasing;
		static float _screenSizeMultiplier;
		static SKFilterQuality _filterQuality;
		static string _scrapMechanicPath;
		static string _referenceLanguage;
		static SKPaint drawPaint = new SKPaint() { IsDither = true };

		public static void RenderWidget(SKCanvas canvas, SKImage atlasImage, MyGuiResource resource, SKRect clientRect, SKColor? drawColor = null, MyGuiWidgetData? widget = null, MyGuiWidgetData? widgetSecondaryData = null, MyGuiWidgetData? widgetTertiaryData = null, bool forceDebug = false)
		{
			//TODO: Major optimizations, this is really slow atm.
			widgetSecondaryData ??= widget;
			widgetTertiaryData ??= widgetSecondaryData;
			var widgetSkin = _allResources.TryGetValue(widget.skin, out var wS) ? wS : null;
			var widgetTertiaryDataSkin = _allResources.TryGetValue(widgetTertiaryData.skin, out var wTDS) ? wTDS : null;
			if (resource == null || resource.basisSkins == null)
			{
				return; // Nothing to render if essential data is missing
			}

			//Debug.WriteLine($"Rendering widget with skin {resource.name}.");

			float defaultAlpha = widgetTertiaryData.properties.TryGetValue("Alpha", out var alphaVal) ? Util.ProperlyParseFloat(alphaVal, true) ?? 1f : 1f;
			float alpha = Util.TryGetValueFromMany([widget?.properties, widgetSkin.defaultProperties], "Alpha", out var alphaMainVal) ? (Util.ProperlyParseFloat(alphaMainVal, true) ?? 1f) * defaultAlpha : defaultAlpha;
			Color selectedColor = widgetTertiaryData.properties.TryGetValue("Colour", out string colorVal) ? (Util.ParseColorFromString(colorVal) ?? Color.White) : Color.White;

			widgetTertiaryData.properties.TryGetValue("ImageTexture", out string imagePathRel);
			widgetTertiaryData.properties.TryGetValue("ImageResource", out string imageResourceRel);

			SKColorFilter colorFilter = null;

			if (drawColor == null || imagePathRel != null || imageResourceRel != null)
			{
				float[] colorMatrix = new float[]
				{
					selectedColor.R / 255f, 0, 0, 0, 0,
					0, selectedColor.G / 255f, 0, 0, 0,
					0, 0, selectedColor.B / 255f, 0, 0,
					0, 0, 0, Math.Clamp(alpha, 0f, 1f), 0
				};
				colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
			}
			else
			{
				float[] colorMatrix = new float[]
				{
					drawColor.Value.Red / 255f, 0, 0, 0, 0,
					0, drawColor.Value.Green / 255f, 0, 0, 0,
					0, 0, drawColor.Value.Blue / 255f, 0, 0,
					0, 0, 0, 1, 0
				};

				colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
			}

			drawPaint.FilterQuality = resource == _nullSkinResource ? SKFilterQuality.None : _filterQuality;
			drawPaint.IsAntialias = _useViewportAntiAliasing;

			// Clone the paint object and set the color filter
			drawPaint.ColorFilter = colorFilter;


			// Iterate through skins in reverse order
			var renderedBasisSkins = resource.basisSkins ?? new();
			if (imagePathRel == null && imageResourceRel == null && _renderInvisibleWidgets && (resource.path ?? "") == "" && (_allResources[widgetSecondaryData.skin].resourceLayout == null || widget == _allResources[widgetSecondaryData.skin].resourceLayout[0]))
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
				var normalState = Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "StateSelected", out var val) && val == "true" && skin.states.FirstOrDefault(s => s.name == "pushed") is { } pushed ? pushed : skin.states.FirstOrDefault(s => s.name == "normal");
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

				if (widget != null)
				{
					if (skin.type == "SimpleText" || skin.type == "EditText")
					{
						if (widgetTertiaryData.properties.TryGetValue("Caption", out string caption))
						{
							int beforeTextClip = canvas.Save();
							canvas.ClipRect(destRect);

							var fontData = Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "FontName", out string value) && _allFonts.ContainsKey(value) ? _allFonts[value] : _allFonts["DeJaVuSans"];
							string fontPath = Path.Combine(_scrapMechanicPath, "Data\\Gui\\Fonts", fontData.source);
							if (!_fontCache.ContainsKey(fontPath))
							{
								_fontCache[fontPath] = SKTypeface.FromFile(fontPath);
							}

							Color? textColor;
							if (Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "TextColour", out var textColourStr))
							{
								textColor = ParseColorFromString(textColourStr);
							}
							else if (!string.IsNullOrEmpty(normalState.color))
							{
								textColor = ParseColorFromString(normalState.color);
							}
							else
							{
								textColor = Color.White;
							}
							float defaultFontSize = (float)fontData.size * _screenSizeMultiplier * 1.33f;
							float actualFontSize = widgetTertiaryData.properties.TryGetValue("FontHeight", out string fontSizeVal) ? (ProperlyParseFloat(fontSizeVal) * 0.85f) : defaultFontSize;

							_baseFontPaint.TextSize = actualFontSize;
							_baseFontPaint.IsAntialias = _useViewportFontAntiAliasing;
							_baseFontPaint.Typeface = _fontCache[fontPath];
							_baseFontPaint.FilterQuality = _filterQuality;

							SKFontMetrics metrics = _baseFontPaint.FontMetrics;

							string actualCaption = widgetTertiaryData.properties.TryGetValue("MaxTextLength", out val) && !string.IsNullOrWhiteSpace(val) ? (int.TryParse(val, out int max) ? caption.Substring(0, Math.Min(max, caption.Length)) : caption) : caption.Substring(0, Math.Min(2048, caption.Length));

							string captionText = Util.ReplaceLanguageTagsInString(
								actualCaption,
								_referenceLanguage,
								_scrapMechanicPath
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

							if (widgetTertiaryData.type == "EditBox" && (!Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "MultiLine", out var ml) || ml != "true"))
							{
								captionText = captionText.Replace("\\n", " ");
							}

							if (widgetTertiaryData.type == "EditBox" && Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "WordWrap", out var ww) && ww == "true")
							{
								captionText = WordWrap(captionText, destRect.Width, _baseFontPaint, actualFontLetterSpacing, _screenSizeMultiplier, defaultFontSize, actualFontSize);
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
								float fontSpacing = charWidth + (actualFontLetterSpacing * (actualFontSize / defaultFontSize) * _screenSizeMultiplier) + kerningAdjustment;
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

							Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "TextAlign", out string textAlign);

							if (!string.IsNullOrEmpty(textAlign) && lineWidths.Count > 0)
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
							SKColor runningTextColor = textColor.GetValueOrDefault(Color.White).ToSKColor().WithAlpha((Byte)Math.Clamp(alpha * 255, 0, 255));

							bool hasShadow = Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "TextShadow", out string textShadow) ? textShadow == "true" : false;
							SKColor? shadowColor = Util.TryGetValueFromMany([widgetTertiaryData.properties, widgetTertiaryDataSkin.defaultProperties], "TextShadowColour", out string textShadowColor) ? Util.ParseColorFromString(textShadowColor, false)?.ToSKColor() : null;

							if (shadowColor == null)
							{
								hasShadow = false;
							}

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

									// Measure current character and compute kerning adjustment.
									string actualChar = allowsAllChars ? character.ToString() : ReplaceInvalidChars(character.ToString(), fontData.allowedChars);
									float charWidth = _baseFontPaint.MeasureText(actualChar == " " ? "X" : actualChar);
									float kerningAdjustment = prevChar != '\0'
										? _baseFontPaint.MeasureText(prevChar.ToString() + actualChar) - (_baseFontPaint.MeasureText(prevChar.ToString()) + charWidth)
										: 0;

									// Total spacing: character width + letter spacing adjustment + kerning.
									float fontSpacing = charWidth + (actualFontLetterSpacing * (actualFontSize / defaultFontSize) * _screenSizeMultiplier) + kerningAdjustment;

									if (hasShadow)
									{
										_baseFontPaint.Color = shadowColor.Value;
										canvas.DrawText(actualChar, spacingX + 1, spacingY + 1, _baseFontPaint);
									}

									if (runningTextColor != _baseFontPaint.Color)
									{
										_baseFontPaint.Color = runningTextColor.WithAlpha((Byte)Math.Clamp(alpha * 255, 0, 255));
									}

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
						if (!string.IsNullOrEmpty(imagePathRel))
						{
							if (!_skinAtlasCache.ContainsKey("CUSTOMIMAGE_" + imagePathRel))
							{
								string imagePath = Util.ConvertToSystemPath(imagePathRel, _scrapMechanicPath, Form1.ModUuidPathCache);
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
							continue;
						}
						else if (!string.IsNullOrEmpty(imageResourceRel) && _allImageResources.ContainsKey(imageResourceRel))
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


							string imageResourceName = widgetTertiaryData.properties.TryGetValue("ImageName", out string iI) ? iI : "";

							Point? currPoint = currentGroup.points.TryGetValue(imageResourceName, out Point cP) ? cP : (string.IsNullOrEmpty(imageResourceName) ? currentGroup.points.First().Value : null);

							if (currPoint == null)
							{
								continue;
							}

							var pos = currPoint.Value;
							var size = Util.GetWidgetPos(true, currentGroup.size);

							canvas.DrawImage(image, new(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y), clientRect, drawPaint);
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
				if ((resource == _nullSkinResource && !_renderInvisibleWidgets) || atlasImage == null)
				{
					continue;
				}

				canvas.DrawImage(atlasImage, tileRect, destRect, drawPaint);

			}

			colorFilter.Dispose();
		}

		public static string WordWrap(string text, float maxWidth, SKPaint paint, float baseLetterSpacing, float screenSizeMultiplier, float defaultFontSize, float actualFontSize)
		{
			string wrappedText = "";
			string currentLine = "";

			var paragraphs = text.Split(new[] { "\\n" }, StringSplitOptions.None);
			for (int p = 0; p < paragraphs.Length; p++)
			{
				var para = paragraphs[p];

				if (para.Length == 0)
				{
					if (currentLine.Length > 0)
					{
						wrappedText += currentLine + "\\n";
						currentLine = "";
					}
					wrappedText += "\\n";
					continue;
				}

				// Split into tokens of spaces or non-spaces to preserve all spaces
				var matches = Regex.Matches(para, "(\\s+)|(\\S+)");
				foreach (Match match in matches)
				{
					var token = match.Value;
					// Measure token width
					float tokenWidth = 0;
					char prevChar = '\0';
					foreach (char c in token)
					{
						string cs = c.ToString();
						float cw = paint.MeasureText(cs);
						float kern = prevChar != '\0'
							? paint.MeasureText(prevChar + cs) - (paint.MeasureText(prevChar.ToString()) + cw)
							: 0;
						tokenWidth += cw + (baseLetterSpacing * (actualFontSize / defaultFontSize) * screenSizeMultiplier) + kern;
						prevChar = c;
					}

					// Determine if we need to wrap before adding token
					if (currentLine.Length > 0)
					{
						var candidate = currentLine.ToString() + token;
						float candWidth = 0;
						prevChar = '\0';
						foreach (char c in candidate)
						{
							string cs = c.ToString();
							float cw = paint.MeasureText(cs);
							float kern = prevChar != '\0'
								? paint.MeasureText(prevChar + cs) - (paint.MeasureText(prevChar.ToString()) + cw)
								: 0;
							candWidth += cw + (baseLetterSpacing * (actualFontSize / defaultFontSize) * screenSizeMultiplier) + kern;
							prevChar = c;
						}

						if (candWidth > maxWidth)
						{
							wrappedText += currentLine + "\\n";
							currentLine = "";
						}
					}

					// If token itself is wider than line width, we still place it alone
					if (currentLine.Length == 0 && tokenWidth > maxWidth)
					{
						wrappedText += token + "\\n";
						continue;
					}

					currentLine += token;
				}

				if (currentLine.Length > 0)
				{
					wrappedText += currentLine;
					if (p < paragraphs.Length - 1)
						wrappedText += "\\n";
					currentLine = "";
				}
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
