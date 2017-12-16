﻿using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using Components.Aphid.Parser.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Components.Aphid.Interpreter
{
    public partial class AphidInterpreter
    {
        private const string
            _return = "$r",
            _imports = "$imports",
            _implicitArg = "$_",
            _implicitArgs = "$args",
            _framesKey = "$frames",
            _block = "$block",
            _scope = "$scope",
            _parent = "$parent";

        private bool _createLoader, _isReturning, _isContinuing, _isBreaking;

        private Stack<AphidFrame> _frames = new Stack<AphidFrame>(new[] 
        { 
            new AphidFrame(null, "[Entrypoint]"),
        });

        private AphidObjectEqualityComparer _comparer = new AphidObjectEqualityComparer();

        private AphidAssemblyBuilder _asmBuilder = new AphidAssemblyBuilder();

        private TextWriter _out = Console.Out;

        public TextWriter Out
        {
            get { return _out; }
            set { _out = value; }
        }

        public Func<string, string> OutFilter { get; set; }

        public Func<string, string> GatorEmitFilter { get; set; }

        private AphidLoader _loader;

        public AphidLoader Loader
        {
            get { return _loader; }
        }

        private AphidObject _currentScope;

        public AphidObject CurrentScope
        {
            get { return _currentScope; }
        }

        private Dictionary<AphidTokenType, AphidFunction> _binaryOperatorTable = new Dictionary<AphidTokenType, AphidFunction>();

        public AphidInterpreter(bool createLoader = true)
        {
            _createLoader = createLoader;
            _currentScope = new AphidObject();
            _currentScope.Add(_scope, _currentScope);
            _currentScope.Add(_parent, _currentScope.Parent);
            Init();            
        }

        public AphidInterpreter(AphidObject currentScope)
        {
            _currentScope = currentScope;

            AphidObject scope;

            if (!_currentScope.TryGetValue(_scope, out scope))
            {
                _currentScope.Add(_scope, _currentScope);
                _currentScope.Add(_parent, _currentScope.Parent);
            }

            Init();
        }

        private void Init()
        {
            if (_createLoader)
            {
                _loader = new AphidLoader(this);
            }

            if (!_currentScope.ContainsKey(_framesKey))
            {
                _currentScope.Add(_framesKey, new AphidObject(_frames));
            }
        }

        private AphidRuntimeException CreateUndefinedMemberException(AphidExpression expression, AphidExpression memberExpression)
        {
            return new AphidRuntimeException(
                    "Undefined member {0} in expression {1}",
                    memberExpression,
                    expression);
        }

        private AphidRuntimeException CreateUnaryOperatorException(UnaryOperatorExpression expression)
        {
            return new AphidRuntimeException("Unknown operator {0} in expression {1}.", expression.Operator, expression);
        }

        public List<string> GetImports()
        {
            AphidObject imports = null;

            if (_currentScope.TryResolve(_imports, out imports))
            {
                return (List<string>)imports.Value;
            }
            else
            {
                var list = new List<string>();
                _currentScope.Add(_imports, new AphidObject(list));

                return list;
            }
        }

        public void AddImport(string name)
        {
            var imports = GetImports();

            if (!imports.Contains(name))
            {
                imports.Add(name);
            }
        }

        public AphidObject GetReturnValue()
        {
            AphidObject retVal = null;

            if (_currentScope.TryResolve(_return, out retVal))
            {
                _currentScope.Remove(_return);
            }

            return retVal;
        }

        private void SetReturnValue(AphidObject obj)
        {
            _currentScope.Add(_return, obj);
        }

        public void EnterChildScope()
        {
            _currentScope = new AphidObject(null, _currentScope);
            _currentScope.Add(_scope, _currentScope);
            _currentScope.Add(_parent, _currentScope.Parent);
        }

        public bool LeaveChildScope(bool bubbleReturnValue = false)
        {
            if (bubbleReturnValue)
            {
                var ret = GetReturnValue();
                _currentScope = _currentScope.Parent;

                if (ret != null)
                {
                    SetReturnValue(ret);

                    return true;
                }
            }
            else
            {
                _currentScope = _currentScope.Parent;
            }

            return false;
        }

        private void InterpretChild(List<AphidExpression> block)
        {
            EnterChildScope();
            Interpret(block, false);
            LeaveChildScope(true);
        }

        private AphidObject CompareDecimals(BinaryOperatorExpression expression, Func<decimal, decimal, bool> equal)
        {

            return new AphidObject(
                equal(
                    Convert.ToDecimal(ValueHelper.Unwrap(InterpretExpression(expression.LeftOperand))),
                    Convert.ToDecimal(ValueHelper.Unwrap(InterpretExpression(expression.RightOperand)))));
        }

        public void WriteOut(string text)
        {
            if (_out != null)
            {
                if (OutFilter != null)
                {
                    text = OutFilter(text);
                }

                _out.Write(text);
            }
        }

        private AphidObject InterpretAndExpression(BinaryOperatorExpression expression)
        {
            var left = (bool)ValueHelper.Unwrap(InterpretExpression(expression.LeftOperand));

            if (!left)
            {
                return new AphidObject(false);
            }
            else
            {
                return new AphidObject((bool)ValueHelper.Unwrap(InterpretExpression(expression.RightOperand)));
            }
        }

        private AphidObject InterpretOrExpression(BinaryOperatorExpression expression)
        {
            var left = (bool)ValueHelper.Unwrap(InterpretExpression(expression.LeftOperand));

            if (left)
            {
                return new AphidObject(true);
            }
            else
            {
                return new AphidObject((bool)ValueHelper.Unwrap(InterpretExpression(expression.RightOperand)));
            }
        }

        private AphidObject InterpretEqualityExpression(BinaryOperatorExpression expression)
        {
            var left = (AphidObject)InterpretExpression(expression.LeftOperand);
            var right = (AphidObject)InterpretExpression(expression.RightOperand);

            bool val;

            if (left == null)
            {
                throw CreateUndefinedMemberException(expression, expression.LeftOperand);
            }
            else if (right == null)
            {
                throw CreateUndefinedMemberException(expression, expression.RightOperand);
            }
            else
            {
                if (left.Value != null && right.Value != null)
                {
                    Type leftType = left.Value.GetType(), rightType = right.Value.GetType();
                    val = InterpretEqualityExpression(left.Value, leftType, right.Value, rightType);
                }
                else if (left.Value != null)
                {
                    val = left.Value.Equals(right.Value);
                }
                else if (right.Value != null)
                {
                    val = right.Value.Equals(left.Value);
                }
                else
                {
                    val = left.Count == 0 && right.Count == 0;
                }
            }

            if (expression.Operator == AphidTokenType.NotEqualOperator)
            {
                val = !val;
            }

            return new AphidObject(val);
        }

        private AphidObject InterpretMemberInteropExpression(object lhs, BinaryOperatorExpression expression, bool returnRef = false)
        {
            if (expression.RightOperand.Type != AphidExpressionType.IdentifierExpression)
            {
                throw new AphidRuntimeException("Invalid member interop access.");
            }

            var members = GetMembers(lhs, expression);

            if (members.Length == 1)
            {
                var propInfo = members.First() as PropertyInfo;

                if (propInfo != null)
                {

                    return ValueHelper.Wrap(
                        !returnRef ?
                            propInfo.GetValue(lhs) :
                            new AphidInteropReference(lhs, propInfo));
                }

                var fieldInfo = members.First() as FieldInfo;

                if (fieldInfo != null)
                {
                    return ValueHelper.Wrap(
                        !returnRef ?
                        fieldInfo.GetValue(lhs) :
                        new AphidInteropReference(lhs, fieldInfo));
                }
            }

            if (!members.Any())
            {
                throw new AphidRuntimeException(
                    "Could not find property '{0}'",
                    expression.RightOperand.ToIdentifier().ToIdentifier());
            }

            return ValueHelper.Wrap(new AphidInteropMember(lhs, members));
        }

        private MemberInfo[] GetMembers(object target, BinaryOperatorExpression expression)
        {
            MemberInfo[] members;

            if (target != null)
            {
                var memberName = expression.RightOperand.ToIdentifier().Identifier;

                members = target
                    .GetType()
                    .GetMembers()
                    .Where(x => x.Name == memberName)
                    .ToArray();
            }
            else
            {
                var path = FlattenPath(expression);
                var type = InteropTypeResolver.ResolveType(GetImports(), path);
                var member = path.Last();

                members = type
                    .GetMembers(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.Name == member)
                    .ToArray();
            }

            return members;
        }

        private object InterpretMemberExpression(BinaryOperatorExpression expression, bool returnRef = false)
        {
            var obj = InterpretExpression(expression.LeftOperand) as AphidObject;

            if (obj != null && !obj.IsAphidType())
            {
                return InterpretMemberInteropExpression(obj.Value, expression, returnRef);
            }

            string key;

            if (expression.RightOperand is IdentifierExpression)
            {
                key = (expression.RightOperand as IdentifierExpression).Identifier;
            }
            else if (expression.RightOperand is StringExpression)
            {
                key = (string)ValueHelper.Unwrap(InterpretStringExpression(expression.RightOperand as StringExpression));
            }
            else if (expression.RightOperand is DynamicMemberExpression)
            {
                var memberExp = expression.RightOperand as DynamicMemberExpression;
                key = ValueHelper.Unwrap(InterpretExpression(memberExp.MemberExpression)).ToString();
            }
            else
            {
                throw new AphidRuntimeException("Unexpected expression {0}", expression.RightOperand);
            }

            if (returnRef)
            {
                return new AphidRef() { Name = key, Object = obj };
            }
            else
            {
                AphidObject val;

                if (obj == null)
                {
                    return InterpretMemberInteropExpression(null, expression, returnRef);
                }
                else if (!obj.TryResolve(key, out val))
                {
                    var t = obj.GetValueType();
                    var extKey = TypeExtender.GetName(t, key);

                    if (!_currentScope.TryResolve(extKey, out val))
                    {
                        return InterpretMemberInteropExpression(obj.Value, expression, returnRef);
                    }

                    var function = ((AphidFunction)val.Value).Clone();
                    function.ParentScope = new AphidObject { Parent = _currentScope };
                    function.ParentScope.Add(function.Args[0], obj);
                    function.Args = function.Args.Skip(1).ToArray();
                    val = new AphidObject(function);
                }

                return val;
            }
        }

        private object InterpetAssignmentExpression(BinaryOperatorExpression expression, bool returnRef = false)
        {
            var value = InterpretExpression(expression.RightOperand);
            var value2 = value as AphidObject;
            var idExp = expression.LeftOperand as IdentifierExpression;
            ArrayAccessExpression arrayAccessExp;

            if (idExp != null)
            {
                var id = idExp.Identifier;
                var destObj = InterpretIdentifierExpression(idExp);

                if (destObj == null)
                {
                    destObj = new AphidObject();

                    _currentScope.Add(id, destObj);
                }
                else
                {
                    destObj.Clear();
                }

                if (value2 != null)
                {
                    destObj.Value = value2.Value;
                    destObj.Parent = value2.Parent;

                    foreach (var x in value2)
                    {
                        destObj.Add(x.Key, x.Value);
                    }
                }
                else
                {
                    destObj.Value = value;
                }
            }
            else if ((arrayAccessExp = expression.LeftOperand as ArrayAccessExpression) != null)
            {
                var targetObj = InterpretExpression(arrayAccessExp.ArrayExpression);
                var targetObjUnwrapped = ValueHelper.Unwrap(targetObj);

                var keyObj = ValueHelper.Unwrap(
                    InterpretExpression(arrayAccessExp.KeyExpression));

                Array targetArray;
                List<AphidObject> targetAphidList;
                
                if ((targetArray = targetObjUnwrapped as Array) != null)
                {
                    targetArray.SetValue(
                        Convert.ChangeType(
                            ValueHelper.Unwrap(value),
                            targetArray.GetType().GetElementType()),
                        Convert.ToInt32(keyObj));
                }
                else if ((targetAphidList = targetObjUnwrapped as List<AphidObject>) != null)
                {
                    if (value2.Count == 0 && value2.Value != null)
                    {
                        value = value2 = new AphidObject(value2.Value);
                    }

                    targetAphidList[Convert.ToInt32(keyObj)] = value2;
                }
                else
                {
                    var targetType = targetObjUnwrapped.GetType();

                    if (targetType
                        .GetInterfaces()
                        .Any(x => x.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        var index = targetType
                            .GetProperties()
                            .Select(x => new { Property = x, Params = x.GetIndexParameters() })
                            .Single(x =>
                                x.Params.Length == 1 &&
                                x.Params.First().ParameterType == typeof(int));

                        index.Property.SetValue(
                            targetObjUnwrapped,
                            Convert.ChangeType(
                                ValueHelper.Unwrap(value),
                                index.Property.PropertyType),
                            new object[] { Convert.ToInt32(keyObj) });
                    }
                    else
                    {
                        throw new AphidRuntimeException("Could not set value by index.");
                    }
                }
            }
            else
            {
                var obj = InterpretBinaryOperatorExpression(expression.LeftOperand as BinaryOperatorExpression, true);

                var interopRef = ValueHelper.Unwrap(obj) as AphidInteropReference;

                if (interopRef != null)
                {
                    var v = ValueHelper.Unwrap(value);

                    if (interopRef.Field != null)
                    {
                        interopRef.Field.SetValue(
                            interopRef.Object,
                            AphidTypeConverter.Convert(interopRef.Field.FieldType, v));
                    }
                    else
                    {
                        interopRef.Property.SetValue(
                            interopRef.Object,
                            v != null ? 
                                AphidTypeConverter.Convert(interopRef.Property.PropertyType, v) :
                                null);
                    }

                    return value;
                }

                var objRef = obj as AphidRef;

                if (objRef.Object == null)
                {
                    throw new AphidRuntimeException("Undefined variable {0}", expression.LeftOperand);
                }
                else if (objRef.Object.ContainsKey(objRef.Name))
                {
                    objRef.Object[objRef.Name].Value = ValueHelper.Unwrap(value);
                    //ValueHelper.Wrap(value).CopyTo(objRef.Object[objRef.Name]);
                }
                else
                {
                    objRef.Object.Add(objRef.Name, ValueHelper.Wrap(value));
                }
            }

            return value;
        }

        private AphidObject InterprentOperatorAndAssignmentExpression(
            Func<AphidObject, AphidObject, AphidObject> performOperation,
            BinaryOperatorExpression expression)
        {
            var left = InterpretExpression(expression.LeftOperand) as AphidObject;
            var value = performOperation(left, InterpretExpression(expression.RightOperand) as AphidObject);
            left.Value = value.Value;

            return left;
        }

        private object InterpretBinaryOperatorExpression(BinaryOperatorExpression expression, bool returnRef = false)
        {
            switch (expression.Operator)
            {
                case AphidTokenType.AdditionOperator:
                    return OperatorHelper.Add(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.MinusOperator:
                    return OperatorHelper.Subtract(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.MultiplicationOperator:
                    return OperatorHelper.Multiply(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.DivisionOperator:
                    return OperatorHelper.Divide(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.MemberOperator:
                    return InterpretMemberExpression(expression, returnRef);

                case AphidTokenType.AssignmentOperator:
                    return InterpetAssignmentExpression(expression, returnRef);

                case AphidTokenType.PlusEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Add, expression);

                case AphidTokenType.MinusEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Subtract, expression);

                case AphidTokenType.MultiplicationEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Multiply, expression);

                case AphidTokenType.DivisionEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Divide, expression);

                case AphidTokenType.ModulusEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Mod, expression);

                case AphidTokenType.BinaryAndEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.BinaryAnd, expression);

                case AphidTokenType.OrEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.BinaryOr, expression);

                case AphidTokenType.XorEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.Xor, expression);

                case AphidTokenType.ShiftLeftEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.BinaryShiftLeft, expression);

                case AphidTokenType.ShiftRightEqualOperator:
                    return InterprentOperatorAndAssignmentExpression(OperatorHelper.BinaryShiftRight, expression);

                case AphidTokenType.NotEqualOperator:
                case AphidTokenType.EqualityOperator:
                    return InterpretEqualityExpression(expression);

                case AphidTokenType.LessThanOperator:
                    return CompareDecimals(expression, (x, y) => x < y);

                case AphidTokenType.LessThanOrEqualOperator:
                    return CompareDecimals(expression, (x, y) => x <= y);

                case AphidTokenType.GreaterThanOperator:
                    return CompareDecimals(expression, (x, y) => x > y);

                case AphidTokenType.GreaterThanOrEqualOperator:
                    return CompareDecimals(expression, (x, y) => x >= y);

                case AphidTokenType.AndOperator:
                    return InterpretAndExpression(expression);

                case AphidTokenType.OrOperator:
                    return InterpretOrExpression(expression);

                case AphidTokenType.ModulusOperator:
                    return OperatorHelper.Mod(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.ShiftLeft:
                    return OperatorHelper.BinaryShiftLeft(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.ShiftRight:
                    return OperatorHelper.BinaryShiftRight(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.BinaryAndOperator:
                    return OperatorHelper.BinaryAnd(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.BinaryOrOperator:
                    return OperatorHelper.BinaryOr(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.XorOperator:
                    return OperatorHelper.Xor(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.PipelineOperator:
                    return InterpretCallExpression(new CallExpression(expression.RightOperand, expression.LeftOperand));

                case AphidTokenType.RangeOperator:
                    return OperatorHelper.Range(
                        InterpretExpression(expression.LeftOperand) as AphidObject,
                        InterpretExpression(expression.RightOperand) as AphidObject);

                case AphidTokenType.SelectOperator:
                    var func = ValueHelper.Unwrap(InterpretExpression(expression.RightOperand));

                    return ((IEnumerable<object>)ValueHelper
                        .Unwrap(InterpretExpression(expression.LeftOperand)))
                        .Select(x => ValueHelper.Wrap(
                            InterpretFunctionExpression(
                                expression,
                                expression.RightOperand,
                                func,
                                new[] { x })))
                        .ToList();

                case AphidTokenType.SelectManyOperator:
                    func = ValueHelper.Unwrap(InterpretExpression(expression.RightOperand));

                    return ((IEnumerable<object>)ValueHelper
                        .Unwrap(InterpretExpression(expression.LeftOperand)))
                        .SelectMany(x =>
                            (IEnumerable<object>)(ValueHelper.Unwrap(
                                InterpretFunctionExpression(
                                    expression,
                                    expression.RightOperand,
                                    func,
                                    new[] { x }))))
                        .Select(ValueHelper.Wrap)
                        .ToList();


                case AphidTokenType.AggregateOperator:
                    func = ValueHelper.Unwrap(InterpretExpression(expression.RightOperand));

                    return ((IEnumerable<object>)ValueHelper
                        .Unwrap(InterpretExpression(expression.LeftOperand)))
                        .Aggregate((x, y) => ValueHelper.Wrap(
                            InterpretFunctionExpression(
                                expression,
                                expression.RightOperand,
                                func,
                                new[] { x, y })));

                case AphidTokenType.AnyOperator:
                    func = ValueHelper.Unwrap(InterpretExpression(expression.RightOperand));

                    return ((IEnumerable<object>)ValueHelper
                        .Unwrap(InterpretExpression(expression.LeftOperand)))
                        .Any(x => (bool)ValueHelper.Unwrap(
                            InterpretFunctionExpression(
                                expression,
                                expression.RightOperand,
                                func,
                                new[] { x })));

                case AphidTokenType.WhereOperator:
                    func = ValueHelper.Unwrap(InterpretExpression(expression.RightOperand));

                    return ((IEnumerable<object>)ValueHelper
                        .Unwrap(InterpretExpression(expression.LeftOperand)))
                        .Where(x => (bool)ValueHelper.Unwrap(
                            InterpretFunctionExpression(
                                expression,
                                expression.RightOperand,
                                func,
                                new[] { x })))
                        .ToList();

                case AphidTokenType.CompositionOperator:
                    return InterpretFunctionComposition(expression);

                case AphidTokenType.CustomOperator0:
                case AphidTokenType.CustomOperator1:
                case AphidTokenType.CustomOperator10:
                case AphidTokenType.CustomOperator100:
                case AphidTokenType.CustomOperator101:
                case AphidTokenType.CustomOperator102:
                case AphidTokenType.CustomOperator103:
                case AphidTokenType.CustomOperator104:
                case AphidTokenType.CustomOperator105:
                case AphidTokenType.CustomOperator106:
                case AphidTokenType.CustomOperator107:
                case AphidTokenType.CustomOperator108:
                case AphidTokenType.CustomOperator109:
                case AphidTokenType.CustomOperator11:
                case AphidTokenType.CustomOperator110:
                case AphidTokenType.CustomOperator111:
                case AphidTokenType.CustomOperator112:
                case AphidTokenType.CustomOperator113:
                case AphidTokenType.CustomOperator114:
                case AphidTokenType.CustomOperator115:
                case AphidTokenType.CustomOperator116:
                case AphidTokenType.CustomOperator117:
                case AphidTokenType.CustomOperator118:
                case AphidTokenType.CustomOperator119:
                case AphidTokenType.CustomOperator12:
                case AphidTokenType.CustomOperator120:
                case AphidTokenType.CustomOperator121:
                case AphidTokenType.CustomOperator122:
                case AphidTokenType.CustomOperator123:
                case AphidTokenType.CustomOperator124:
                case AphidTokenType.CustomOperator125:
                case AphidTokenType.CustomOperator13:
                case AphidTokenType.CustomOperator14:
                case AphidTokenType.CustomOperator15:
                case AphidTokenType.CustomOperator16:
                case AphidTokenType.CustomOperator17:
                case AphidTokenType.CustomOperator18:
                case AphidTokenType.CustomOperator19:
                case AphidTokenType.CustomOperator2:
                case AphidTokenType.CustomOperator20:
                case AphidTokenType.CustomOperator21:
                case AphidTokenType.CustomOperator22:
                case AphidTokenType.CustomOperator23:
                case AphidTokenType.CustomOperator24:
                case AphidTokenType.CustomOperator25:
                case AphidTokenType.CustomOperator26:
                case AphidTokenType.CustomOperator27:
                case AphidTokenType.CustomOperator28:
                case AphidTokenType.CustomOperator29:
                case AphidTokenType.CustomOperator3:
                case AphidTokenType.CustomOperator30:
                case AphidTokenType.CustomOperator31:
                case AphidTokenType.CustomOperator32:
                case AphidTokenType.CustomOperator33:
                case AphidTokenType.CustomOperator34:
                case AphidTokenType.CustomOperator35:
                case AphidTokenType.CustomOperator36:
                case AphidTokenType.CustomOperator37:
                case AphidTokenType.CustomOperator38:
                case AphidTokenType.CustomOperator39:
                case AphidTokenType.CustomOperator4:
                case AphidTokenType.CustomOperator40:
                case AphidTokenType.CustomOperator41:
                case AphidTokenType.CustomOperator42:
                case AphidTokenType.CustomOperator43:
                case AphidTokenType.CustomOperator44:
                case AphidTokenType.CustomOperator45:
                case AphidTokenType.CustomOperator46:
                case AphidTokenType.CustomOperator47:
                case AphidTokenType.CustomOperator48:
                case AphidTokenType.CustomOperator49:
                case AphidTokenType.CustomOperator5:
                case AphidTokenType.CustomOperator50:
                case AphidTokenType.CustomOperator51:
                case AphidTokenType.CustomOperator52:
                case AphidTokenType.CustomOperator53:
                case AphidTokenType.CustomOperator54:
                case AphidTokenType.CustomOperator55:
                case AphidTokenType.CustomOperator56:
                case AphidTokenType.CustomOperator57:
                case AphidTokenType.CustomOperator58:
                case AphidTokenType.CustomOperator59:
                case AphidTokenType.CustomOperator6:
                case AphidTokenType.CustomOperator60:
                case AphidTokenType.CustomOperator61:
                case AphidTokenType.CustomOperator62:
                case AphidTokenType.CustomOperator63:
                case AphidTokenType.CustomOperator64:
                case AphidTokenType.CustomOperator65:
                case AphidTokenType.CustomOperator66:
                case AphidTokenType.CustomOperator67:
                case AphidTokenType.CustomOperator68:
                case AphidTokenType.CustomOperator69:
                case AphidTokenType.CustomOperator7:
                case AphidTokenType.CustomOperator70:
                case AphidTokenType.CustomOperator71:
                case AphidTokenType.CustomOperator72:
                case AphidTokenType.CustomOperator73:
                case AphidTokenType.CustomOperator74:
                case AphidTokenType.CustomOperator75:
                case AphidTokenType.CustomOperator76:
                case AphidTokenType.CustomOperator77:
                case AphidTokenType.CustomOperator78:
                case AphidTokenType.CustomOperator79:
                case AphidTokenType.CustomOperator8:
                case AphidTokenType.CustomOperator80:
                case AphidTokenType.CustomOperator81:
                case AphidTokenType.CustomOperator82:
                case AphidTokenType.CustomOperator83:
                case AphidTokenType.CustomOperator84:
                case AphidTokenType.CustomOperator85:
                case AphidTokenType.CustomOperator86:
                case AphidTokenType.CustomOperator87:
                case AphidTokenType.CustomOperator88:
                case AphidTokenType.CustomOperator89:
                case AphidTokenType.CustomOperator9:
                case AphidTokenType.CustomOperator90:
                case AphidTokenType.CustomOperator91:
                case AphidTokenType.CustomOperator92:
                case AphidTokenType.CustomOperator93:
                case AphidTokenType.CustomOperator94:
                case AphidTokenType.CustomOperator95:
                case AphidTokenType.CustomOperator96:
                case AphidTokenType.CustomOperator97:
                case AphidTokenType.CustomOperator98:
                case AphidTokenType.CustomOperator99:
                    return InterpretCustomBinaryOperator(expression);

                default:
                    throw new AphidRuntimeException("Unknown operator {0} in expression {1}", expression.Operator, expression);
            }
        }

        private AphidObject InterpretBinaryOperatorBodyExpression(BinaryOperatorBodyExpression expression)
        {
            var func = InterpretFunctionExpression(expression.Function);
            _binaryOperatorTable[expression.Operator] = func.GetFunction();

            return func;
        }

        private AphidObject InterpretFunctionComposition(BinaryOperatorExpression composition)
        {
            var funcs = new[] { composition.LeftOperand, composition.RightOperand }
                .Select(x => ValueHelper.Unwrap(InterpretExpression(x)))
                .ToArray();

            var c = new AphidFunctionComposition(
                composition.LeftOperand,
                composition.RightOperand,
                funcs[0],
                funcs[1]);

            return new AphidObject(c);
        }

        private AphidObject InterpretCustomUnaryOperator(UnaryOperatorExpression expression)
        {
            return CallCustomOperatorFunction(
                expression.Operator,
                "unary",
                new[] { InterpretExpression(expression.Operand) });
        }

        private AphidObject InterpretCustomBinaryOperator(BinaryOperatorExpression expression)
        {
            return CallCustomOperatorFunction(
                expression.Operator,
                "binary",
                new[]
                {
                    InterpretExpression(expression.LeftOperand),
                    InterpretExpression(expression.RightOperand)
                });
        }

        private AphidObject CallCustomOperatorFunction(
            AphidTokenType op,
            string name,
            object[] args)
        {
            return CallFunction(GetCustomOperatorFunction(op, name), args);
        }

        private AphidFunction GetCustomOperatorFunction(AphidTokenType op, string name)
        {
            AphidFunction func;

            if (!_binaryOperatorTable.TryGetValue(op, out func))
            {
                throw new AphidRuntimeException(
                    "Custom {0} operator '{1}' not defined.",
                    func,
                    op);
            }

            return func;
        }

        private AphidObject InterpretObjectExpression(ObjectExpression expression)
        {
            if (expression.Identifier == null ||
                expression.Identifier.Attributes == null ||
                !expression.Identifier.Attributes.Any() ||
                expression.Identifier.Attributes[0].Identifier != "class")
            {
                var obj = new AphidObject();

                foreach (var kvp in expression.Pairs)
                {
                    var id = (kvp.LeftOperand as IdentifierExpression).Identifier;
                    var value = ValueHelper.Wrap(InterpretExpression(kvp.RightOperand));
                    obj.Add(id, value);
                }

                return obj;
            }
            else
            {
                //if (!expression.IsStatement())
                //{
                //    throw new AphidRuntimeException(
                //        "Class declaration '{0}' must be statement.",
                //        expression.Identifier.Identifier);
                //}

                var t = _asmBuilder.CreateType(expression, GetImports());

                return new AphidObject(t); ;
            }
        }

        private AphidObject InterpretIdentifierExpression(IdentifierExpression expression)
        {
            AphidObject obj;

            if (_currentScope.TryResolve(expression.Identifier, out obj))
            {
                return obj;
            }
            else
            {
                return null;
            }
        }

        private AphidObject InterpretStringExpression(StringExpression expression)
        {
            return new AphidObject(StringParser.Parse(expression.Value));
        }

        public AphidObject CallFunction(string name, params object[] parms)
        {
            var val = InterpretIdentifierExpression(new IdentifierExpression(name));
            var func = ValueHelper.Unwrap(val) as AphidFunction;
            return CallFunction(func, parms);
        }

        public AphidObject CallFunction(AphidFunction function, params object[] parms)
        {
            return CallFunctionCore(function, parms.Select(ValueHelper.Wrap));
        }

        private AphidObject CallFunctionCore(AphidFunction function, IEnumerable<AphidObject> parms)
        {
            var functionScope = new AphidObject(null, function.ParentScope)
            {
                { _parent, function.ParentScope }
            };

            functionScope.Add(_scope, functionScope);
            var i = 0;
            var argList = parms.ToList();
            functionScope[_implicitArgs] = new AphidObject(argList);

            foreach (var arg in argList)
            {
                if (i == 0)
                {
                    SetImplicitArg(functionScope, arg);
                }

                if (function.Args.Length == i)
                {
                    break;
                }

                functionScope.Add(function.Args[i++], arg);
            }

            var lastScope = _currentScope;
            _currentScope = functionScope;
            Interpret(function.Body);
            var retVal = GetReturnValue();
            _currentScope = lastScope;

            return retVal;
        }

        private void SetImplicitArg(AphidObject arg)
        {
            SetImplicitArg(_currentScope, arg);
        }

        private void SetImplicitArg(AphidObject scope, AphidObject arg)
        {
            scope[_implicitArg] = arg;
        }

        private AphidObject CallInteropFunction(AphidInteropFunction func, params AphidObject[] objArgs)
        {
            object[] args = objArgs;

            if (func.UnwrapParameters)
            {
                args = objArgs.Select(x => x.Value).ToArray();
            }

            return ValueHelper.Wrap(func.Invoke(this, args));
        }

        public AphidObject CallFunction(AphidObject function, params object[] args)
        {
            var value = function.Value;
            var func = value as AphidFunction;

            if (func != null)
            {
                return CallFunction(func, args);
            }

            var func2 = value as AphidInteropFunction;

            if (func2 != null)
            {
                return CallInteropFunction(func2, (AphidObject[])args);
            }

            throw new AphidRuntimeException("Object is not function: {0}", function);
        }

        private string GetInteropAttribute(AphidExpression expression)
        {
            switch (expression.Type)
            {
                case AphidExpressionType.IdentifierExpression:
                    var attr = expression.ToIdentifier().Attributes.SingleOrDefault();

                    return attr != null ? attr.Identifier : null;

                case AphidExpressionType.BinaryOperatorExpression:
                    return GetInteropAttribute(
                        expression.ToBinaryOperator().LeftOperand);

                case AphidExpressionType.CallExpression:
                    return GetInteropAttribute(
                        expression.ToCall().FunctionExpression);

                default:
                    return null;
            }
        }

        public AphidObject CallStaticInteropFunction(CallExpression callExpression)
        {
            var path = FlattenPath(callExpression.FunctionExpression);
            var pathStr = string.Join(".", path);
            var imports = GetImports();

            var type = InteropTypeResolver.ResolveType(GetImports(), path);
            var methodName = path.Last();

            var args = callExpression.Args
                .Select(InterpretExpression)
                .Select(ValueHelper.Unwrap)
                .ToArray();

            var methodInfo = InteropMethodResolver.Resolve(type, methodName, args);

            MethodBase method;

            if (!methodInfo.GenericArguments.Any())
            {
                method = methodInfo.Method;
            }
            else
            {
                var m = (MethodInfo)methodInfo.Method;

                var genArgs = methodInfo.GenericArguments
                    .Take(m.GetGenericArguments().Length)
                    .ToArray();

                method = m.MakeGenericMethod(genArgs);
            }

            var convertedArgs = AphidTypeConverter.Convert(methodInfo.Arguments);

            return ValueHelper.Wrap(method.Invoke(null, convertedArgs));
        }

        private string FlattenAndJoinPath(AphidExpression exp)
        {
            return string.Join(".", FlattenPath(exp));
        }

        private string[] FlattenPath(AphidExpression exp)
        {
            var pathExps = Flatten(exp);

            if (!pathExps.All(x => x.Type == AphidExpressionType.IdentifierExpression))
            {
                throw new AphidRuntimeException("Invalid static interop call path.");
            }

            var path = pathExps
                .Select(x => ((IdentifierExpression)x).Identifier)
                .ToArray();

            return path;
        }

        private AphidExpression[] Flatten(AphidExpression exp)
        {
            var expressions = new List<AphidExpression>();

            switch (exp.Type)
            {
                case AphidExpressionType.BinaryOperatorExpression:
                    var binOpExp = (BinaryOperatorExpression)exp;
                    expressions.AddRange(Flatten(binOpExp.LeftOperand));
                    expressions.AddRange(Flatten(binOpExp.RightOperand));
                    break;

                default:
                    expressions.Add(exp);
                    break;
            }

            return expressions.ToArray();
        }

        private AphidObject InterpretCallExpression(CallExpression expression)
        {
            var args = expression.Args.Select(InterpretExpression).ToArray();

            var value = InterpretExpression(expression.FunctionExpression);
            object funcExp = ValueHelper.Unwrap(value);
            return InterpretFunctionExpression(expression, expression.FunctionExpression, funcExp, args);
        }

        private AphidObject InterpretFunctionExpression(
            AphidExpression callExpression,
            AphidExpression expression,
            object funcExp,
            object[] args)
        {
            var func = funcExp as AphidInteropFunction;

            if (func != null)
            {
                var interopArgs = func.UnwrapParameters ?
                    args.Select(ValueHelper.Unwrap).ToArray() :
                    args;

                PushFrame(callExpression, expression, interopArgs);
                var retVal = ValueHelper.Wrap(func.Invoke(this, interopArgs)); ;
                PopFrame();

                return retVal;
            }

            // Todo: make this use enums rather than slow type casting
            var interopMembers = funcExp as AphidInteropMember;

            if (interopMembers != null)
            {
                return InterpretInteropCallExpression(
                    callExpression,
                    expression,
                    args.Select(ValueHelper.DeepUnwrap).ToArray(),
                    interopMembers);
            }

            var interopPartial = funcExp as AphidInteropPartialFunction;

            if (interopPartial != null)
            {
                var curArgs = args.Select(ValueHelper.DeepUnwrap).ToArray();

                return InterpretInteropCallExpression(
                    callExpression,
                    expression,
                    interopPartial.Applied
                        .Concat(curArgs)
                        .ToArray(),
                    interopPartial.Member);
            }

            var func2 = funcExp as AphidFunction;

            if (func2 != null)
            {
                PushFrame(callExpression, expression, args);
                var retVal = CallFunctionCore(func2, args.Select(ValueHelper.Wrap));
                PopFrame();

                return retVal;
            }

            var composition = funcExp as AphidFunctionComposition;

            if (composition != null)
            {
                var retVal = InterpretFunctionExpression(
                    callExpression,
                    composition.LeftExpression,
                    composition.LeftFunction,
                    args);

                return InterpretFunctionExpression(
                    callExpression,
                    composition.RightExpression,
                    composition.RightFunction,
                    new[] { retVal });
            }

            throw new AphidRuntimeException("Could not find function {0}", expression);
        }

        private AphidObject InterpretImplicitArgumentExpression(AphidExpression expression)
        {
            return _currentScope.Resolve(_implicitArg);
        }

        private AphidObject InterpretImplicitArgumentsExpression(AphidExpression expression)
        {
            return _currentScope.Resolve(
                _implicitArgs,
                "$args cannot be used outside of function.");
        }

        private void PushFrame(
            AphidExpression callExpression,
            AphidExpression functionExpression,
            IEnumerable<object> args)
        {
            var name = functionExpression.Type == AphidExpressionType.IdentifierExpression ?
                ((IdentifierExpression)functionExpression).Identifier :
                "[Anonymous]";

            PushFrame(callExpression, name, args);
        }

        private void PushFrame(AphidExpression callExpression, string name, IEnumerable<object> args)
        {
            _frames.Push(new AphidFrame(callExpression, name, args));
        }

        private void PopFrame()
        {
            _frames.Pop();
        }

        private AphidObject InterpretInteropCallExpression(
            AphidExpression callExpression,
            AphidExpression expression,
            object[] arguments,
            AphidInteropMember interopMembers)
        {
            var methodInfo = InteropMethodResolver.Resolve(
                interopMembers.Members.OfType<MethodInfo>(),
                arguments);

            var method = !methodInfo.Method.IsGenericMethod ?
                methodInfo.Method :
                ((MethodInfo)methodInfo.Method).MakeGenericMethod(methodInfo.GenericArguments);

            var convertedArgs = AphidTypeConverter.Convert(methodInfo.Arguments);

            PushFrame(
                callExpression,
                string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name),
                convertedArgs);

            var retVal = method.Invoke(interopMembers.Target, convertedArgs);
            PopFrame();

            return ValueHelper.Wrap(retVal);
        }

        private AphidObject WrapInteropValue(object value)
        {
            if (AphidObject.IsAphidType(value))
            {
                return ValueHelper.Wrap(value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private AphidObject InterpretInteropNewExpression(AphidExpression operand)
        {
            switch (operand.Type)
            {
                case AphidExpressionType.CallExpression:
                    var call = operand.ToCall();

                    var args = call.Args
                        .Select(InterpretExpression)
                        .Select(ValueHelper.DeepUnwrap)
                        .ToArray();

                    var path = FlattenPath(call.FunctionExpression);
                    var type = InteropTypeResolver.ResolveType(GetImports(), path, isType: true);
                    var ctor = InteropMethodResolver.Resolve(type.GetConstructors(), args);
                    var convertedArgs = AphidTypeConverter.Convert(ctor.Arguments);
                    var result = ((ConstructorInfo)ctor.Method).Invoke(convertedArgs);

                    return ValueHelper.Wrap(result);

                default:
                    throw new NotImplementedException();
            }
        }

        private AphidObject InterpretFunctionExpression(FunctionExpression expression)
        {
            return new AphidObject(new AphidFunction()
            {
                Args = expression.Args
                    .Select(x => ((IdentifierExpression)x).Identifier)
                    .ToArray(),

                Body = expression.Body,
                ParentScope = _currentScope,
            });
        }

        private AphidObject InterpretArrayExpression(ArrayExpression expression)
        {
            var list = new AphidObject(expression.Elements.Select(InterpretExpression).OfType<AphidObject>().ToList());

            return list;
        }

        private AphidObject InterpretUnaryOperatorExpression(UnaryOperatorExpression expression)
        {
            if (!expression.IsPostfix)
            {
                switch (expression.Operator)
                {
                    case AphidTokenType.AdditionOperator:
                        return (AphidObject)InterpretExpression(expression.Operand);

                    case AphidTokenType.MinusOperator:
                        var val = ValueHelper.Unwrap(InterpretExpression(expression.Operand));

                        if (!(val is decimal))
                        {
                            throw new AphidRuntimeException(
                                "Unary operator '-' expects number, {0} was provided instead.",
                                val.GetType());
                        }

                        return ValueHelper.Wrap((decimal)val * -1);

                    case AphidTokenType.ComplementOperator:
                        val = InterpretAndUnwrap(expression.Operand);
                        ValueHelper.AssertNumber(val, "unary operator '~'");

                        return ValueHelper.Wrap((decimal)~Convert.ToUInt64(val));

                    case AphidTokenType.retKeyword:
                        SetReturnValue(ValueHelper.Wrap(InterpretExpression(expression.Operand)));
                        _isReturning = true;
                        return null;

                    case AphidTokenType.deleteKeyword:
                        var operand = ((IdentifierExpression)expression.Operand).Identifier;
                        return new AphidObject(_currentScope.TryResolveAndRemove(operand));

                    case AphidTokenType.NotOperator:
                        return new AphidObject(!(bool)ValueHelper.Unwrap(InterpretExpression(expression.Operand) as AphidObject));

                    case AphidTokenType.IncrementOperator:
                        var obj = InterpretExpression(expression.Operand) as AphidObject;
                        obj.Value = ((decimal)obj.Value) + 1;
                        return obj;

                    case AphidTokenType.DecrementOperator:
                        obj = InterpretExpression(expression.Operand) as AphidObject;
                        obj.Value = ((decimal)obj.Value) - 1;
                        return obj;

                    case AphidTokenType.DistinctOperator:
                        var opExp = InterpretExpression(expression.Operand);
                        var list = ((IEnumerable<object>)ValueHelper.Unwrap(opExp));

                        var result = list
                            .Select(ValueHelper.Unwrap)
                            .Distinct()
                            .Select(ValueHelper.Wrap) // To maintain backwards compat with Aphid
                            .ToList();                // list extension such as __list.count().

                        return ValueHelper.Wrap(result);

                    //return new AphidObject(list.Distinct(_comparer).ToList());

                    case AphidTokenType.usingKeyword:
                        var path = FlattenAndJoinPath(expression.Operand);
                        AddImport(path);

                        return null;

                    case AphidTokenType.newKeyword:
                        return InterpretInteropNewExpression(expression.Operand);

                    case AphidTokenType.loadKeyword:
                        Assembly asm;

                        switch (expression.Operand.Type)
                        {
                            case AphidExpressionType.IdentifierExpression:
                            case AphidExpressionType.BinaryOperatorExpression:
                                path = FlattenAndJoinPath(expression.Operand);
                                asm = Assembly.LoadWithPartialName(path);
                                break;

                            case AphidExpressionType.StringExpression:
                                path = StringParser.Parse(((StringExpression)expression.Operand).Value);
                                asm = Assembly.LoadFile(path);
                                break;

                            default:
                                throw new AphidRuntimeException("Invalid operand used with load keyword.");
                                
                        }

                        return ValueHelper.Wrap(asm);

                    case AphidTokenType.InteropOperator:
                        var attr = GetInteropAttribute(expression.Operand);

                        switch (attr)
                        {
                            case null:
                                switch (expression.Operand.Type)
                                {
                                    case AphidExpressionType.CallExpression:
                                        var callExp = (CallExpression)expression.Operand;
                                        return CallStaticInteropFunction(callExp);

                                    case AphidExpressionType.BinaryOperatorExpression:
                                        return InterpretMemberInteropExpression(
                                            null,
                                            expression.Operand.ToBinaryOperator());

                                    default:
                                        throw new NotImplementedException();
                                }

                            default:
                                throw new NotImplementedException();

                        }

                    case AphidTokenType.CustomOperator0:
                    case AphidTokenType.CustomOperator1:
                    case AphidTokenType.CustomOperator10:
                    case AphidTokenType.CustomOperator100:
                    case AphidTokenType.CustomOperator101:
                    case AphidTokenType.CustomOperator102:
                    case AphidTokenType.CustomOperator103:
                    case AphidTokenType.CustomOperator104:
                    case AphidTokenType.CustomOperator105:
                    case AphidTokenType.CustomOperator106:
                    case AphidTokenType.CustomOperator107:
                    case AphidTokenType.CustomOperator108:
                    case AphidTokenType.CustomOperator109:
                    case AphidTokenType.CustomOperator11:
                    case AphidTokenType.CustomOperator110:
                    case AphidTokenType.CustomOperator111:
                    case AphidTokenType.CustomOperator112:
                    case AphidTokenType.CustomOperator113:
                    case AphidTokenType.CustomOperator114:
                    case AphidTokenType.CustomOperator115:
                    case AphidTokenType.CustomOperator116:
                    case AphidTokenType.CustomOperator117:
                    case AphidTokenType.CustomOperator118:
                    case AphidTokenType.CustomOperator119:
                    case AphidTokenType.CustomOperator12:
                    case AphidTokenType.CustomOperator120:
                    case AphidTokenType.CustomOperator121:
                    case AphidTokenType.CustomOperator122:
                    case AphidTokenType.CustomOperator123:
                    case AphidTokenType.CustomOperator124:
                    case AphidTokenType.CustomOperator125:
                    case AphidTokenType.CustomOperator13:
                    case AphidTokenType.CustomOperator14:
                    case AphidTokenType.CustomOperator15:
                    case AphidTokenType.CustomOperator16:
                    case AphidTokenType.CustomOperator17:
                    case AphidTokenType.CustomOperator18:
                    case AphidTokenType.CustomOperator19:
                    case AphidTokenType.CustomOperator2:
                    case AphidTokenType.CustomOperator20:
                    case AphidTokenType.CustomOperator21:
                    case AphidTokenType.CustomOperator22:
                    case AphidTokenType.CustomOperator23:
                    case AphidTokenType.CustomOperator24:
                    case AphidTokenType.CustomOperator25:
                    case AphidTokenType.CustomOperator26:
                    case AphidTokenType.CustomOperator27:
                    case AphidTokenType.CustomOperator28:
                    case AphidTokenType.CustomOperator29:
                    case AphidTokenType.CustomOperator3:
                    case AphidTokenType.CustomOperator30:
                    case AphidTokenType.CustomOperator31:
                    case AphidTokenType.CustomOperator32:
                    case AphidTokenType.CustomOperator33:
                    case AphidTokenType.CustomOperator34:
                    case AphidTokenType.CustomOperator35:
                    case AphidTokenType.CustomOperator36:
                    case AphidTokenType.CustomOperator37:
                    case AphidTokenType.CustomOperator38:
                    case AphidTokenType.CustomOperator39:
                    case AphidTokenType.CustomOperator4:
                    case AphidTokenType.CustomOperator40:
                    case AphidTokenType.CustomOperator41:
                    case AphidTokenType.CustomOperator42:
                    case AphidTokenType.CustomOperator43:
                    case AphidTokenType.CustomOperator44:
                    case AphidTokenType.CustomOperator45:
                    case AphidTokenType.CustomOperator46:
                    case AphidTokenType.CustomOperator47:
                    case AphidTokenType.CustomOperator48:
                    case AphidTokenType.CustomOperator49:
                    case AphidTokenType.CustomOperator5:
                    case AphidTokenType.CustomOperator50:
                    case AphidTokenType.CustomOperator51:
                    case AphidTokenType.CustomOperator52:
                    case AphidTokenType.CustomOperator53:
                    case AphidTokenType.CustomOperator54:
                    case AphidTokenType.CustomOperator55:
                    case AphidTokenType.CustomOperator56:
                    case AphidTokenType.CustomOperator57:
                    case AphidTokenType.CustomOperator58:
                    case AphidTokenType.CustomOperator59:
                    case AphidTokenType.CustomOperator6:
                    case AphidTokenType.CustomOperator60:
                    case AphidTokenType.CustomOperator61:
                    case AphidTokenType.CustomOperator62:
                    case AphidTokenType.CustomOperator63:
                    case AphidTokenType.CustomOperator64:
                    case AphidTokenType.CustomOperator65:
                    case AphidTokenType.CustomOperator66:
                    case AphidTokenType.CustomOperator67:
                    case AphidTokenType.CustomOperator68:
                    case AphidTokenType.CustomOperator69:
                    case AphidTokenType.CustomOperator7:
                    case AphidTokenType.CustomOperator70:
                    case AphidTokenType.CustomOperator71:
                    case AphidTokenType.CustomOperator72:
                    case AphidTokenType.CustomOperator73:
                    case AphidTokenType.CustomOperator74:
                    case AphidTokenType.CustomOperator75:
                    case AphidTokenType.CustomOperator76:
                    case AphidTokenType.CustomOperator77:
                    case AphidTokenType.CustomOperator78:
                    case AphidTokenType.CustomOperator79:
                    case AphidTokenType.CustomOperator8:
                    case AphidTokenType.CustomOperator80:
                    case AphidTokenType.CustomOperator81:
                    case AphidTokenType.CustomOperator82:
                    case AphidTokenType.CustomOperator83:
                    case AphidTokenType.CustomOperator84:
                    case AphidTokenType.CustomOperator85:
                    case AphidTokenType.CustomOperator86:
                    case AphidTokenType.CustomOperator87:
                    case AphidTokenType.CustomOperator88:
                    case AphidTokenType.CustomOperator89:
                    case AphidTokenType.CustomOperator9:
                    case AphidTokenType.CustomOperator90:
                    case AphidTokenType.CustomOperator91:
                    case AphidTokenType.CustomOperator92:
                    case AphidTokenType.CustomOperator93:
                    case AphidTokenType.CustomOperator94:
                    case AphidTokenType.CustomOperator95:
                    case AphidTokenType.CustomOperator96:
                    case AphidTokenType.CustomOperator97:
                    case AphidTokenType.CustomOperator98:
                    case AphidTokenType.CustomOperator99:
                        return InterpretCustomUnaryOperator(expression);

                    default:
                        throw CreateUnaryOperatorException(expression);
                }
            }
            else
            {
                switch (expression.Operator)
                {
                    case AphidTokenType.IncrementOperator:
                        var obj = InterpretExpression(expression.Operand) as AphidObject;
                        var v = obj.Value;
                        obj.Value = ((decimal)obj.Value) + 1;
                        return new AphidObject(v);

                    case AphidTokenType.DecrementOperator:
                        obj = InterpretExpression(expression.Operand) as AphidObject;
                        v = obj.Value;
                        obj.Value = ((decimal)obj.Value) - 1;
                        return new AphidObject(v);

                    case AphidTokenType.definedKeyword:
                        if (expression.Operand is IdentifierExpression)
                        {
                            return ValueHelper.Wrap(InterpretIdentifierExpression(expression.Operand as IdentifierExpression) != null);
                        }
                        else if (expression.Operand is BinaryOperatorExpression)
                        {
                            var objRef = InterpretBinaryOperatorExpression(expression.Operand as BinaryOperatorExpression, true) as AphidRef;
                            return new AphidObject(objRef.Object.ContainsKey(objRef.Name));
                        }
                        else
                        {
                            throw new AphidRuntimeException("Unknown ? operand");
                        }


                    //var obj = InterpretExpression(

                    default:
                        throw CreateUnaryOperatorException(expression);
                }
            }
        }

        private AphidObject InterpretBooleanExpression(BooleanExpression expression)
        {
            return new AphidObject(expression.Value);
        }

        private AphidObject InterpretIfExpression(IfExpression expression)
        {
            if ((bool)ValueHelper.Unwrap(InterpretExpression(expression.Condition)))
            {
                InterpretChild(expression.Body);
            }
            else if (expression.ElseBody != null)
            {
                InterpretChild(expression.ElseBody);
            }
            return null;
        }

        private AphidObject InterpretNumberExpression(NumberExpression expression)
        {
            return new AphidObject(expression.Value);
        }

        private AphidObject InterpretArrayAccessExpression(ArrayAccessExpression expression)
        {
            var val = ValueHelper.Unwrap(InterpretExpression(expression.ArrayExpression));
            var index = Convert.ToInt32(ValueHelper.Unwrap(InterpretExpression(expression.KeyExpression)));
            var array = val as List<AphidObject>;
            string str;
            IList list;
            IEnumerable enumerable;

            if (array != null)
            {
                if (index < 0 || index >= array.Count)
                {
                    throw new AphidRuntimeException("Index out of range: {0}.", index);
                }

                return array[index];
            }
            else if ((str = val as string) != null)
            {
                return new AphidObject(str[index].ToString());
            }
            else if ((list = val as IList) != null)
            {
                return ValueHelper.Wrap(list[index]);
            }
            else if ((enumerable = val as IEnumerable) != null)
            {
                var i = 0;

                foreach (var o in enumerable)
                {
                    if (i++ == index)
                    {
                        return new AphidObject(o);
                    }
                }

                throw new AphidRuntimeException("Index out of range: {0}.", index);
            }
            else
            {
                throw new AphidRuntimeException("Array access not supported by {0}.", val);
            }
        }

        private AphidObject InterpretForExpression(ForExpression expression)
        {
            EnterChildScope();
            var init = InterpretExpression(expression.Initialization);

            while ((bool)(InterpretExpression(expression.Condition) as AphidObject).Value)
            {
                EnterChildScope();
                Interpret(expression.Body, resetIsReturning: false);
                InterpretExpression(expression.Afterthought);
                _isContinuing = false;

                if (LeaveChildScope(true) || _isBreaking)
                {
                    _isBreaking = false;
                    break;
                }
            }

            LeaveChildScope(true);

            return null;
        }

        private AphidObject InterpretForEachExpression(ForEachExpression expression)
        {
            var collection = InterpretExpression(expression.Collection) as AphidObject;
            var elements = collection.Value as IEnumerable;

            var elementId = expression.Element != null ?
                (expression.Element as IdentifierExpression).Identifier :
                null;

            foreach (var element in elements)
            {
                EnterChildScope();
                var obj = ValueHelper.Wrap(element);
                SetImplicitArg(obj);

                if (elementId != null)
                {
                    _currentScope.Add(elementId, obj);
                }

                Interpret(expression.Body, false);
                _isContinuing = false;

                if (LeaveChildScope(true) || _isBreaking)
                {
                    _isBreaking = false;
                    break;
                }
            }

            return null;
        }

        private AphidObject InterpretLoadScriptExpression(LoadScriptExpression expression)
        {
            var file = ValueHelper.Unwrap(InterpretExpression(expression.FileExpression)) as string;

            if (_loader == null || file == null)
            {
                throw new AphidRuntimeException("Cannot load script {0}", expression.FileExpression);
            }

            _loader.LoadScript(file);

            return null;
        }

        private AphidObject InterpretLoadLibraryExpression(LoadLibraryExpression expression)
        {
            var library = ValueHelper.Unwrap(InterpretExpression(expression.LibraryExpression)) as string;

            if (_loader == null || library == null)
            {
                throw new AphidRuntimeException("Cannot load script {0}", expression.LibraryExpression);
            }

            _loader.LoadLibrary(library, _currentScope);

            return null;
        }

        private AphidObject InterpretContinueExpression()
        {
            _isContinuing = true;
            return null;
        }

        private AphidObject InterpretBreakExpression()
        {
            _isBreaking = true;
            return null;
        }

        private AphidObject InterpretPartialFunctionExpression(PartialFunctionExpression expression)
        {
            var obj = (AphidObject)InterpretExpression(expression.Call.FunctionExpression);

            var func = obj.Value as AphidFunction;
            if (func != null)
            {
                var partialArgCount = func.Args.Length - expression.Call.Args.Count();
                var partialArgs = func.Args.Skip(partialArgCount).ToArray();

                var partialFunc = new AphidFunction()
                {
                    Args = partialArgs,
                    Body = new List<AphidExpression> 
                    {
                        new UnaryOperatorExpression(AphidTokenType.retKeyword,
                            new CallExpression(
                                expression.Call.FunctionExpression, 
                                expression.Call.Args.Concat(
                                partialArgs.Select(x => new IdentifierExpression(x))).ToList())),
                    },
                    ParentScope = _currentScope,
                };

                return new AphidObject(partialFunc);
            }
            else
            {
                var interopObj = obj.Value as AphidInteropMember;

                if (interopObj == null)
                {
                    throw new NotImplementedException();
                }

                var applied = expression.Call.Args
                    .Select(InterpretExpression)
                    .Select(ValueHelper.Unwrap)
                    .ToArray();

                return new AphidObject(new AphidInteropPartialFunction(interopObj, applied));
            }
        }

        private AphidObject InterpretPartialOperatorExpression(PartialOperatorExpression expression)
        {
            var name = "$po";

            var func = new AphidFunction()
            {
                Args = new[] { name },
                Body = new List<AphidExpression> 
                {
                    new UnaryOperatorExpression(
                        AphidTokenType.retKeyword,
                        new BinaryOperatorExpression(
                            new IdentifierExpression(name),
                            expression.Operator,
                            expression.Operand))
                },
            };

            return new AphidObject(func);
        }

        private AphidObject InterpretThisExpression()
        {
            return _currentScope;
        }

        private AphidObject InterpretPatternMatchingExpression(PatternMatchingExpression expression)
        {
            var left = (AphidObject)InterpretExpression(expression.TestExpression);

            foreach (var pattern in expression.Patterns)
            {
                if (pattern.Patterns != null && pattern.Patterns.Any())
                {
                    foreach (var patternTest in pattern.Patterns)
                    {
                        var right = (AphidObject)InterpretExpression(patternTest);

                        var b = left.Value != null ?
                            left.Value.Equals(right.Value) :
                            (null == right.Value && left.Count == 0 && right.Count == 0);

                        if (b)
                        {
                            return ValueHelper.Wrap(InterpretExpression(pattern.Value));
                        }
                    }
                }
                else
                {
                    return ValueHelper.Wrap(InterpretExpression(pattern.Value));
                }
            }

            return new AphidObject();
        }

        private void InterpretExtendExpression(ExtendExpression expression)
        {
            var obj = InterpretObjectExpression(expression.Object);
            TypeExtender.Extend(this, expression.ExtendType, obj);
        }

        private void InterpretWhileExpression(WhileExpression expression)
        {
            while ((bool)((AphidObject)(InterpretExpression(expression.Condition))).Value)
            {
                EnterChildScope();
                Interpret(expression.Body, false);
                _isContinuing = false;

                if (LeaveChildScope(true) || _isBreaking)
                {
                    _isBreaking = false;
                    break;
                }
            }
        }

        private void InterpretDoWhileExpression(DoWhileExpression expression)
        {
            do
            {
                EnterChildScope();
                Interpret(expression.Body, false);
                _isContinuing = false;

                if (LeaveChildScope(true) || _isBreaking)
                {
                    _isBreaking = false;
                    break;
                }
            } while ((bool)((AphidObject)(InterpretExpression(expression.Condition))).Value);
        }

        private void InterpretTryBlock(TryExpression expression)
        {
            EnterChildScope();
            Interpret(expression.TryBody, false);
            LeaveChildScope(true);
        }

        private void InterpretCatchBlock(TryExpression expression, Exception e)
        {
            LeaveChildScope(true);
            EnterChildScope();

            if (expression.CatchArg != null)
            {
                var ex = new AphidObject(ExceptionHelper.Unwrap(e).Message);
                ex.Add("stack", new AphidObject(ExceptionHelper.StackTrace(GetStackTrace())));
                _currentScope.Add(expression.CatchArg.Identifier, ex);
            }

            Interpret(expression.CatchBody, false);
            LeaveChildScope(true);
        }

        private void InterpretFinallyBlock(TryExpression expression)
        {
            EnterChildScope();
            Interpret(expression.FinallyBody, false);
            LeaveChildScope(false);
        }

        private void InterpretTryExpression(TryExpression expression)
        {
            if (expression.FinallyBody == null)
            {
                try
                {
                    InterpretTryBlock(expression);
                }
                catch (Exception e)
                {
                    InterpretCatchBlock(expression, e);
                }
            }
            else if (expression.CatchBody != null)
            {
                try
                {
                    InterpretTryBlock(expression);
                }
                catch (Exception e)
                {
                    InterpretCatchBlock(expression, e);
                }
                finally
                {
                    InterpretFinallyBlock(expression);
                }
            }
            else
            {
                try
                {
                    InterpretTryBlock(expression);
                }
                finally
                {
                    InterpretFinallyBlock(expression);
                }
            }
        }

        private void InterpretTextExpression(TextExpression expression)
        {
            WriteOut(expression.Text);
        }

        private void InterpretGatorEmitExpression(GatorEmitExpression expression)
        {
            var obj = InterpretExpression(expression.Expression);

            if (obj == null)
            {
                return;
            }

            var result = ValueHelper.Unwrap(obj).ToString();

            if (GatorEmitFilter != null)
            {
                result = GatorEmitFilter(result);
            }

            WriteOut(result);
        }

        private AphidObject InterpretTernaryOperatorExpression(TernaryOperatorExpression expression)
        {
            switch (expression.Operator)
            {
                case AphidTokenType.ConditionalOperator:
                    return (AphidObject)InterpretExpression(
                        (bool)((AphidObject)InterpretExpression(expression.FirstOperand)).Value ?
                        expression.SecondOperand :
                        expression.ThirdOperand);

                default:
                    throw new InvalidOperationException();
            }
        }

        private void InterpretSwitchExpression(SwitchExpression expression)
        {
            var exp = (AphidObject)InterpretExpression(expression.Expression);

            foreach (var c in expression.Cases)
            {
                foreach (var c2 in c.Cases)
                {
                    var caseValue = (AphidObject)InterpretExpression(c2);

                    if (!exp.Value.Equals(caseValue.Value))
                    {
                        continue;
                    }

                    EnterChildScope();
                    Interpret(c.Body, resetIsReturning: false);
                    LeaveChildScope(bubbleReturnValue: true);

                    return;
                }
            }

            if (expression.DefaultCase != null)
            {
                EnterChildScope();
                Interpret(expression.DefaultCase, resetIsReturning: false);
                LeaveChildScope(bubbleReturnValue: true);
            }
        }

        public object InterpretExpression(AphidExpression expression)
        {
            switch (expression.Type)
            {
                case AphidExpressionType.BinaryOperatorExpression:
                    return InterpretBinaryOperatorExpression((BinaryOperatorExpression)expression);

                case AphidExpressionType.BinaryOperatorBodyExpression:
                    return InterpretBinaryOperatorBodyExpression((BinaryOperatorBodyExpression)expression);

                case AphidExpressionType.ObjectExpression:
                    return InterpretObjectExpression((ObjectExpression)expression);

                case AphidExpressionType.StringExpression:
                    return InterpretStringExpression((StringExpression)expression);

                case AphidExpressionType.NumberExpression:
                    return InterpretNumberExpression((NumberExpression)expression);

                case AphidExpressionType.CallExpression:
                    return InterpretCallExpression((CallExpression)expression);

                case AphidExpressionType.IdentifierExpression:
                    return InterpretIdentifierExpression((IdentifierExpression)expression);

                case AphidExpressionType.FunctionExpression:
                    return InterpretFunctionExpression((FunctionExpression)expression);

                case AphidExpressionType.ArrayExpression:
                    return InterpretArrayExpression((ArrayExpression)expression);

                case AphidExpressionType.UnaryOperatorExpression:
                    return InterpretUnaryOperatorExpression((UnaryOperatorExpression)expression);

                case AphidExpressionType.BooleanExpression:
                    return InterpretBooleanExpression((BooleanExpression)expression);

                case AphidExpressionType.IfExpression:
                    return InterpretIfExpression((IfExpression)expression);

                case AphidExpressionType.ArrayAccessExpression:
                    return InterpretArrayAccessExpression((ArrayAccessExpression)expression);

                case AphidExpressionType.ForEachExpression:
                    return InterpretForEachExpression((ForEachExpression)expression);

                case AphidExpressionType.ForExpression:
                    return InterpretForExpression((ForExpression)expression);

                case AphidExpressionType.LoadScriptExpression:
                    return InterpretLoadScriptExpression((LoadScriptExpression)expression);

                case AphidExpressionType.LoadLibraryExpression:
                    return InterpretLoadLibraryExpression((LoadLibraryExpression)expression);

                case AphidExpressionType.NullExpression:
                    return new AphidObject(null);

                case AphidExpressionType.ContinueExpression:
                    return InterpretContinueExpression();

                case AphidExpressionType.BreakExpression:
                    return InterpretBreakExpression();

                case AphidExpressionType.PartialFunctionExpression:
                    return InterpretPartialFunctionExpression((PartialFunctionExpression)expression);

                case AphidExpressionType.ThisExpression:
                    return InterpretThisExpression();

                case AphidExpressionType.PatternMatchingExpression:
                    return InterpretPatternMatchingExpression((PatternMatchingExpression)expression);

                case AphidExpressionType.ExtendExpression:
                    InterpretExtendExpression((ExtendExpression)expression);

                    return null;

                case AphidExpressionType.WhileExpression:
                    InterpretWhileExpression((WhileExpression)expression);

                    return null;

                case AphidExpressionType.DoWhileExpression:
                    InterpretDoWhileExpression((DoWhileExpression)expression);

                    return null;

                case AphidExpressionType.TryExpression:
                    InterpretTryExpression((TryExpression)expression);

                    return null;

                case AphidExpressionType.TernaryOperatorExpression:
                    return InterpretTernaryOperatorExpression((TernaryOperatorExpression)expression);

                case AphidExpressionType.SwitchExpression:
                    InterpretSwitchExpression((SwitchExpression)expression);

                    return null;

                case AphidExpressionType.TextExpression:
                    InterpretTextExpression((TextExpression)expression);

                    return null;

                case AphidExpressionType.GatorOpenExpression:
                case AphidExpressionType.GatorCloseExpression:
                    return null;

                case AphidExpressionType.GatorEmitExpression:
                    InterpretGatorEmitExpression((GatorEmitExpression)expression);

                    return null;

                case AphidExpressionType.PartialOperatorExpression:
                    var partialOpExp = (PartialOperatorExpression)expression;

                    return InterpretPartialOperatorExpression(partialOpExp);

                case AphidExpressionType.ImplicitArgumentExpression:
                    return InterpretImplicitArgumentExpression((ImplicitArgumentExpression)expression);

                case AphidExpressionType.ImplicitArgumentsExpression:
                    return InterpretImplicitArgumentsExpression((ImplicitArgumentsExpression)expression);

                default:
                    throw new AphidRuntimeException("Unexpected expression {0}", expression);
            }
        }

        public object InterpretAndUnwrap(AphidExpression expression)
        {
            return ValueHelper.Unwrap(InterpretExpression(expression));
        }

        public void Interpret(List<AphidExpression> expressions, bool resetIsReturning = true)
        {
            AphidObject document;

            if (!_currentScope.TryGetValue(_block, out document))
            {
                _currentScope.Add(_block, new AphidObject(expressions));
            }

            foreach (var expression in expressions)
            {
                if (expression is IdentifierExpression)
                {
                    _currentScope.Add((expression as IdentifierExpression).Identifier, new AphidObject());
                }
                else
                {
                    InterpretExpression(expression);
                }

                if (_isBreaking || _isContinuing)
                {
                    break;
                }
                else if (_isReturning)
                {
                    if (resetIsReturning)
                    {
                        _isReturning = false;
                    }

                    break;
                }
            }
        }

        public void Interpret(string code, bool isTextDocument = false)
        {
            var lexer = new AphidLexer(code);

            if (isTextDocument)
            {
                lexer.SetTextMode();
            }

            var parser = new AphidParser(lexer.GetTokens());
            var ast = new PartialOperatorMutator().MutateRecursively(parser.Parse());
            ast = new AphidMacroMutator().MutateRecursively(ast);
            ast = new AphidIdDirectiveMutator().MutateRecursively(ast);
            Interpret(ast);
        }

        public void InterpretFile(string filename, bool isTextDocument = false)
        {
            _loader.LoadScript(filename, isTextDocument);
        }

        public AphidFrame[] GetStackTrace()
        {
            return _frames.ToArray();
        }
    }
}
