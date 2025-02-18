using MyGui.net.Properties;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			public WidgetHighlightType(SKPoint position, SKColor highlightColor)
			{
				this.position = position;
				this.highlightColor = highlightColor;
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
					_renderWidgetHighligths.Add(widget, new WidgetHighlightType(widgetPosition, SKColors.Green.WithAlpha(128)));
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
				var normalState = widget.properties.TryGetValue("StateSelected", out var val) && val == "true" && skin.states.FirstOrDefault(state => state.name == "pushed") != null ? skin.states.FirstOrDefault(state => state.name == "pushed") : skin.states.FirstOrDefault(state => state.name == "normal");
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
						if (widgetTertiaryData.properties.ContainsKey("Caption"))
						{
							int beforeTextClip = canvas.Save();
							canvas.ClipRect(destRect);

							var fontData = widgetTertiaryData.properties.ContainsKey("FontName") && _allFonts.ContainsKey(widgetTertiaryData.properties["FontName"]) ? _allFonts[widgetTertiaryData.properties["FontName"]] : _allFonts["DeJaVuSans"];
							string fontPath = Path.Combine(Settings.Default.ScrapMechanicPath, "Data\\Gui\\Fonts", fontData.source);
							if (!_fontCache.ContainsKey(fontPath))
							{
								_fontCache[fontPath] = SKTypeface.FromFile(fontPath);
							}

							Color? textColor = widgetTertiaryData.properties.ContainsKey("TextColour") ? ParseColorFromString(widgetTertiaryData.properties["TextColour"]) : Color.White;
							_baseFontPaint.Color = new(textColor.Value.R, textColor.Value.G, textColor.Value.B);
							_baseFontPaint.TextSize = widgetTertiaryData.properties.ContainsKey("FontHeight") && ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) != double.NaN ? (float)ProperlyParseDouble(widgetTertiaryData.properties["FontHeight"]) : (float)fontData.size;
							_baseFontPaint.IsAntialias = Settings.Default.UseViewportFontAntiAliasing;
							_baseFontPaint.Typeface = _fontCache[fontPath];
							_baseFontPaint.FilterQuality = (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

							string captionText = Util.ReplaceLanguageTagsInString(widgetTertiaryData.properties["Caption"], Settings.Default.ReferenceLanguage, Form1.ScrapMechanicPath);

							var spacingX = destRect.Left;
							var spacingY = destRect.Top + _baseFontPaint.TextSize;
							foreach (var character in fontData.allowedChars == "ALL CHARACTERS" ? captionText : ReplaceInvalidChars(captionText, fontData.allowedChars))
							{
								/*if (!destRect.Contains(new SKPoint(spacingX + _baseFontPaint.MeasureText(character.ToString()) + (float)(fontData.letterSpacing ?? 0), spacingY)))
								{
									spacingX = destRect.Left;
									spacingY += _baseFontPaint.TextSize;
								}*/
								canvas.DrawText(character.ToString(), spacingX, spacingY, _baseFontPaint);
								spacingX += _baseFontPaint.MeasureText(character.ToString()) + (float)(fontData.letterSpacing ?? 0);
							}
							canvas.RestoreToCount(beforeTextClip);
						}
						continue;
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
				var drawPaint = new SKPaint();
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
