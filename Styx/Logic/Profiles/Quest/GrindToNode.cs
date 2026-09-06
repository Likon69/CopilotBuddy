using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;


namespace Styx.Logic.Profiles.Quest
{
    /// <summary>
    /// Node for grinding to a specific level.
    /// </summary>
    public class GrindToNode : OrderNode
    {
        public GrindToNode(float level, string conditionText, XElement element)
            : base(OrderNodeType.GrindTo, element)
        {
            Level = level;
            ConditionText = conditionText;
            Condition = conditionText == null ? null : DelayCompiledExpression.Condition(conditionText);
        }

        public GrindToNode(float level, XElement element)
            : this(level, null, element)
        {
        }

        /// <summary>
        /// Target level to grind to.
        /// </summary>
        public float Level { get; private set; }

        /// <summary>
        /// The condition text as written in the profile.
        /// </summary>
        public string ConditionText { get; private set; }

        /// <summary>
        /// Condition expression for grinding, null when the node grinds to a level instead.
        /// </summary>
        [CompileExpression]
        public DelayCompiledExpression<Func<bool>> Condition { get; private set; }

        /// <summary>
        /// Goal text to display while grinding.
        /// </summary>
        public string GoalText { get; private set; }

        public override string ToString()
        {
            return $"[GrindToNode Level: {Level} GoalText: {GoalText}]";
        }

        public new static GrindToNode FromXml(XElement element)
        {
            // Get goal text
            var goalTextAttr = element.Attributes()
                .FirstOrDefault(a => a.Name.LocalName.Equals("goaltext", StringComparison.OrdinalIgnoreCase));
            string goalText = goalTextAttr?.Value ?? "";

            // Check for condition attribute
            var conditionAttr = element.Attributes()
                .FirstOrDefault(a => a.Name.LocalName.Equals("condition", StringComparison.OrdinalIgnoreCase));
            if (conditionAttr != null)
            {
                return new GrindToNode(-1f, conditionAttr.Value, element) { GoalText = goalText };
            }

            // Check for level attribute
            var levelAttr = element.Attributes()
                .FirstOrDefault(a => a.Name.LocalName.Equals("level", StringComparison.OrdinalIgnoreCase));
            if (levelAttr != null)
            {
                if (!float.TryParse(levelAttr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var level))
                    throw new ProfileAttributeExpectedException<float>(levelAttr);
                return new GrindToNode(level, element) { GoalText = goalText };
            }

            throw new ProfileException("You need at least one level or condition attribute in GrindToNode!");
        }
    }
}
