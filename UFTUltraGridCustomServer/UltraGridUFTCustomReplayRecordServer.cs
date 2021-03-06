using System;
using Mercury.QTP.CustomServer;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;

namespace UFTUltraGridCustomServer
{

    [ReplayInterface]
    public interface IUltraGridUFTCustomServerReplay
    {
        #region Wizard generated sample code (commented)
        //		void  CustomMouseDown(int X, int Y);
        #endregion

        Object GetNAProperty(String propertyPath);
        Hashtable GetRow(Object filter);
        String GetLevelRow(Object filter);
        int GetX(Object filter, String headerCaption);
        int GetY(Object filter, String headerCaption);
    }
    
    /// <summary>
    /// Summary description for UltraGridUFTCustomReplayRecordServer.
    /// </summary>
    public class UltraGridUFTCustomReplayRecordServer :
        CustomServerBase,
        IUltraGridUFTCustomServerReplay
    {
        // Do not call Base class methods or properties in the constructor.
        // The services are not initialized.
        public UltraGridUFTCustomReplayRecordServer()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region IRecord override Methods
        #region Wizard generated sample code (commented)
        /*		/// <summary>
		/// To change Window messages filter, implement this method.
		/// The default implementation is to get only the 
        /// Control's window messages.
		/// </summary>
		public override WND_MsgFilter GetWndMessageFilter()
		{
			return WND_MsgFilter.WND_MSGS;
		}

		/// <summary>
		/// To catch window messages, implement this method.
		/// This method is called only if the CustomServer is running
		/// in the QuickTest context.
		/// </summary>
		public override RecordStatus OnMessage(ref Message tMsg)
		{
			// TODO:  Add OnMessage implementation.
			return RecordStatus.RECORD_HANDLED;
		}
*/
        #endregion
        /// <summary>
        /// To extend the Record process, add Events handlers
        /// to listen to the custom control's events.
        /// </summary>
        public override void InitEventListener()
        {
            #region Wizard generated sample code (commented)
            /*			// You can add as many handlers as you need.
			// For example, to add an OnMouseDown handler, 
            // first create the Delegate:
			Delegate  e = new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);

            // Then, add the event handler as the first handler of the event.
			// The first argument is the name of the event for which to listen.
			// This must be an event that the control supports. You can
			// use the .NET Spy to obtain the list of events supported by the control.
			// The second argument is the event handler delegate. 

			AddHandler("MouseDown", e);
*/
            #endregion
        }

        /// <summary>
        /// Called by QuickTest to release the handlers
        /// added in the InitEventListener method.
        /// Only handlers added using QuickTest methods are released 
        /// by the QuickTest infrastructure. If you use standard C# syntax,
        /// you must release the handlers in your code at the end 
        /// of the Record process.
        /// </summary>
        public override void ReleaseEventListener()
        {
        }

        #endregion


        #region Record events handlers
        #region Wizard generated sample code (commented)
        /*		public void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs  e)
		{
			// This example shows how to create a script line in QuickTest
			// when a MouseDown event is encountered during recording.
			if(e.Button == System.Windows.Forms.MouseButtons.Left)
			{			
				RecordFunction( "CustomMouseDown", RecordingMode.RECORD_SEND_LINE, e.X, e.Y);
			}
		}
*/
        #endregion
        #endregion


        #region Replay interface implementation
        #region Wizard generated sample code (commented)
        /*		public void CustomMouseDown(int X, int Y)
		{
			MouseClick(X, Y, MOUSE_BUTTON.LEFT_MOUSE_BUTTON);
		}
*/

        public int GetX(Object filter, String headerCaption)
        {
            int x = GetCoordinates(filter, headerCaption).X;
            ReplayReportStep("GetX", EventStatus.EVENTSTATUS_GENERAL, x);
            return x;
        }

        public int GetY(Object filter, String headerCaption)
        {
            int y = GetCoordinates(filter, headerCaption).Y;
            ReplayReportStep("GetY", EventStatus.EVENTSTATUS_GENERAL, y);
            return y;
        }

        public String GetLevelRow(Object filter)
        {
            Hashtable table = (Hashtable)filter;
            var grid = (Infragistics.Win.UltraWinGrid.UltraGrid)SourceControl;
            var resList = new List<int>();

            String resultPath = null;
            if (GetRowLevelPath(table, grid.Rows, resList))
            {
                resultPath = "";
                int i = resList.Count - 1;
                resultPath += resList[i--];
                for (; i >= 0; --i)
                {
                    resultPath += ";" + resList[i];
                }            
            }
            ReplayReportStep("GetLevelRow", EventStatus.EVENTSTATUS_GENERAL, resultPath);
            return resultPath;
        }

        public Hashtable GetRow(Object filter)
        {
            Hashtable inputTable = (Hashtable)filter;
            var grid = (Infragistics.Win.UltraWinGrid.UltraGrid)SourceControl;
            var row = FindRowByFilter(inputTable, grid.Rows);
            var result = new Hashtable();

            foreach (var cell in row.Cells)
            {
                result.Add(cell.Column.Header.Caption, cell.Text);
            }

            ReplayReportStep("GetRow", EventStatus.EVENTSTATUS_GENERAL);
            return result;
        }

        public Object GetNAProperty(String propertyPath)
        {

            var result = GetNAProperty(propertyPath, (Infragistics.Win.UltraWinGrid.UltraGrid)SourceControl);
            ReplayReportStep("GetNAProperty", EventStatus.EVENTSTATUS_GENERAL, result);
            return result;
        }

        private Object GetNAProperty(String propertyPath, object obj)
        {
            if (propertyPath.Length == 0)
            {
                ReplayThrowError("Property name is empty!");
            }

            List<ObjectPropertyDescriptor> descriptorLIst = GetListOfProperties(propertyPath);

            object currentValue = obj;
            for (int i = 0; i < descriptorLIst.Count; i++)
            {
                ObjectPropertyDescriptor descr = descriptorLIst[i];

                PropertyInfo property = currentValue.GetType().GetProperty(descr.Name);
                if (property == null)
                {
                    ReplayThrowError("Cannot find property: " + descr.Name);
                    break;
                }

                ParameterInfo[] parms = property.GetIndexParameters();
                object value = null;
                if (parms.Length == 0)
                {
                    value = property.GetValue(currentValue, null);
                    if (descr.IsIndexed)
                    {
                        Type type = value.GetType();
                        if (type.IsArray)
                        {
                            value = type.GetMethod("Get").Invoke(value, new object[] { descr.Index });
                        }
                        else // It looks like Collection
                        {
                            property = type.GetProperty("Item");
                            if (property == null)
                            {
                                ReplayThrowError("Cannot find property: \"Item\" for " + descr.Name);
                                break;
                            }
                            value = property.GetValue(value, new object[] { descr.Index });
                        }                       
                    }
                }
                else
                {
                    value = property.GetValue(currentValue, new object[] { descr.Index });
                }

                currentValue = value;
                if ((descriptorLIst.Count - i) <= 1) // last item in the list == last property we need to return value
                {
                    break;
                }
            }

            return currentValue;
        }

        private List<ObjectPropertyDescriptor> GetListOfProperties(string propertyPath)
        {
            if (propertyPath.Length == 0)
            {
                throw new ArgumentException("Empty property path");
            }

            List<ObjectPropertyDescriptor> list = new List<ObjectPropertyDescriptor>();
            int startOfPropertyName = 0;
            for (int i = 0; i < propertyPath.Length; i++)
            {
                char ch = propertyPath[i];
                while (i < propertyPath.Length && (Char.IsLetter(propertyPath[i]) || Char.IsNumber(propertyPath[i])))
                {
                    ++i;
                }
                string propertyName = propertyPath.Substring(startOfPropertyName, i - startOfPropertyName);
                startOfPropertyName = i;

                var descriptor = new ObjectPropertyDescriptor(propertyName);

                if (i < propertyPath.Length && propertyPath[i] == '[')
                {
                    descriptor.IsIndexed = true;
                    int indexStart = ++i;
                    while (i < propertyPath.Length && propertyPath[i] != ']')
                    {
                        ++i;
                    }
                    string index = propertyPath.Substring(indexStart, i - indexStart);
                    descriptor.Index = Int32.Parse(index);
                    ++i;
                }
                list.Add(descriptor);

                if (i >= propertyPath.Length)
                {
                    break;
                }

                if (propertyPath[i] == '.')
                {
                    startOfPropertyName = ++i;
                    continue;
                }
                else
                {
                    throw new Exception("Incorrect property path: mot (.) after property name or index");
                }
            }

            return list;
        }

        private bool IsRowMatchesFilter(UltraGridRow row, Hashtable filter)
        {
            int numberOfMatchedCells = 0;
            foreach (var cell in row.Cells)
            {
                if (filter.ContainsKey(cell.Column.Header.Caption))
                {
                    String val = (String)filter[cell.Column.Header.Caption];
                    if (val.CompareTo(cell.Text) == 0)
                    {
                        ++numberOfMatchedCells;
                    }
                }
            }

            // check if all matched
            return (numberOfMatchedCells == filter.Count);
        }

        private UltraGridRow FindRowByFilter(Hashtable filter, RowsCollection rows)
        {
            var row = _FindRowByFilter(filter, rows);
            if (row == null)
            {
                ReplayThrowError("Row is not found!");
            }
            return row;
        }

        private UltraGridRow _FindRowByFilter(Hashtable filter, RowsCollection rows)
        {
            // Loop through every row in the passed in rows collection.
            foreach (UltraGridRow row in rows)
            {
                
                // check if all matched
                if (IsRowMatchesFilter(row, filter))
                {
                    return row;
                }

                // If the row has any child rows. Typically, there is only a single child band. However,
                // there will be multiple child bands if the band associated with row1 has mupliple child
                // bands. This would be the case for exmple when you have a database hierarchy in which a
                // table has multiple child tables.
                if (null != row.ChildBands)
                {
                    // Loop throgh each of the child bands.
                    foreach (UltraGridChildBand childBand in row.ChildBands)
                    {
                        
                        // Call this method recursivedly for each child rows collection.
                        var result = _FindRowByFilter(filter, childBand.Rows);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        private bool GetRowLevelPath(Hashtable filter, RowsCollection rows, List<int> outList)
        {
            // Loop through every row in the passed in rows collection.
            for (int i = 0; i < rows.Count; ++i)
            {
                UltraGridRow row = rows[i];
                // If you are using Outlook GroupBy feature and have grouped rows by columns in the
                // UltraGrid, then rows collection can contain group-by rows or regular rows. So you 
                // may need to have code to handle group-by rows as well.

                if (IsRowMatchesFilter(row, filter))
                {
                    outList.Add(i);
                    return true;
                }

                // If the row has any child rows. Typically, there is only a single child band. However,
                // there will be multiple child bands if the band associated with row1 has mupliple child
                // bands. This would be the case for exmple when you have a database hierarchy in which a
                // table has multiple child tables.
                if (null != row.ChildBands)
                {
                    // Loop throgh each of the child bands.
                    for (int j = 0; j < row.ChildBands.Count; ++j)
                    {
                        var childBand = row.ChildBands[j];
                        // Call this method recursivedly for each child rows collection.
                        if (GetRowLevelPath(filter, childBand.Rows, outList))
                        {
                            outList.Add(j);
                            outList.Add(i);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private Point GetCoordinates(Object filter, String headerCaption)
        {
            Hashtable inputTable = (Hashtable)filter;
            if (inputTable.Count == 0)
            {
                ReplayThrowError("Empty dictionary!");
            }

            var grid = (Infragistics.Win.UltraWinGrid.UltraGrid)SourceControl;
            var row = FindRowByFilter(inputTable, grid.Rows);

            foreach (var cell in row.Cells)
            {
                if (cell.Column.Header.Caption.CompareTo(headerCaption) == 0)
                {
                    var uiElement = cell.GetUIElement();
                    if (uiElement == null)
                    {
                        // need to expand row.
                        ExpandRow(row);
                        uiElement = cell.GetUIElement();
                        if (uiElement == null)
                        {
                            ReplayThrowError("Cell is not visible!");
                        }
                    }

                    return uiElement.Rect.Location;
                }
            }

            ReplayThrowError("Cell is not found!");
            throw new ApplicationException("Unspecified error!");
        }

        private void ExpandRow(UltraGridRow row)
        {
            _ExpandRow(row);
        }

        private void _ExpandRow(UltraGridRow row)
        {
            if(row == null)
            {
                return;
            }
            if(row.IsExpandable)
            {
                row.Expanded = true;
            }
            _ExpandRow(row.ParentRow);
            
        }
        #endregion
        #endregion
    }
}
