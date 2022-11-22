using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

using TS = Tekla.Structures;
using TSM = Tekla.Structures.Model;
using TSG3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using Object = System.Object;
using System.Drawing;
using System.Windows.Threading;

namespace MyTeklaPlugin
{
    public class TeklaHelper
    {
        const int SNAPSHOT_MAX_WIDTH = 1500;
        const int SNAPSHOT_MAX_HEIGHT = 1500;
        TSM.Model myModel;
        List<ColumnDesign> allColumns;
        string rcTablePath = @"attributes\ecsrc_table";
        string rcColumnDesignFileName = "RC_column_design.csv";

        /// <summary>
        /// for capturing snapshot with the same thread
        /// </summary>
        private Dispatcher teklaDispatcher { get; set; }
        private TSMUI.ModelViewEnumerator visibleViews { get; set; }


        public TeklaHelper(TSM.Model model, TSMUI.ModelViewEnumerator views, Dispatcher dispatcher)
        {
            myModel = model;
            visibleViews = views;
            teklaDispatcher = dispatcher;
            string projectFoloderPath = myModel.GetInfo().ModelPath;
            string rcColumnDesignFilePath = Path.Combine(projectFoloderPath, rcTablePath, rcColumnDesignFileName);
            allColumns = ColumnDesign.ReadFromCsv(rcColumnDesignFilePath);
        }

        public List<ColumnDesign> GetColumnDesignData()
        {
            Console.WriteLine("Start getting column design data...");
            ModelObjectEnumerator moe = new TSMUI.ModelObjectSelector().GetSelectedObjects(); 
            Console.WriteLine($"{moe.GetSize()} element{(moe.GetSize() > 1 ? "s" : "")} selected");

            List<Beam> allBeams = GetAllColumns();
            List<ColumnDesign> columnList = new List<ColumnDesign>();

            Console.WriteLine($"Num of columns: {allBeams.Count()}");


            while (moe.MoveNext())
            {
                Beam beam = moe.Current as Beam;
                if (!(moe.Current is Beam))
                    continue;

                string level = string.Empty;
                beam.GetUserProperty("COLUMN_STORY_POS", ref level);

                string pos = string.Empty;
                beam.GetUserProperty("COLUMN_POS", ref pos);
                ColumnDesign column = allColumns.Find(x => x.Story == level && x.Position == pos);

                if (column == null)
                    continue;

                // 刪除切平面
                DeleteAllClipPlanes();

                // crop and zoom
                Part part = beam as Part;
                TSMUI.ModelObjectSelector mos = new TSMUI.ModelObjectSelector();
                mos.Select(new System.Collections.ArrayList() { part });
                TS.ModelInternal.Operation.dotStartAction("ZoomToSelected", "");
                CropAndZoomToSelectedPart(part, level, pos, ref column);

                // 執行截圖
                GetSnapshot(beam, level, pos, ref column);

                columnList.Add(column);


            }
            // 刪除切平面
            DeleteAllClipPlanes();



            Console.WriteLine($"Successfully get {columnList.Count()} column design data...");
            Console.WriteLine("Done!");

            return columnList;
        }

        public List<Beam> GetAllColumns()
        {
            List<Beam> allColumns = new List<Beam>();

            TSM.ModelObjectEnumerator moe = myModel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            Console.WriteLine($"Num of beams: {moe.GetSize()}");

            while (moe.MoveNext())
            {
                Beam beam = moe.Current as Beam;

                int beamType = 0;
                if (beam.GetUserProperty("BC_AUTOREBAR_OPTION", ref beamType))
                {
                    string pos = string.Empty;
                    string level = string.Empty;
                    if (beamType == 1)
                    {

                        beam.GetUserProperty("COLUMN_STORY_POS", ref level);
                        beam.GetUserProperty("COLUMN_POS", ref pos);
                        beam.Name = $"{level}-{pos}";
                        beam.Modify();
                        allColumns.Add(beam);
                    }

                    else if (beamType == 2)
                    {
                        beam.GetUserProperty("BEAM_STORY_POS", ref level);
                        beam.GetUserProperty("BEAM_POS", ref pos);
                        beam.Name = $"{level}-{pos}";
                        beam.Modify();
                    }

                }



            }
            return allColumns;

        }
        public string GetBeamType(int typeValue)
        {
            if (typeValue == 1)
                return "柱";
            else if (typeValue == 2)
                return "梁";
            else
                return "其他";
        }

