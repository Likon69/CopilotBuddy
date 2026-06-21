using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace ObjectGatherer.Helpers {
    public class DataRowComparer : IComparer {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        readonly ListSortDirection _direction;
        readonly int _columnIndex;
        
        // ===========================================================
        // Constructors
        // ===========================================================

        public DataRowComparer(int columnIndex, ListSortDirection direction) {
            _columnIndex = columnIndex;
            _direction = direction;
        }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public int Compare(object x, object y) {
            var obj1 = (DataRow)x;
            var obj2 = (DataRow)y;
            return String.CompareOrdinal(obj1[_columnIndex].ToString(), obj2[_columnIndex].ToString()) * (_direction == ListSortDirection.Ascending ? 1 : -1);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
