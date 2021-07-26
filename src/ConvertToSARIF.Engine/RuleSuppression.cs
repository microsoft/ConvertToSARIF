// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis.Sarif;

namespace ConvertToSARIF.Engine
{
    public class RuleSuppression
    {

        /// <summary>
        /// The start offset of the rule suppression attribute (not where it starts to apply)
        /// </summary>
        public int StartAttributeLine
        {
            get;
            set;
        }

        /// <summary>
        /// The start offset of the rule suppression
        /// </summary>
        public int StartOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The end offset of the rule suppression
        /// </summary>
        public int EndOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the rule being suppressed
        /// </summary>
        public string RuleName
        {
            get;
            set;
        }

        /// <summary>
        /// ID of the violation instance
        /// </summary>
        public string RuleSuppressionID
        {
            get;
            set;
        }

        /// <summary>
        /// Scope of the rule suppression
        /// </summary>
        public string Scope
        {
            get;
            set;
        }

        /// <summary>
        /// Target of the rule suppression
        /// </summary>
        public string Target
        {
            get;
            set;
        }

        /// <summary>
        /// Returns error occurred in trying to parse the attribute
        /// </summary>
        public string Error
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the justification for the suppression
        /// </summary>
        public string Justification
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the kind of the suppression
        /// </summary>
        public SuppressionKind Kind
        {
            get;
            set;
        }

        public RuleSuppression()
        { }

        /// <summary>
        /// Constructs rule expression from rule name, id, start, end, startAttributeLine and justification
        /// </summary>
        /// <param name="ruleName"></param>
        /// <param name="ruleSuppressionID"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startAttributeLine"></param>
        /// <param name="justification"></param>
        public RuleSuppression(string ruleName, string ruleSuppressionID, int start, int end, int startAttributeLine, string justification,SuppressionKind kind)
        {
            RuleName = ruleName;
            RuleSuppressionID = ruleSuppressionID;
            StartOffset = start;
            EndOffset = end;
            StartAttributeLine = startAttributeLine;
            Justification = justification;
            Kind = kind;
        }
    }
}