using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;


namespace AutoTagSheetViews
{
    [Transaction(TransactionMode.Manual)]
    public class AutoTagCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document document = uidoc.Document;

            try
            {
                Reference pickedRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick a viewport to tag");

                Viewport pickedViewport = document.GetElement(pickedRef) as Viewport;
                if (pickedViewport == null)
                {
                    TaskDialog.Show("AutoTag", "Selected element is not a viewport.");
                    return Result.Failed;
                }

                View view = document.GetElement(pickedViewport.ViewId) as View;
                if (view == null || !view.CanBePrinted)
                {
                    TaskDialog.Show("AutoTag", "Viewport does not reference a valid plan or printable view.");
                    return Result.Failed;
                }

                BuiltInCategory targetCategory = BuiltInCategory.OST_Walls;
                int totalTags = 0;

                using (Transaction tx = new Transaction(document, "Tag elements in selected viewport"))
                {
                    tx.Start();

                    FilteredElementCollector collector = new FilteredElementCollector(document, view.Id)
                        .OfCategory(targetCategory)
                        .WhereElementIsNotElementType();

                    foreach (Element element in collector)
                    {
                        Wall wall = element as Wall;
                        if (wall != null)
                        {
                            XYZ offsetPoint = GetPerpendicularOffsetPoint(wall, view);
                            if (offsetPoint != null)
                            {
                                IndependentTag tag = IndependentTag.Create(
                                    document, view.Id, new Reference(wall), true,
                                    TagMode.TM_ADDBY_CATEGORY,
                                    TagOrientation.Horizontal,
                                    offsetPoint);

                                totalTags++;
                            }
                        }
                    }

                    tx.Commit();
                }

                TaskDialog.Show("AutoTag Complete", $"Tagged {totalTags} elements in view: {view.Name}");
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                TaskDialog.Show("AutoTag", "Operation canceled.");
                return Result.Cancelled;
            }

        }
        private XYZ GetElementCenter(Element element)
        {
            BoundingBoxXYZ boundingBoxXYZ = element.get_BoundingBox(null);
            if (boundingBoxXYZ != null)
            {
                return (boundingBoxXYZ.Min + boundingBoxXYZ.Max) / 2.0;
            }
            return null;
        }

        private XYZ GetPerpendicularOffsetPoint(Wall wall, View view, double offsetFeet = 3.0)
        {
            XYZ center = GetElementCenter(wall);
            if (center == null) return null;

            XYZ wallDir = wall.Orientation;
            XYZ viewDir = view.ViewDirection;
            XYZ perpDir = wallDir.CrossProduct(viewDir).Normalize();

            return center + (perpDir.Multiply(offsetFeet));
        }

    }
}