        public void GetSnapshot(Beam beam, string level, string pos, ref ColumnDesign column)
        {
            // set up and run macro
            string macroFilename = "bimuiopluginscreenshot.cs";
            string macrosFolder = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_MACRO_DIRECTORY", ref macrosFolder);
            if (macrosFolder.IndexOf(';') > 0) { macrosFolder = macrosFolder.Remove(macrosFolder.IndexOf(';')); }
            string macroFilePath = Path.Combine(macrosFolder, macroFilename);
            if (!File.Exists(macroFilePath))
            {
                Stream macroFileStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MyTeklaPlugin.TeklaPluginSnapshotMacro.cs");
                using (FileStream fs = File.Create(macroFilePath))
                {
                    macroFileStream.CopyTo(fs);
                }
            }

            string base64ImageString = teklaDispatcher.Invoke<string>(() =>
            {
                TSM.Operations.Operation.RunMacro("..\\" + macroFilename);
                Console.WriteLine("Snapshot image saved.");

                // get image from clipboard
                if (Clipboard.ContainsImage())
                {
                    var image = Clipboard.GetImage();
                    using (MemoryStream m = new MemoryStream())
                    {
                        using (var newImage = ScaleImage(image, SNAPSHOT_MAX_WIDTH, SNAPSHOT_MAX_HEIGHT))
                        {
                            newImage.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            string base64String = Convert.ToBase64String(imageBytes);
                            return base64String;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No image in clipboard.");
                }
                return null;
            });
            if (base64ImageString != null)
            {
                column.snapshot = base64ImageString;
            }
        }

        public void CropAndZoomToSelectedPart(Part selectedObj, string level, string pos, ref ColumnDesign column)
        {
            if (selectedObj == null)
                return;
            visibleViews.Reset();
            List<TSMUI.ClipPlane> clipplanes = new List<TSMUI.ClipPlane>();
            TSG3D.CoordinateSystem beamCoorSys = selectedObj.GetCoordinateSystem();
            TransformationPlane beamTransformationPlane = new TransformationPlane(beamCoorSys);
            myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(beamTransformationPlane);

            AABB PartBoundingBox = new AABB();
            Solid PartSolid = selectedObj.GetSolid();
            double extendDistance = 1000;
            double MaximumX = PartSolid.MaximumPoint.X + extendDistance;
            double MaximumY = PartSolid.MaximumPoint.Y + extendDistance;
            double MaximumZ = PartSolid.MaximumPoint.Z + extendDistance;
            double MinimumX = PartSolid.MinimumPoint.X;
            double MinimumY = PartSolid.MinimumPoint.Y - extendDistance;
            double MinimumZ = PartSolid.MinimumPoint.Z - extendDistance;
            double MiddleX = (MaximumX + MinimumX) / 2;
            double MiddleY = (MaximumY + MinimumY) / 2;
            double MiddleZ = (MaximumZ + MinimumZ) / 2;

            TSG3D.Point p1 = new TSG3D.Point(MinimumX, PartSolid.MinimumPoint.Y, PartSolid.MinimumPoint.Z);
            TSG3D.Point p2 = new TSG3D.Point(MinimumX, PartSolid.MinimumPoint.Y, PartSolid.MaximumPoint.Z);
            TSG3D.Point p3 = new TSG3D.Point(MinimumX, PartSolid.MaximumPoint.Y, PartSolid.MinimumPoint.Z);
            TSG3D.Point p4 = new TSG3D.Point(MinimumX, PartSolid.MaximumPoint.Y, PartSolid.MaximumPoint.Z);
            TSG3D.Point p5 = new TSG3D.Point(PartSolid.MaximumPoint.X, PartSolid.MinimumPoint.Y, PartSolid.MinimumPoint.Z);
            TSG3D.Point p6 = new TSG3D.Point(PartSolid.MaximumPoint.X, PartSolid.MinimumPoint.Y, PartSolid.MaximumPoint.Z);
            TSG3D.Point p7 = new TSG3D.Point(PartSolid.MaximumPoint.X, PartSolid.MaximumPoint.Y, PartSolid.MinimumPoint.Z);
            TSG3D.Point p8 = new TSG3D.Point(PartSolid.MaximumPoint.X, PartSolid.MaximumPoint.Y, PartSolid.MaximumPoint.Z);

            // Transform to global
            p1 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p1);
            p2 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p2);
            p3 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p3);
            p4 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p4);
            p5 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p5);
            p6 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p6);
            p7 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p7);
            p8 = beamTransformationPlane.TransformationMatrixToGlobal.Transform(p8);

            // Add dimenstions to column
            // bottom
            column.Dimensions.Add(new LineSegment(p1, p2));
            column.Dimensions.Add(new LineSegment(p1, p3));
            column.Dimensions.Add(new LineSegment(p2, p4));
            column.Dimensions.Add(new LineSegment(p4, p3));

            //top
            column.Dimensions.Add(new LineSegment(p5, p6));
            column.Dimensions.Add(new LineSegment(p5, p7));
            column.Dimensions.Add(new LineSegment(p6, p8));
            column.Dimensions.Add(new LineSegment(p8, p7));

            // Height
            column.Dimensions.Add(new LineSegment(p1, p5));

            PartBoundingBox.MaxPoint = PartSolid.MaximumPoint;
            PartBoundingBox.MinPoint = PartSolid.MinimumPoint;

            // set center point
            column.CenterPoint = beamTransformationPlane.TransformationMatrixToGlobal.Transform(PartBoundingBox.GetCenterPoint());

            while (visibleViews.MoveNext())
            {
                TSMUI.View ViewSel = visibleViews.Current;
                TSMUI.ClipPlane cp = new TSMUI.ClipPlane();
                cp.View = ViewSel;
                cp.Location = new TSG3D.Point(MiddleX, MinimumY, MiddleZ);
                cp.UpVector = new Vector(0, -1, 0);
                TSG3D.Point abovePlanePt = new TSG3D.Point(new TSG3D.Point(MiddleX, MinimumY, MiddleZ) + new Vector(0, -1, 0));
                TSG3D.Point transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                TSG3D.Point transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                TSG3D.Vector transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                cp.Location = new TSG3D.Point(MaximumX, MiddleY, MiddleZ);
                cp.UpVector = new Vector(1, 0, 0);
                abovePlanePt = new TSG3D.Point(cp.Location + cp.UpVector);
                transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                cp.Location = new TSG3D.Point(MiddleX, MaximumY, MiddleZ);
                cp.UpVector = new Vector(0, 1, 0);
                abovePlanePt = new TSG3D.Point(cp.Location + cp.UpVector);
                transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                cp.Location = new TSG3D.Point(MinimumX + 10, MiddleY, MiddleZ);
                cp.UpVector = new Vector(-1, 0, 0);
                abovePlanePt = new TSG3D.Point(cp.Location + cp.UpVector);
                transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                cp.Location = new TSG3D.Point(MiddleX, MiddleY, MinimumZ);
                cp.UpVector = new Vector(0, 0, -1);
                abovePlanePt = new TSG3D.Point(cp.Location + cp.UpVector);
                transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                cp.Location = new TSG3D.Point(MiddleX, MiddleY, MaximumZ);
                cp.UpVector = new Vector(0, 0, 1);
                abovePlanePt = new TSG3D.Point(cp.Location + cp.UpVector);
                transformedLocation = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(cp.Location));
                transformedAbovePlanePt = new TSG3D.Point(beamTransformationPlane.TransformationMatrixToGlobal.Transform(abovePlanePt));
                transformedUpVec = new TSG3D.Vector(transformedAbovePlanePt - transformedLocation);
                cp.Insert();

                // set bounding box to required size
                PartBoundingBox.MinPoint = beamTransformationPlane.TransformationMatrixToGlobal.Transform(PartBoundingBox.MinPoint);
                PartBoundingBox.MaxPoint = beamTransformationPlane.TransformationMatrixToGlobal.Transform(PartBoundingBox.MaxPoint);

                // Add Coupler dimensions
                column.Dimensions.AddRange(GetCouplerDimensionsBySelectedPart(selectedObj));

                // set clipplanes
                column.Clipplanes = clipplanes;
            }
        }

        public List<LineSegment> GetCouplerDimensionsBySelectedPart(Part part)
        {
            SortedList<double, LineSegment> allCouplersDimensions = new SortedList<double, LineSegment>();

            Solid solid = part.GetSolid();
            //Console.WriteLine($"Get couplers in {part.Identifier}...");
            TSM.ModelObjectEnumerator moe = myModel.GetModelObjectSelector().GetObjectsByBoundingBox(solid.MinimumPoint, solid.MaximumPoint);
            double minZ = solid.MinimumPoint.Z;
            while (moe.MoveNext())
            {
                if (moe.Current is Beam)
                {
                    Beam coupler = moe.Current as Beam;
                    if (coupler.Name == "COUPLER")
                    {
                        TSG3D.Point p = new TSG3D.Point();
                        TSG3D.Point tempPoint = new TSG3D.Point(coupler.StartPoint + coupler.EndPoint);
                        TSG3D.Point midPoint = new TSG3D.Point(tempPoint.X / 2, tempPoint.Y / 2, tempPoint.Z / 2);
                        LineSegment lineSeg = new LineSegment(midPoint, new TSG3D.Point(midPoint.X, midPoint.Y, minZ));
                        if (!allCouplersDimensions.ContainsKey(Math.Round(lineSeg.Length())))
                            allCouplersDimensions.Add(Math.Round(lineSeg.Length()), lineSeg);
                    }

                }
            }

            return allCouplersDimensions.Values.ToList();
        }

        public void DeleteAllClipPlanes()
        {
            TSMUI.ModelViewEnumerator ViewEnum = TSMUI.ViewHandler.GetVisibleViews();
            while (ViewEnum.MoveNext())
            {
                TSMUI.View ViewSel = ViewEnum.Current;
                TSMUI.ClipPlaneCollection cPlanes = ViewSel.GetClipPlanes();
                foreach (TSMUI.ClipPlane cPlane in cPlanes)
                {
                    cPlane.Delete();
                }
            }
        }

        private static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);

            using (var graphics = System.Drawing.Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}
