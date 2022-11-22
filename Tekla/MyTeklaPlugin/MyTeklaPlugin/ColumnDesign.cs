using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TSG3D = Tekla.Structures.Geometry3d;
using TS = Tekla.Structures;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace MyTeklaPlugin
{
    public class ColumnDesign
    {

        #region Constructor
        public ColumnDesign()
        {
            this.Clipplanes = new List<TSMUI.ClipPlane>();
            this.Dimensions = new List<TSG3D.LineSegment>();
        }

        public ColumnDesign(string line)
        {
            string[] text = line.Split(',');
            this.Story = text[0];
            this.Name = text[1];
            this.Position = text[2];

            this.Width = Convert.ToDouble(text[3]);
            this.Length = Convert.ToDouble(text[4]);
            this.QuaterLength = Convert.ToDouble(text[5]);

            this.GradeX = text[6];
            this.SizeX = text[7];
            this.NumberX = int.Parse(text[8]);

            this.GradeY = text[9];
            this.SizeY = text[10];
            this.NumberY = int.Parse(text[11]);

            this.GradeStirrupJoint = text[12];
            this.SizeStirrupJoint = text[13];
            this.SpacingStirrupJoint = Convert.ToDouble(text[14]);

            this.GradeStirrupConfined = text[15];
            this.SizeStirrupConfined = text[16];
            this.SpacingStirrupConfined = Convert.ToDouble(text[17]);

            this.GradeStirrupMiddle = text[18];
            this.SizeStirrupMiddle = text[19];
            this.SpacingStirrupMiddle = Convert.ToDouble(text[20]);

            this.GradeTieX = text[21];
            this.SizeTieX = text[22];
            this.NumberTieX = int.Parse(text[23]);

            this.GradeTieY = text[24];
            this.SizeTieY = text[25];
            this.NumberTieY = int.Parse(text[26]);

            this.BundledBarX1 = int.Parse(text[27]);
            this.BundledBarX2 = int.Parse(text[28]);
            this.BundledBarY1 = int.Parse(text[29]);
            this.BundledBarY2 = int.Parse(text[30]);

            this.SizeCorner = text[31];
            this.Cover = Convert.ToDouble(text[32]);
            this.RebarRatio = Convert.ToDouble(text[33]);

            this.Clipplanes = new List<TSMUI.ClipPlane>();
            this.Dimensions = new List<TSG3D.LineSegment>();
        }

        #endregion

        #region Property

        public string Story { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double QuaterLength { get; set; }

        public string GradeX { get; set; }
        public string SizeX { get; set; }
        public int NumberX { get; set; }

        public string GradeY { get; set; }
        public string SizeY { get; set; }
        public int NumberY { get; set; }

        public string GradeStirrupJoint { get; set; }
        public string SizeStirrupJoint { get; set; }
        public double SpacingStirrupJoint { get; set; }

        public string GradeStirrupConfined { get; set; }
        public string SizeStirrupConfined { get; set; }
        public double SpacingStirrupConfined { get; set; }

        public string GradeStirrupMiddle { get; set; }
        public string SizeStirrupMiddle { get; set; }
        public double SpacingStirrupMiddle { get; set; }

        public string GradeTieX { get; set; }
        public string SizeTieX { get; set; }
        public int NumberTieX { get; set; }

        public string GradeTieY { get; set; }
        public string SizeTieY { get; set; }
        public int NumberTieY { get; set; }

        public int BundledBarX1 { get; set; }
        public int BundledBarX2 { get; set; }
        public int BundledBarY1 { get; set; }
        public int BundledBarY2 { get; set; }

        public string SizeCorner { get; set; }
        public double Cover { get; set; }
        public double RebarRatio { get; set; }

        public string snapshot { get; set; }

        public List<TSMUI.ClipPlane> Clipplanes { set; get; }
        public List<TSG3D.LineSegment> Dimensions { set; get; }
        public TSG3D.Point CenterPoint { get; set; }

        #endregion

        static public List<ColumnDesign> ReadFromCsv(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("Big5")))
            {
                List<ColumnDesign> allColumns = new List<ColumnDesign>();
                reader.ReadLine();
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    allColumns.Add(new ColumnDesign(line));
                }

                return allColumns;
            }
        }

        // 向量轉點
        static public TSG3D.Point VectorToPoint(TSG3D.Vector vec)
        {
            return new TSG3D.Point(vec.X, vec.Y, vec.Z);
        }
        // 點轉向量
        static public TSG3D.Vector PointToVector(TSG3D.Point pt)
        {
            return new TSG3D.Vector(pt.X, pt.Y, pt.Z).GetNormal();
        }
    }
}

