using System.Windows.Forms;

namespace ObjectGatherer.Helpers {
    public class CustomAddRow {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        public CustomAddRow() { }

        public CustomAddRow(DataGridViewCheckBoxColumn enabled, string category, uint entry, string name, DataGridViewButtonColumn delete) {
            Enabled = enabled;
            Category = category;
            Entry = entry;
            Name = name;
            Delete = delete;
        }

        public DataGridViewCheckBoxColumn Enabled { get; set; }
        public string Category { get; set; }
        public uint Entry { get; set; }
        public string Name { get; set; }
        public DataGridViewButtonColumn Delete { get; set; }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
