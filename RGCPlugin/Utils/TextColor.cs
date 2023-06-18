using System;

using PluginAPI.Core;

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace RGCPlugin.Utils
{
    public class TextColor : IYamlConvertible
    {
        public byte R = 255;
        public byte G = 255;
        public byte B = 255;

        public TextColor() { }
        public TextColor(string colorString) => LoadColorFromString(colorString);
        public TextColor(byte R = 255, byte G = 255, byte B = 255)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        // Loads the color string (hex, rgb)
        public void LoadColorFromString(string color)
        {
            if (color.StartsWith("#")) // Check if the format is hex (#ffffff)
            {
                color = color.Substring(1);
                switch (color.Length)
                {
                    case 1:
                        {
                            byte v = Convert.ToByte(color + color, 16);
                            R = v;
                            G = v;
                            B = v;
                        }
                        break;

                    case 2:
                        {
                            byte v = Convert.ToByte(color, 16);
                            R = v;
                            G = v;
                            B = v;
                        }
                        break;

                    case 3:
                        {
                            R = Convert.ToByte(color.Shift(1, out color).Replicate(2), 16);
                            G = Convert.ToByte(color.Shift(1, out color).Replicate(2), 16);
                            B = Convert.ToByte(color.Shift(1, out color).Replicate(2), 16);
                        }
                        break;

                    case 6:
                        {
                            R = Convert.ToByte(color.Shift(2, out color), 16);
                            G = Convert.ToByte(color.Shift(2, out color), 16);
                            B = Convert.ToByte(color.Shift(2, out color), 16);
                        }
                        break;

                    default:
                        Log.Error("A hex color must be (1, 2, 3, 6) characters long excluding #");
                        break;
                }
            }
            else if (color.NumberOfChars(",") == 2) // Check if the format is RGB (255, 255, 255 | 255,255,255) with or without spaces
            {
                color = color.Replace(" ", "");

                string[] rgb = color.Split(',');

                if (byte.TryParse(rgb[0], out byte r))
                    R = r;
                else Log.Warning("The value of R is invalid");

                if (byte.TryParse(rgb[1], out byte g))
                    G = g;
                else Log.Warning("The value of G is invalid");

                if (byte.TryParse(rgb[2], out byte b))
                    B = b;
                else Log.Warning("The value of B is invalid");
            }
            else Log.Error($"Invalid color string provided '{color}'");
        }

        // Converts the RGB to the target format
        public string ColorToHex()
        {
            string GetCode(byte v) => Convert.ToString(v, 16).PadLeft(2, '0');

            return $"#{GetCode(R)}{GetCode(G)}{GetCode(B)}";
        }
        public string ColorToRGB() => $"{R}, {G}, {B}";

        // Returns the unity formated string
        public string GetColoredText(string text) => $"<color={ColorToHex()}>{text}</color>";

        // Handles the de/serialization of this object
        public void Read(IParser _, Type __, ObjectDeserializer des)
        {
            string data = (string)des(typeof(string)); // Gets the data

            LoadColorFromString(data); // Loads the color
        }
        public void Write(IEmitter _, ObjectSerializer ser) 
        {
            // Adding this as i had a 4 hour long issue, until i found out that the Instance was null
            if (Plugin.Instance == null)
            {
                ser(ColorToRGB());
                return;
            }

            if (Plugin.Instance.Config.SerializeColorsAsHex)
                ser(ColorToHex());
            else ser(ColorToRGB());
        }
    }
}
