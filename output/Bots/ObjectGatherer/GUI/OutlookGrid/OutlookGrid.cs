// Copyright 2006 Herre Kuijpers - <herre@xs4all.nl>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ObjectGatherer.GUI.OutlookGrid {
    #region implementation of the OutlookGrid!
    public partial class OutlookGrid : DataGridView {
        #region OutlookGrid constructor
        public OutlookGrid() {
            InitializeComponent();

            // very important, this indicates that a new default row class is going to be used to fill the grid
            // in this case our custom OutlookGridRow class
            base.RowTemplate = new OutlookGridRow();
            _groupTemplate = new OutlookgGridDefaultGroup();

        }
        #endregion OutlookGrid constructor

        #region OutlookGrid property definitions
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new DataGridViewRow RowTemplate {
            get { return base.RowTemplate; }
        }

        private IOutlookGridGroup _groupTemplate;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IOutlookGridGroup GroupTemplate {
            get {
                return _groupTemplate;
            }
            set {
                _groupTemplate = value;
            }
        }

        [Category("Appearance")]
        public Image CollapseIcon { get; set; }

        [Category("Appearance")]
        public Image ExpandIcon { get; set; }


        private DataSourceManager _dataSource;
        public new object DataSource {
            get {
                if(_dataSource == null) return null;

                // special case, datasource is bound to itself.
                // for client it must look like no binding is set,so return null in this case
                return _dataSource.DataSource.Equals(this) ? null : _dataSource.DataSource;

                // return the origional datasource.
            }
            set { throw new NotImplementedException(); }
        }

        #endregion OutlookGrid property definitions

        #region OutlookGrid new methods
        public void CollapseAll() {
            SetGroupCollapse(true);
        }

        public void ExpandAll() {
            SetGroupCollapse(false);
        }

        public void ClearGroups() {
            _groupTemplate.Column = null; //reset
            FillGrid(null);
        }

        public void BindData(object dataSource, string dataMember) {
            DataMember = DataMember;
            if(dataSource == null) {
                _dataSource = null;
                Columns.Clear();
            } else {
                _dataSource = new DataSourceManager(dataSource, dataMember);
                SetupColumns();
                FillGrid(null);
            }
        }
        public override void Sort(IComparer comparer) {
            if(_dataSource == null) // if no datasource is set, then bind to the grid itself
                _dataSource = new DataSourceManager(this, null);

            _dataSource.Sort(comparer);
            FillGrid(_groupTemplate);
        }


        public override void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction) {
            if(_dataSource == null) // if no datasource is set, then bind to the grid itself
                _dataSource = new DataSourceManager(this, null);

            _dataSource.Sort(new OutlookGridRowComparer(dataGridViewColumn.Index, direction));
            FillGrid(_groupTemplate);
        }
        #endregion OutlookGrid new methods

        #region OutlookGrid event handlers
        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e) {
            var row = (OutlookGridRow)Rows[e.RowIndex];
            if(row.IsGroupRow)
                e.Cancel = true;
            else
                base.OnCellBeginEdit(e);
        }

        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e) {
            if(e.RowIndex >= 0) {

                var row = (OutlookGridRow)Rows[e.RowIndex];
                if(!row.IsGroupRow) {
                    return;
                }

                row.Group.Collapsed = !row.Group.Collapsed;

                //this is a workaround to make the grid re-calculate it's contents and backgroun bounds
                // so the background is updated correctly.
                // this will also invalidate the control, so it will redraw itself
                row.Visible = false;
                row.Visible = true;
                return;
            }
            base.OnCellClick(e);
        }

        // the OnCellMouseDown is overriden so the control can check to see if the
        // user clicked the + or - sign of the group-row
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e) {
            if(e.RowIndex < 0) return;

            var row = (OutlookGridRow)Rows[e.RowIndex];
            if(row.IsGroupRow && row.IsIconHit(e)) {
                Debug.WriteLine("OnCellMouseDown " + DateTime.Now.Ticks);
                row.Group.Collapsed = !row.Group.Collapsed;

                //this is a workaround to make the grid re-calculate it's contents and backgroun bounds
                // so the background is updated correctly.
                // this will also invalidate the control, so it will redraw itself
                row.Visible = false;
                row.Visible = true;
            } else
                base.OnCellMouseDown(e);
        }
        #endregion OutlookGrid event handlers

        #region Grid Fill functions
        private void SetGroupCollapse(bool collapsed) {
            try {
                if(Rows.Count == 0) return;
                if(_groupTemplate == null) return;

                // set the default grouping style template collapsed property
                _groupTemplate.Collapsed = collapsed;

                // loop through all rows to find the GroupRows
                foreach(var row in Rows.Cast<OutlookGridRow>().Where(row => row.IsGroupRow)) {
                    row.Group.Collapsed = collapsed;
                }

                // workaround, make the grid refresh properly
                Rows[0].Visible = !Rows[0].Visible;
                Rows[0].Visible = !Rows[0].Visible;
            } catch {
                MessageBox.Show(@"We had an error in SetGroupCollapse.");
            }
        }

        private void SetupColumns() {
            // clear all columns, this is a somewhat crude implementation
            // refinement may be welcome.
            Columns.Clear();

            // start filling the grid
            if(_dataSource == null) {
                return;
            }

            var list = _dataSource.Rows;
            if(list.Count <= 0) {
                return;
            }

            var enabledColumn = new DataGridViewCheckBoxColumn {
                DataPropertyName = "Enabled",
                HeaderText = @"Enabled",
                Name = "Enabled",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            var categoryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Category",
                HeaderText = @"Category",
                Name = "Category",
                Visible = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            var entryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Entry",
                HeaderText = @"Entry",
                Name = "Entry",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            var nameColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Name",
                HeaderText = @"Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            var deleteColumn = new DataGridViewButtonColumn {
                DataPropertyName = "Delete",
                Text = "Delete",
                HeaderText = @"Delete",
                Name = "Delete",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                UseColumnTextForButtonValue = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            Columns.Add(enabledColumn);
            Columns.Add(categoryColumn);
            Columns.Add(entryColumn);
            Columns.Add(nameColumn);
            Columns.Add(deleteColumn);
        }

        /// <summary>
        /// the fill grid method fills the grid with the data from the DataSourceManager
        /// It takes the grouping style into account, if it is set.
        /// </summary>
        private void FillGrid(IOutlookGridGroup groupingStyle) {
            OutlookGridRow row;

            Rows.Clear();

            // start filling the grid
            if(_dataSource == null)
                return;
            var list = _dataSource.Rows;
            if(list.Count <= 0) return;

            // this block is used of grouping is turned off
            // this will simply list all attributes of each object in the list
            if(groupingStyle == null) {
                foreach(DataSourceRow r in list) {
                    row = (OutlookGridRow)RowTemplate.Clone();
                    var cellNumber = 0;
                    // ReSharper disable once UnusedVariable
                    foreach(var val in r) {
                        cellNumber++;
                        if(cellNumber == 6) {
                            cellNumber = 1;
                        }

                        DataGridViewCell cell = null;
                        switch(cellNumber) {
                            case 1:
                                cell = new DataGridViewCheckBoxCell();
                                break;
                            case 2:
                            case 3:
                            case 4:
                                cell = new DataGridViewTextBoxCell();
                                break;
                            case 5:
                                cell = new DataGridViewButtonCell();
                                break;
                        }

                        if(cell == null) {
                            continue;
                        }

                        if(row != null) {
                            row.Cells.Add(cell);
                        }
                    }
                    if(row != null) {
                        Rows.Add(row);
                    }
                }
            }

            // this block is used when grouping is used
                // items in the list must be sorted, and then they will automatically be grouped
            else {
                IOutlookGridGroup groupCur = null;
                var counter = 0; // counts number of items in the group

                foreach(DataSourceRow r in list) {
                    row = (OutlookGridRow)RowTemplate.Clone();
                    if(groupingStyle.Column != null) {
                        var result = r[groupingStyle.Column.Index];
                        if(groupCur != null && groupCur.CompareTo(result) == 0) // item is part of the group
                        {
                            if(row != null) row.Group = groupCur;
                            counter++;
                        } else // item is not part of the group, so create new group
                        {
                            if(groupCur != null)
                                groupCur.ItemCount = counter;

                            groupCur = (IOutlookGridGroup)groupingStyle.Clone(); // init
                            groupCur.Value = result;
                            if(row != null) {
                                row.Group = groupCur;
                                row.IsGroupRow = true;
                                row.Height = groupCur.Height;
                                row.CreateCells(this, groupCur.Value);
                                Rows.Add(row);
                            }

                            // add content row after this
                            row = (OutlookGridRow)RowTemplate.Clone();
                            if(row != null) row.Group = groupCur;
                            counter = 1; // reset counter for next group
                        }
                    }

                    var cellNumber = 0;
                    foreach(var obj in r) {
                        cellNumber++;
                        if(cellNumber == 6) {
                            cellNumber = 1;
                        }

                        DataGridViewCell cell = null;
                        switch(cellNumber) {
                            case 1:
                                cell = new DataGridViewCheckBoxCell();
                                break;
                            case 2:
                            case 3:
                            case 4:
                                cell = new DataGridViewTextBoxCell();
                                break;
                            case 5:
                                cell = new DataGridViewButtonCell();
                                break;
                        }

                        if(cell == null) {
                            continue;
                        }

                        cell.Value = obj.ToString();
                        if(row != null) {
                            row.Cells.Add(cell);
                        }
                    }
                    if(row != null) {
                        Rows.Add(row);
                    }

                    if(groupCur != null) {
                        groupCur.ItemCount = counter;
                    }
                }
            }

        }
        #endregion Grid Fill functions
    }
    #endregion implementation of the OutlookGrid!
}
