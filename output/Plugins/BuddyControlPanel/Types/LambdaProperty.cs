// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;

// ReSharper disable CheckNamespace
#endregion


namespace BuddyControlPanel
{
    public class LambdaProperty<T>
    {
        public LambdaProperty(string name, Func<T> getter, Action<T> setter = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(name), () => "name may not be null or empty.");
            Contract.Requires((getter != null) || (setter != null), () => "Getter and Setter may not both be null.");

            Name = name;
            Getter = getter ?? NullGetter;
            Setter = setter ?? NullSetter;
        }

        public string Name { get; private set; }
        public Func<T> Getter { get; private set; }
        public Action<T> Setter { get; private set; }

        private static T NullGetter()
        {
            return default(T);
        }

        private static void NullSetter(T unused)
        {
            /*empty*/
        }
    }


    public class LambdaProperty<T1, T2>
    {
        public LambdaProperty(string name, Func<T1, T2> getter, Action<T1, T2> setter = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(name), () => "name may not be null or empty.");
            Contract.Requires((getter != null) || (setter != null), () => "Getter and Setter may not both be null.");

            Name = name;
            Getter = getter ?? NullGetter;
            Setter = setter ?? NullSetter;
        }

        public string Name { get; private set; }
        public Func<T1, T2> Getter { get; private set; }
        public Action<T1, T2> Setter { get; private set; }

        private static T2 NullGetter(T1 unused)
        {
            return default(T2);
        }

        private static void NullSetter(T1 unused1, T2 unused2)
        {
            /*empty*/
        }
    }
}