// Originally contributed by Chinajade.
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.

#region Usings

using System;
using System.Diagnostics;
#endregion


namespace BuddyControlPanel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class ContractAnnotationAttribute : Attribute
    {
        public ContractAnnotationAttribute(string contract) { }
    }

    public class Contract
    {
        public delegate string ProvideStringDelegate();

        /// <summary>
        /// <para>This is an efficent poor man's mechanism for reporting contract violations in methods.</para>
        /// <para>If the provided ISCONTRACTOKAY evaluates to true, no action is taken.
        /// If ISCONTRACTOKAY is false, a diagnostic message--given by the STRINGPROVIDERDELEGATE--is emitted to the log, along with a stack trace.</para>
        /// <para>This emitted information can then be used to locate and repair the code misusing the interface.</para>
        /// <para>For convenience, this method returns the evaluation if ISCONTRACTOKAY.</para>
        /// <para>Notes:<list type="bullet">
        /// <item><description><para> * The interface is built in terms of a StringProviderDelegate,
        /// so we don't pay a performance penalty to build an error message that is not used
        /// when ISCONTRACTOKAY is true.</para></description></item>
        /// <item><description><para> * The .NET 4.0 Contract support is insufficient due to the way Buddy products
        /// dynamically compile parts of the project at run time.</para></description></item>
        /// </list></para>
        /// </summary>
        /// <param name="isContractOkay"></param>
        /// <param name="provideStringProviderDelegate"></param>
        /// <returns>the evaluation of the provided ISCONTRACTOKAY predicate delegate</returns>
        ///  30Jun2012-15:58UTC chinajade
        ///  NB: We could provide a second interface to ContractRequires() that is slightly more convenient for static string use.
        ///  But *please* don't!  If helps maintainers to not make mistakes if they see the use of this interface consistently
        ///  throughout the code.
        [ContractAnnotation("isContractOkay:false=>halt")]
        public static bool Requires(bool isContractOkay, ProvideStringDelegate provideStringProviderDelegate)
        {
            if (!isContractOkay)
            {
                // TODO: (Future enhancement) Build a string representation of isContractOkay if stringProviderDelegate is null
                string message = provideStringProviderDelegate() ?? "NO MESSAGE PROVIDED";
                var trace = new StackTrace(1);

                PluginLog.Error("[CONTRACT VIOLATION] {0}\nLocation:\n{1}", message, trace.ToString());
                throw new ContractException(message);
            }

            return isContractOkay;
        }

        [ContractAnnotation("isContractOkay:false=>halt")]
        public static bool Provides(bool isContractOkay, ProvideStringDelegate provideStringProviderDelegate)
        {
            if (!isContractOkay)
            {
                // TODO: (Future enhancement) Build a string representation of isContractOkay if stringProviderDelegate is null
                string message = provideStringProviderDelegate() ?? "NO MESSAGE PROVIDED";
                var trace = new StackTrace(1);

                PluginLog.Error("[CONTRACT VIOLATION] {0}\nLocation:\n{1}", message, trace.ToString());
                throw new ContractException(message);
            }

            return isContractOkay;
        }
    }


    public class ContractException : SystemException
    {
        public ContractException(string message = null)
            : base(message ?? string.Empty)
        {
            // empty   
        }
    }
}