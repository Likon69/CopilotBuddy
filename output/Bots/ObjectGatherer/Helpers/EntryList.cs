using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ObjectGatherer.GUI;
using Styx.Helpers;

namespace ObjectGatherer.Helpers {
    public class EntryList {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static List<uint> IdList = new List<uint>();
        public static List<uint> SmallTimber = new List<uint>();
        public static List<uint> MediumTimber = new List<uint>();
        public static List<uint> LargeTimber = new List<uint>();

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Add(uint entry) {
            try {
                IdList.Add(entry);
            } catch(Exception ex) {
                CustomLog.Diagnostic("Exception in adding to IdList : {0}", ex);
            }
        }

        public static void Remove(uint entry) {
            try {
                IdList.Remove(entry);
            } catch(Exception ex) {
                CustomLog.Diagnostic("Exception in removing from IdList : {0}", ex);
            }
        }

        public static void Populate() {
            PopulateTimbers();
            if(File.Exists(ObjectGathererGUI.SettingsFilePath)) {
                foreach(var item in XDocument.Load(ObjectGathererGUI.SettingsFilePath).Descendants("Objects")) {
                    var enabled = item.Element("Enabled");
                    var entry = item.Element("Entry");
                    if(enabled == null || entry == null) {
                        continue;
                    }

                    if(!bool.Parse(enabled.Value)) {
                        continue;
                    }

                    Add(entry.Value.ToUInt32());
                }
            } else {
                Add(214388);
                Add(210565);
                Add(186633);
                Add(186634);
                Add(210894);
                Add(214458);
                Add(214985);
                Add(213363);
                Add(213364);
                Add(213366);
                Add(213368);
                Add(213649);
                Add(213653);
                Add(213741);
                Add(213742);
                Add(213748);
                Add(213749);
                Add(213750);
                Add(213751);
                Add(213765);
                Add(213768);
                Add(213771);
                Add(213782);
                Add(213793);
                Add(213842);
                Add(213844);
                Add(213845);
                Add(213956);
                Add(213959);
                Add(213960);
                Add(213962);
                Add(213964);
                Add(213966);
                Add(213967);
                Add(213968);
                Add(213969);
                Add(213970);
                Add(213972);
                Add(214340);
                Add(214438);
                Add(214439);
                Add(185915);
                Add(214945);
                Add(176944);
                Add(179697);
                Add(203090);
                Add(207472);
                Add(207473);
                Add(207474);
                Add(207475);
                Add(207476);
                Add(207477);
                Add(207478);
                Add(207479);
                Add(207480);
                Add(207484);
                Add(207485);
                Add(207486);
                Add(207487);
                Add(207488);
                Add(207489);
                Add(207492);
                Add(207493);
                Add(207494);
                Add(207495);
                Add(207496);
                Add(207497);
                Add(207498);
                Add(207500);
                Add(207507);
                Add(207512);
                Add(207513);
                Add(207517);
                Add(207518);
                Add(207519);
                Add(207520);
                Add(207521);
                Add(207522);
                Add(207523);
                Add(207524);
                Add(207528);
                Add(207529);
                Add(207533);
                Add(207534);
                Add(207535);
                Add(207540);
                Add(207542);
                Add(213362);
                Add(213650);
                Add(213769);
                Add(213770);
                Add(213774);
                Add(213961);
                Add(214325);
                Add(214407);
                Add(214337);
                Add(214337);
                Add(218593);
                Add(50409);
                Add(50410);
                Add(233604);
                Add(233922);
                Add(234021);
                Add(234080);
                Add(234097);
                Add(234109);
                Add(234110);
                Add(234122);
                Add(234126);
                Add(234193);
                Add(234197);
                Add(237727);
                Add(233634);
                Add(234000);
                Add(234098);
                Add(234111);
                Add(234119);
                Add(234123);
                Add(234127);
                Add(234194);
                Add(234196);
                Add(234198);
                Add(234022);
                Add(233635);
                Add(234023);
                Add(234007);
                Add(234099);
                Add(234120);
                Add(234124);
                Add(234128);
                Add(234195);
                Add(234199);
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static void PopulateTimbers() {
            SmallTimber.Clear();
            SmallTimber.Add(233604);
            SmallTimber.Add(233922);
            SmallTimber.Add(234021);
            SmallTimber.Add(234080);
            SmallTimber.Add(234097);
            SmallTimber.Add(234109);
            SmallTimber.Add(234110);
            SmallTimber.Add(234122);
            SmallTimber.Add(234126);
            SmallTimber.Add(234193);
            SmallTimber.Add(234197);
            SmallTimber.Add(237727);
            MediumTimber.Clear();
            MediumTimber.Add(233634);
            MediumTimber.Add(234000);
            MediumTimber.Add(234098);
            MediumTimber.Add(234111);
            MediumTimber.Add(234119);
            MediumTimber.Add(234123);
            MediumTimber.Add(234127);
            MediumTimber.Add(234194);
            MediumTimber.Add(234196);
            MediumTimber.Add(234198);
            MediumTimber.Add(234022);
            LargeTimber.Clear();
            LargeTimber.Add(233635);
            LargeTimber.Add(234023);
            LargeTimber.Add(234007);
            LargeTimber.Add(234099);
            LargeTimber.Add(234120);
            LargeTimber.Add(234124);
            LargeTimber.Add(234128);
            LargeTimber.Add(234195);
            LargeTimber.Add(234199);
        }
    }
}
