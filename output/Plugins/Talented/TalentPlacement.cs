namespace Talented
{
    public class TalentPlacement
    {
        public int Tab;
        public int Index;
        public int Count;
        public string Name;

        public TalentPlacement(int tab, int index, int count)
        {
            Tab = tab;
            Index = index;
            Count = count;
        }

        public TalentPlacement(int tab, int index, int count, string name)
        {
            Tab = tab;
            Index = index;
            Count = count;
            Name = name;
        }

        public TalentPlacement(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override string ToString()
        {
            return string.Format("{0} Count:{1}", Name, Count);
        }
    }
}
