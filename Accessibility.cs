using StereoKit;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StereoKitApp
{
	class Accessibility
	{
		private static void ColorizeFingers(int size, Gradient horizontal, Gradient vertical)
		{
			Tex tex = new Tex(TexType.Image, TexFormat.Rgba32Linear);
			tex.AddressMode = TexAddress.Clamp;

			Color32[] pixels = new Color32[size * size];
			for (int y = 0; y < size; y++)
			{
				Color v = vertical.Get(1 - y / (size - 1.0f));
				for (int x = 0; x < size; x++)
				{
					Color h = horizontal.Get(x / (size - 1.0f));
					pixels[x + y * size] = v * h;
				}
			}
			tex.SetColors(size, size, pixels);

			Default.MaterialHand[MatParamName.DiffuseTex] = tex;
		}
		public static void Step()
        {
			UI.Label("Hand Color");
            if (UI.Button("Black"))
            {
				ColorizeFingers(16,
				new Gradient(new GradientKey(new Color(0, 0, 0, 1), 1)),
				new Gradient(
					new GradientKey(new Color(1, 1, 1, 0), 0),
					new GradientKey(new Color(1, 1, 1, 0), 0.4f),
					new GradientKey(new Color(1, 1, 1, 1), 0.6f),
					new GradientKey(new Color(1, 1, 1, 1), 0.9f)));
			}
			UI.SameLine();
            if (UI.Button("Rainbow"))
            {
				ColorizeFingers(16,
				new Gradient(
					new GradientKey(Color.HSV(0.0f, 1, 1), 0.1f),
					new GradientKey(Color.HSV(0.2f, 1, 1), 0.3f),
					new GradientKey(Color.HSV(0.4f, 1, 1), 0.5f),
					new GradientKey(Color.HSV(0.6f, 1, 1), 0.7f),
					new GradientKey(Color.HSV(0.8f, 1, 1), 0.9f)),
				new Gradient(
					new GradientKey(new Color(1, 1, 1, 0), 0),
					new GradientKey(new Color(1, 1, 1, 0), 0.4f),
					new GradientKey(new Color(1, 1, 1, 1), 0.9f)));
			}
			UI.SameLine();
			if (UI.Button("Normal"))
            {
				ColorizeFingers(16,
					new Gradient(new GradientKey(new Color(1, 1, 1, 1), 1)),
					new Gradient(
						new GradientKey(new Color(.4f, .4f, .4f, 0), 0),
						new GradientKey(new Color(.6f, .6f, .6f, 0), 0.4f),
						new GradientKey(new Color(.8f, .8f, .8f, 1), 0.55f),
						new GradientKey(new Color(1, 1, 1, 1), 1)));
			}
		}
	}
}
