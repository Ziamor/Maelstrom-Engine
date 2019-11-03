using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maelstrom.UI {
    public class Font {
        //Info
        private int size = -1;
        private Vector4 padding;
        private Vector2 spacing;

        //Common
        private int lineHeight, fontBase, scaleW, scaleH, pages, packed;

        List<FontChar> chars;
        public Font(string fontName) {
            Console.WriteLine($"Loading font {fontName}");
            string fontMetapath = $@"Assets\{fontName}.fnt";
            string[] lines = File.ReadAllLines(fontMetapath);
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);

            for (int i = 0; i < lines.Length; i++) {
                ReadLine(regex.Replace(lines[i], " "));
            }

            Console.WriteLine($"Finished loading font {fontName}");
        }

        private void ReadLine(string line) {
            string[] currentLine = line.Split(null);
            switch (currentLine[0]) {
                case "info":
                    ParseInfo(currentLine);
                    break;
                case "common":
                    ParseCommon(currentLine);
                    break;
                case "page":
                    break;
                case "chars":
                    chars = new List<FontChar>(ReadIntProperty(currentLine[1]));
                    break;
                case "char":
                    ParseChar(currentLine);
                    break;
                case "kernings":
                    //Console.WriteLine("kernings");
                    break;
                case "kerning":
                    //Console.WriteLine("kerning");
                    break;
            }
        }

        private int ReadIntProperty(string property) {
            int value = -1;
            string[] splitProperty = property.Split('=');
            bool success = false;

            if (splitProperty.Length > 1) {
                success = int.TryParse(splitProperty[1], out value);
            }

            if (!success) {
                Console.WriteLine($"ERROR: error reading int property: {property}");
            }

            return value;
        }

        private Vector2 ReadVector2Property(string property) {
            float[] values = new float[2];
            string[] splitProperty = property.Split('=');
            bool success = false;

            if (splitProperty.Length > 1) {
                string[] vecValues = splitProperty[1].Split(',');
                if (vecValues.Length > 1) {
                    for (int i = 0; i < vecValues.Length; i++) {
                        success = float.TryParse(vecValues[1], out float vecValue);
                        if (!success)
                            break;

                        values[i] = vecValue;
                    }
                }
            }

            if (!success) {
                Console.WriteLine("ERROR: error reading Vector2 property");
            }

            Vector2 value = new Vector2(values[0], values[1]);
            return value;
        }

        private Vector4 ReadVector4Property(string property) {
            float[] values = new float[4];
            string[] splitProperty = property.Split('=');
            bool success = false;

            if (splitProperty.Length > 1) {
                string[] vecValues = splitProperty[1].Split(',');
                if (vecValues.Length > 1) {
                    for (int i = 0; i < vecValues.Length; i++) {
                        success = float.TryParse(vecValues[1], out float vecValue);
                        if (!success)
                            break;

                        values[i] = vecValue;
                    }
                }
            }

            if (!success) {
                Console.WriteLine("ERROR: error reading Vector4 property");
            }

            Vector4 value = new Vector4(values[0], values[1], values[2], values[3]);
            return value;
        }

        private void ParseInfo(string[] line) {
            size = ReadIntProperty(line[2]);
            padding = ReadVector4Property(line[10]);
            spacing = ReadVector2Property(line[11]);
        }

        private void ParseCommon(string[] line) {
            lineHeight = ReadIntProperty(line[1]);
            fontBase = ReadIntProperty(line[2]);
            scaleW = ReadIntProperty(line[3]);
            scaleH = ReadIntProperty(line[4]);
            pages = ReadIntProperty(line[5]);
            packed = ReadIntProperty(line[6]);
        }

        private void ParseChar(string[] line) {
            int id = ReadIntProperty(line[1]);
            int x = ReadIntProperty(line[2]);
            int y = ReadIntProperty(line[3]);
            int width = ReadIntProperty(line[4]);
            int height = ReadIntProperty(line[5]);
            int xoffset = ReadIntProperty(line[6]);
            int yoffset = ReadIntProperty(line[7]);
            int xadvance = ReadIntProperty(line[8]);
            int page = ReadIntProperty(line[9]);
            int chnl = ReadIntProperty(line[10]);
            FontChar fontChar = new FontChar(id, x, y, width, height, xoffset, yoffset, xadvance, page, chnl);
            chars.Add(fontChar);
        }
    }

    internal class FontChar {
        private int id;
        private int x;
        private int y;
        private int width;
        private int height;
        private int xoffset;
        private int yoffset;
        private int xadvance;
        private int page;
        private int chnl;

        public FontChar(int id, int x, int y, int width, int height, int xoffset, int yoffset, int xadvance, int page, int chnl) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.xoffset = xoffset;
            this.yoffset = yoffset;
            this.xadvance = xadvance;
            this.page = page;
            this.chnl = chnl;
        }
    }
}
