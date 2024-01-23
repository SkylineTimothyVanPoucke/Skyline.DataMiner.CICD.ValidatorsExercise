﻿// <auto-generated>
// Do not change. Treat this as auto-generated code as this is converted from code in DataMiner.
// If there are software changes this allows easier comparison.
// </auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    class ConditionMember
    {
        private readonly List<ConditionMemberBlock> paBlocks;
        private readonly List<ConditionOperator> uiaLink;
        private readonly List<uint> uiaOtherMembers;

        public ConditionMember(string strData, List<string> saStrings, Action addConditionCanBeSimplifiedWarning)
        {
            bool operandExpected = true;
            bool isConstantExpression = true;

            this.StrData = strData;

            paBlocks = new List<ConditionMemberBlock>();
            uiaLink = new List<ConditionOperator>();
            uiaOtherMembers = new List<uint>();

            string[] saData = strData.Split(' ');

            for (int i = 0; i < saData.Length; i++)
            {
                string strBlock = saData[i];

                if (strBlock.Equals("+", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Plus);
                }
                else if (strBlock.Equals("-", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Minus);
                }
                else if (strBlock.Equals("*", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Multiply);
                }
                else if (strBlock.Equals("/", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Divide);
                }
                else if (strBlock.Equals("&", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.And);
                }
                else if (strBlock.Equals("|", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Or);
                }
                else if (strBlock.Equals("^", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.ExclusiveOr);
                }
                else if (strBlock.Equals("==", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.Equal);
                }
                else if (strBlock.Equals("!=", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.NotEqual);
                }
                else if (strBlock.Equals(">", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.GreaterThan);
                }
                else if (strBlock.Equals("<", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.LessThan);
                }
                else if (strBlock.Equals(">=", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.GreaterThanOrEqual);
                }
                else if (strBlock.Equals("<=", StringComparison.Ordinal))
                {
                    operandExpected = AddOperator(operandExpected, ConditionOperator.LessThanOrEqual);
                }
                else if (strBlock.Length > 0 && strBlock.StartsWith("#"))
                {
                    if (!operandExpected)
                    {
                        throw new InvalidConditionException("Missing operator or operand detected.");
                    }

                    operandExpected = false;

                    paBlocks.Add(null);
                    string value = strBlock.Substring(1, strBlock.Length - 2);
                    uint conditionId;
                    bool parsingSucceeded = UInt32.TryParse(value, out conditionId);

                    if(!parsingSucceeded)
                    {
                        throw new InvalidConditionException("Invalid formatted condition detected.");
                    }

                    uiaOtherMembers.Add(conditionId);
                    isConstantExpression = false;
                }
                else if (strBlock.Length > 0)
                {
                    if (!operandExpected)
                    {
                        throw new InvalidConditionException("Missing operator or operand detected.");
                    }

                    operandExpected = false;

                    var conditionMemberBlock = new ConditionMemberBlock(strBlock, saStrings);

                    if (conditionMemberBlock.ParameterID != null)
                    {
                        isConstantExpression = false;
                    }

                    paBlocks.Add(conditionMemberBlock);
                }
            }

            if(operandExpected)
            {
                throw new InvalidConditionException("Missing operator or operand detected.");
            }

            if (isConstantExpression)
            {
                addConditionCanBeSimplifiedWarning();
            }

            uiaLink.Insert(0, ConditionOperator.Undefined);
        }

        public string StrData { get; }

        public Variant Perform(Condition[] paConditions, IProtocolModel pProtocol)
        {
            bool bOk = false;

            int iOtherBlockCount = 0;
            Variant varSourceValue = new Variant();
            Variant varToCompareValue = new Variant();

            ConditionMemberBlock pBlock = null;
            Condition pCondition = null;

            Variant varTmpValue = new Variant();

            bool bComposed = false;
            bool bEquationPassed = false;

            var iEquation = ConditionOperator.Undefined;
            var iTmpEquation = ConditionOperator.Undefined;

            for (int i = 0; i < paBlocks.Count; i++)
            {
                pBlock = paBlocks[i];
                if (pBlock != null)
                {
                    varTmpValue = pBlock.GetValue(pProtocol);
                }
                else
                {
                    if ((uiaOtherMembers.Count - 1) >= iOtherBlockCount)
                    {
                        int iNr = (int)uiaOtherMembers[iOtherBlockCount];
                        if (iNr <= (paConditions.Length - 1))
                        {
                            pCondition = paConditions[iNr];

                            if (pCondition != null)
                            {
                                varTmpValue = pCondition.Perform(paConditions, pProtocol);

                                if (varTmpValue.Type == VariantType.VT_BOOL)
                                {
                                    bComposed = true;
                                    bOk = (bool)true;
                                }
                            }
                            else
                            {
                                bComposed = true;
                                bOk = false;
                            }
                        }
                    }

                    iOtherBlockCount++;
                }

                if (i > 0 && (uiaLink.Count - 1) >= i)
                {
                    iTmpEquation = uiaLink[i];

                    if (iTmpEquation == ConditionOperator.Equal
                        || iTmpEquation == ConditionOperator.NotEqual
                        || iTmpEquation == ConditionOperator.GreaterThan
                        || iTmpEquation == ConditionOperator.LessThan
                        || iTmpEquation == ConditionOperator.GreaterThanOrEqual
                        || iTmpEquation == ConditionOperator.LessThanOrEqual)
                    {
                        if (bEquationPassed)
                        {
                            //bEquationPassed = !bEquationPassed;
                            throw new InvalidConditionException("Multiple relational or equality operators were detected in a single condition member.");
                        }

                        iEquation = iTmpEquation;
                        bEquationPassed = true;
                    }
                    else
                    {
                        switch (iTmpEquation)
                        {
                            case ConditionOperator.Plus:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_BSTR)
                                            {
                                                varTmpValue.StringValue = pvarTmp.StringValue + varTmpValue.StringValue;
                                            }
                                            else if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                varTmpValue.DoubleValue += pvarTmp.DoubleValue;
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidConditionException("The addition operator ('+') must be used with operands of the same type.");
                                        }
                                    }

                                    break;
                                }
                            case ConditionOperator.Minus:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;
                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                varTmpValue.DoubleValue = pvarTmp.DoubleValue - varTmpValue.DoubleValue;
                                            }
                                            else
                                            {
                                                throw new InvalidConditionException("The subtraction operator ('-') must be used with two double operands.");
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidConditionException("The subtraction operator ('-') must be used with two double operands.");
                                        }
                                    }

                                    break;
                                }
                            case ConditionOperator.Multiply:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                varTmpValue.DoubleValue = pvarTmp.DoubleValue * varTmpValue.DoubleValue;
                                            }
                                            else
                                            {
                                                throw new InvalidConditionException("The multiplication operator ('*') must be used with two double operands.");
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidConditionException("The multiplication operator ('*') must be used with two double operands.");
                                        }
                                    }
                                    break;
                                }
                            case ConditionOperator.Divide:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                varTmpValue.DoubleValue = pvarTmp.DoubleValue / varTmpValue.DoubleValue;
                                            }
                                            else
                                            {
                                                throw new InvalidConditionException("The division operator ('/') must be used with two double operands.");
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidConditionException("The division operator ('/') must be used with two double operands.");
                                        }
                                    }
                                    break;
                                }
                            case ConditionOperator.And:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                int ivarTmpVal = Convert.ToInt32(Math.Floor(pvarTmp.DoubleValue.Value));
                                                int iTmpValueVal = Convert.ToInt32(Math.Floor(varTmpValue.DoubleValue.Value));
                                                iTmpValueVal = ivarTmpVal & iTmpValueVal;
                                                varTmpValue.DoubleValue = iTmpValueVal;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case ConditionOperator.Or:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                int ivarTmpVal = Convert.ToInt32(Math.Floor(pvarTmp.DoubleValue.Value));
                                                int iTmpValueVal = Convert.ToInt32(Math.Floor(varTmpValue.DoubleValue.Value));
                                                iTmpValueVal = ivarTmpVal | iTmpValueVal;
                                                varTmpValue.DoubleValue = iTmpValueVal;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case ConditionOperator.ExclusiveOr:
                                {
                                    Variant pvarTmp = bEquationPassed ? varToCompareValue : varSourceValue;

                                    if (pvarTmp != null)
                                    {
                                        if (varTmpValue.Type == pvarTmp.Type)
                                        {
                                            if (varTmpValue.Type == VariantType.VT_R8)
                                            {
                                                int ivarTmpVal = Convert.ToInt32(Math.Floor(pvarTmp.DoubleValue.Value));
                                                int iTmpValueVal = Convert.ToInt32(Math.Floor(varTmpValue.DoubleValue.Value));
                                                iTmpValueVal = ivarTmpVal ^ iTmpValueVal;
                                                varTmpValue.DoubleValue = iTmpValueVal;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }

                if (i == 0 || !bEquationPassed)
                {
                    varSourceValue.Type = varTmpValue.Type;
                    varSourceValue.StringValue = varTmpValue.StringValue;
                    varSourceValue.DoubleValue = varTmpValue.DoubleValue;
                    varSourceValue.BooleanValue = varTmpValue.BooleanValue;
                }

                if (bEquationPassed)
                {
                    varToCompareValue.Type = varTmpValue.Type;
                    varToCompareValue.StringValue = varTmpValue.StringValue;
                    varToCompareValue.DoubleValue = varTmpValue.DoubleValue;
                    varToCompareValue.BooleanValue = varTmpValue.BooleanValue;
                }

                varTmpValue = new Variant();
            }

            if (!bComposed && bEquationPassed)
            {
                // compare the two variants
                if (varSourceValue.Type == VariantType.VT_NULL || varToCompareValue.Type == VariantType.VT_NULL)
                {
                    if (varSourceValue.Type == VariantType.VT_NULL && varToCompareValue.Type == VariantType.VT_NULL)
                    {
                        bOk = true;
                    }
                    else if (varSourceValue.Type == VariantType.VT_NULL)
                    {
                        if ((paBlocks.Count - 1) >= 1)
                        {
                            pBlock = paBlocks[1];
                            if (pBlock != null)
                                bOk = pBlock.IsEmpty();
                        }
                    }
                    else if (varToCompareValue.Type == VariantType.VT_NULL)
                    {
                        if ((paBlocks.Count - 1) >= 0)
                        {
                            pBlock = paBlocks[0];
                            if (pBlock != null)
                                bOk = pBlock.IsEmpty();
                        }
                    }

                    switch (iEquation)
                    {
                        case ConditionOperator.NotEqual:
                            bOk = !bOk;
                            break;
                    }
                }
                else if (varSourceValue.Type == varToCompareValue.Type && varSourceValue.Type == VariantType.VT_BSTR)
                {
                    switch (iEquation)
                    {
                        case ConditionOperator.Equal:
                            {
                                bOk = String.Equals(varSourceValue.StringValue, varToCompareValue.StringValue, StringComparison.OrdinalIgnoreCase);
                                break;
                            }
                        case ConditionOperator.NotEqual:
                            {
                                bOk = !String.Equals(varSourceValue.StringValue, varToCompareValue.StringValue, StringComparison.OrdinalIgnoreCase);
                                break;
                            }
                        case ConditionOperator.GreaterThan:
                            {
                                bOk = String.Compare(varSourceValue.StringValue, varToCompareValue.StringValue) > 0 ? true : false;
                                break;
                            }
                        case ConditionOperator.GreaterThanOrEqual:
                            {
                                bOk = String.Compare(varSourceValue.StringValue, varToCompareValue.StringValue) >= 0 ? true : false;
                                break;
                            }
                        case ConditionOperator.LessThan:
                            {
                                bOk = String.Compare(varSourceValue.StringValue, varToCompareValue.StringValue) < 0 ? true : false;
                                break;
                            }
                        case ConditionOperator.LessThanOrEqual:
                            {
                                bOk = String.Compare(varSourceValue.StringValue, varToCompareValue.StringValue) <= 0 ? true : false;
                                break;
                            }
                    }
                }
                else
                {
                    double dblSource = 0, dblToCmp = 0;
                    if (varSourceValue.Type == VariantType.VT_BSTR && varToCompareValue.Type == VariantType.VT_R8)
                    {
                        dblSource = GetDoubleValue(varSourceValue.StringValue);
                        dblToCmp = varToCompareValue.DoubleValue.Value;
                    }
                    else if (varSourceValue.Type == VariantType.VT_R8 && varToCompareValue.Type == VariantType.VT_BSTR)
                    {
                        dblToCmp = GetDoubleValue(varToCompareValue.StringValue);
                        dblSource = varSourceValue.DoubleValue.Value;
                    }
                    else if (varSourceValue.Type == VariantType.VT_R8 && varToCompareValue.Type == VariantType.VT_R8)
                    {
                        dblToCmp = varToCompareValue.DoubleValue.Value;
                        dblSource = varSourceValue.DoubleValue.Value;
                    }

                    switch (iEquation)
                    {
                        case ConditionOperator.Equal:
                            {
                                bOk = (dblSource == dblToCmp);
                                break;
                            }
                        case ConditionOperator.NotEqual:
                            {
                                bOk = (dblSource != dblToCmp);
                                break;
                            }
                        case ConditionOperator.GreaterThan:
                            {
                                bOk = (dblSource > dblToCmp);
                                break;
                            }
                        case ConditionOperator.GreaterThanOrEqual:
                            {
                                bOk = (dblSource >= dblToCmp);
                                break;
                            }
                        case ConditionOperator.LessThan:
                            {
                                bOk = (dblSource < dblToCmp);
                                break;
                            }
                        case ConditionOperator.LessThanOrEqual:
                            {
                                bOk = (dblSource <= dblToCmp);
                                break;
                            }
                    }
                }
            }
            else if (!bEquationPassed)
            {
                return varSourceValue;
            }

            Variant bRet = new Variant();
            bRet.Type = VariantType.VT_BOOL;
            bRet.BooleanValue = bOk;

            return bRet;
        }

        private static double GetDoubleValue(string value)
        {
            if(value == String.Empty)
            {
                throw new InvalidConditionException("Unexpected empty string used in combination with other operand that is a double.");
            }
            else
            {
                double returnValue;
                bool parsingSucceeded = Double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out returnValue);

                if(!parsingSucceeded)
                {
                    throw new InvalidConditionException("Expected a string representing a double or an empty string. Found: '"+value+"'.");
                }

                return returnValue;
            }
        }

        private bool AddOperator(bool operandExpected, ConditionOperator conditionOperator)
        {
            if (operandExpected)
            {
                throw new InvalidConditionException("Missing operator or operand detected.");
            }

            uiaLink.Add(conditionOperator);
            return true;
        }
    }
}