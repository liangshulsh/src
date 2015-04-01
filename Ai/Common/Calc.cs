// Ai Software Common Library.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Ai.Common {
    public static class Calc {
        /// <summary>
        /// List of accepted operators for calculation.
        /// </summary>
        public const string OPERATORS = "^*/\\+-";
        /// <summary>
        /// Represent the unlimited value of a number.
        /// </summary>
        public const char UNLIMITED_VALUE = '~';
        /// <summary>
        /// Character used to separate each members of a list.
        /// </summary>
        public const char LIST_SEPARATOR = ',';
        /// <summary>
        /// List of invalid characters if used in code-name of a function.
        /// </summary>
        public const string INVALID_CHARS = "~!@#$%^&*()+`-={}|[]\\:\";'<>?,./ \n";
        private static CalcFunction[] _builtin_functions ={CalcFunctions.Abs,CalcFunctions.ACos,
                                                             CalcFunctions.ASin,CalcFunctions.ATan};
        /// <summary>
        /// Determines if an object is a floating number or percentage number.
        /// </summary>
        /// <param name="value">An object to check.</param>
        /// <returns>true, if an object is either floating number or percentage number, false, otherwise.</returns>
        public static bool isNumber(object value) {
            if (value == null) return false;
            double d;
            bool isNum;
            string str = value.ToString().Trim();
            if (str == string.Empty) return false;
            if (str[str.Length - 1] == '%' && str.Length > 1) str = str.Substring(0, str.Length - 1);
            isNum = double.TryParse(str,
                System.Globalization.NumberStyles.Float,
                Ai.Renderer.Drawing.en_us_ci.NumberFormat,
                out d);
            return isNum;
        }
        /// <summary>
        /// Converts an object to a double value.
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <returns>If succeeded, returns double represent the value of the object, 0d if fails.</returns>
        public static double convertToNumber(object value) {
            if (isNumber(value)) {
                string str = value.ToString().Trim();
                string strSign = "";
                if (str[str.Length - 1] == '%') {
                    strSign = "%";
                    str = str.Substring(0, str.Length - 1);
                }
                double d = double.Parse(str,
                    System.Globalization.NumberStyles.Float,
                    Ai.Renderer.Drawing.en_us_ci.NumberFormat);
                return (strSign == "%" ? d / 100 : d);
            }
            return 0d;
        }
        /// <summary>
        /// Represent the bracket and its member in a formula.
        /// </summary>
        public sealed class Bracket {
            int _openIndex = -1;
            int _closeIndex = -1;
            bool _valid = true;
            CalcFunction _function = null;
            List<object> _childs = null;
            Bracket _parent = null;
            public Bracket() {
                _childs = new List<object>();
            }
            public Bracket(Bracket parent) {
                _childs = new List<object>();
                _parent = parent;
            }
            /// <summary>
            /// Gets the zero-based index of the opening bracket in formula.
            /// </summary>
            public int OpenIndex {
                get { return _openIndex; }
            }
            /// <summary>
            /// Gets the zero-based index of the closing bracket in formula.
            /// </summary>
            public int CloseIndex {
                get { return _closeIndex; }
            }
            /// <summary>
            /// Gets or sets the parent bracket in formula that contains this bracket.
            /// </summary>
            public Bracket Parent {
                get { return _parent; }
                set { _parent = value; }
            }
            /// <summary>
            /// Gets or sets the CalcFunction object associated with the bracket.
            /// </summary>
            public CalcFunction Function {
                get { return _function; }
                set { _function = value; }
            }
            /// <summary>
            /// Gets the containing members of the bracket.
            /// </summary>
            public object[] Childs { get { return _childs.ToArray(); } }
            /// <summary>
            /// Gets a value indicating that the formula in the bracket is valid.
            /// </summary>
            public bool Valid { get { return _valid; } }
            /// <summary>
            /// Determines wether an object is contained within the bracket.
            /// </summary>
            /// <param name="value">An object to search.</param>
            /// <returns>true if the object is found within the bracket, false, otherwise.</returns>
            public bool contains(object value) { return _childs.Contains(value); }
            /// <summary>
            /// Parses a string that conatins the formula of a bracket.
            /// </summary>
            /// <param name="value">A string value represent the formula of the bracket.</param>
            /// <param name="start">Starting index where the formula will be parsed.</param>
            /// <returns>Returns the zero-based index where the bracket is closed within the formula.</returns>
            public int parseString(string value, int start) {
                int i = start;
                _openIndex = start - 1;
                bool quit = false;
                _childs.Clear();
                _valid = true;
                string strNode = "";
                while (!quit) {
                    if (value[i] == '(') {
                        Bracket child = new Bracket(this);
                        i = child.parseString(value, i + 1);
                        if (strNode != "") {
                            if (OPERATORS.Contains(strNode.Trim())) {
                                // The last node was an operator.
                                _childs.Add(strNode.Trim());
                            } else if (strNode == "~") {
                                // The last node was the unlimited number sign.
                                _childs.Add(strNode.Trim());
                            } else if (isNumber(strNode)) {
                                // The last node was a number.
                                _childs.Add(strNode.Trim());
                            } else {
                                if (strNode[strNode.Length - 1] == '%') {
                                    // Check if the last node was a percentage number.
                                    if (!isNumber(strNode.Substring(0, strNode.Length - 1))) _valid = false;
                                    _childs.Add(strNode);
                                } else {
                                    // Check if the last node was a code of registered function's code.
                                    // Build up functions.
                                    bool found = false;
                                    foreach (CalcFunction fx in _builtin_functions) {
                                        if (fx.evaluateCode(strNode)) {
                                            child.Function = fx;
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) {
                                        // User defined functions.
                                        foreach (CalcFunction fx in CalcFunctions.UserFunctions) {
                                            if (fx.evaluateCode(strNode)) {
                                                child.Function = fx;
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!found) {
                                        // No functions were found.
                                        _valid = false;
                                        _childs.Add(strNode);
                                    }
                                }
                            }
                        }
                        _childs.Add(child);
                        strNode = "";
                    } else {
                        if (value[i] == ')') {
                            quit = true;
                            _closeIndex = i;
                        } else {
                            if (OPERATORS.Contains(value[i].ToString().Trim())) {
                                if (_childs.Count == 0) {
                                    if (value[i] != '-' && strNode == "") _valid = false;
                                } else {
                                    if (strNode == LIST_SEPARATOR.ToString().Trim() && value[i] != '-') _valid = false;
                                }
                                if (strNode != "") _childs.Add(strNode);
                                strNode = value[i].ToString().Trim();
                            } else {
                                if (strNode == "~") {
                                    _childs.Add(strNode);
                                    strNode = "";
                                }
                                if (value[i] == '.' || (value[i] >= 48 && value[i] <= 57) ||
                                    (value[i] >= 65 && value[i] <= 90) || (value[i] >= 97 && value[i] <= 122) ||
                                    value[i] == '_' || value[i] == '%') {
                                    if (OPERATORS.Contains(strNode) && strNode != "") {
                                        if (_childs.Count > 0) {
                                            _childs.Add(strNode);
                                            strNode = "";
                                        }
                                    }
                                    strNode += value[i];
                                } else if (value[i] == LIST_SEPARATOR) {
                                    if (strNode != "") _childs.Add(strNode);
                                    else if (_childs.Count == 0) _valid = false;
                                    strNode = LIST_SEPARATOR.ToString().Trim();
                                } else if (value[i] == ' ') {
                                    if (strNode != "") _childs.Add(strNode);
                                    strNode = "";
                                } else if (value[i] == UNLIMITED_VALUE) {
                                    if (strNode != "") _childs.Add(strNode);
                                    strNode = "~";
                                }
                            }
                        }
                    }
                    i++;
                    if (i >= value.Length) quit = true;
                }
                if (strNode != "") _childs.Add(strNode);
                return i;
            }
            /// <summary>
            /// Calculates the members of the bracket.
            /// </summary>
            /// <returns>An object represent the result of calculation operation.</returns>
            public object calculate() {
                int operatorIdx;
                while (_childs.Count > 1) {
                    operatorIdx = -1;
                    foreach (char c in OPERATORS) {
                        if (_childs.Contains(c.ToString().Trim())) {
                            operatorIdx = _childs.IndexOf(c.ToString().Trim());
                            break;
                        }
                    }
                    if (operatorIdx == -1) break;
                    operateChild((string)_childs[operatorIdx], operatorIdx);
                }
                if (_function != null) return _function.calculate(this);
                return _childs[0];
            }
            /// <summary>
            /// Execute the operation of two childs using specified operator.
            /// </summary>
            /// <param name="op">Operator used to operate the childs.</param>
            /// <param name="opIdx">The zero-based index where the operator is in the childs.</param>
            private void operateChild(string op, int opIdx) {
                object left = null;
                object right = null;
                if (opIdx > 0) left = _childs[opIdx - 1];
                if (opIdx < _childs.Count - 1) right = _childs[opIdx + 1];
                if (left != null && right != null) {
                    if (left is Bracket) {
                        Bracket b = (Bracket)left;
                        left = b.calculate();
                    }
                    if (right is Bracket) {
                        Bracket b = (Bracket)right;
                        right = b.calculate();
                    }
                }
                double dleft = convertToNumber(left);
                double dright = convertToNumber(right);
                switch (op.Trim()) {
                    case "^":
                        _childs[opIdx - 1] = Math.Pow(dleft, dright);
                        break;
                    case "*":
                        _childs[opIdx - 1] = dleft * dright;
                        break;
                    case "/":
                        _childs[opIdx - 1] = dleft / dright;
                        break;
                    case "\\":
                        _childs[opIdx - 1] = dleft % dright;
                        break;
                    case "+":
                        _childs[opIdx - 1] = dleft + dright;
                        break;
                    case "-":
                        _childs[opIdx - 1] = dleft - dright;
                        break;
                }
                _childs.RemoveAt(opIdx);
                _childs.RemoveAt(opIdx);
            }
        }
        /// <summary>
        /// Define a case-insensitive function to calculate value(s).
        /// </summary>
        public abstract class CalcFunction {
            string _name = "";
            string _code = "_code";
            string _desc = "";
            CalcFunctionCollection _list = null;
            public CalcFunction() { }
            /// <summary>
            /// Gets or sets the function's code used in calculation.
            /// </summary>
            public string Code {
                get { return _code; }
                internal set {
                    if (validateCode(value)) {
                        _code = value;
                    }
                }
            }
            /// <summary>
            /// Gets or sets the human readable name of the function.
            /// </summary>
            public string Name {
                get { return _name; }
                internal set { _name = value; }
            }
            /// <summary>
            /// Gets or sets the description of the function.
            /// </summary>
            public string Description {
                get { return _desc; }
                internal set { _desc = value; }
            }
            /// <summary>
            /// Gets or sets the list where the function is.
            /// </summary>
            internal CalcFunctionCollection List {
                get { return _list; }
                set { _list = value; }
            }
            /// <summary>
            /// Returns a number as a result of calculation of value(s).
            /// </summary>
            /// <param name="value">An object represent the value(s) to be calculated using the function.</param>
            /// <returns></returns>
            public abstract object calculate(Bracket b);
            /// <summary>
            /// Evaluates whether a value is valid for the function.
            /// </summary>
            /// <param name="value">A value to be evaluated for the function.</param>
            /// <returns></returns>
            public abstract bool evaluate(Bracket b);
            /// <summary>
            /// Evaluates a given code that matches with function code.
            /// </summary>
            /// <param name="code">A string to be evaluated.</param>
            /// <returns>True, if the given code is equal with function code, false, otherwise.</returns>
            public bool evaluateCode(string code) {
                return string.Compare(_code, code.Trim(), true) == 0;
            }
            /// <summary>
            /// Checks that a given string is valid for code naming of a function.
            /// </summary>
            /// <param name="code">A string to be checks.</param>
            /// <returns>True, if the string is valid for code naming, false, otherwise.</returns>
            private bool validateCode(string code) {
                bool valid = true;
                valid = code.Trim() != "";
                if (valid) {
                    // Test for invalid character.
                    foreach (char c in INVALID_CHARS) {
                        if (code.Trim().Contains(c.ToString().Trim())) {
                            valid = false;
                            break;
                        }
                    }
                }
                if (valid) {
                    // Check the code in built in function's codes.
                    bool found = false;
                    foreach (CalcFunction f in _builtin_functions) {
                        if (f.evaluateCode(code)) {
                            found = true;
                            break;
                        }
                    }
                    valid = !found;
                }
                if (valid) { 
                    // Check if code contains '_' only.
                    string test = code.Replace("_", "");
                    valid = test != "";
                }
                // Check if the code is a number.
                if (valid) valid = !isNumber(code.Trim());
                if (valid) {
                    if (_list != null) {
                        // Check if the code is already registered.
                        bool found = false;
                        foreach (CalcFunction f in _list) {
                            if (f.evaluateCode(code)) {
                                found = true;
                                break;
                            }
                        }
                        valid = !found;
                    }
                }
                return valid;
            }
        }
        /// <summary>
        /// Represent the collection of CalcFunction object.
        /// </summary>
        public class CalcFunctionCollection : CollectionBase {
            #region Constructor
            public CalcFunctionCollection() : base() { }
            #endregion
            #region Public Members
            /// <summary>
            /// Gets a CalcFunction object in the collection specified by its index.
            /// </summary>
            /// <param name="index">The index of the item in the collection to get.</param>
            /// <returns>A CalcFunction object located at the specified index within the collection.</returns>
            public CalcFunction this[int index] {
                get {
                    if (index >= 0 && index < List.Count) return (CalcFunction)List[index];
                    return null;
                }
            }
            /// <summary>
            /// Gets a CalcFunction object in the collection specified by its code-name.
            /// </summary>
            /// <param name="code">The code-name of the function in the collection to get.</param>
            /// <returns>A CalcFunction object that have specified code-name within the collection.</returns>
            public CalcFunction this[string code] {
                get {
                    foreach (CalcFunction f in List) {
                        if (string.Compare(f.Code.Trim(), code.Trim(), true) == 0) return f;
                    }
                    return null;
                }
            }
            /// <summary>
            /// Returns the index within the collection of the specified function.
            /// </summary>
            /// <param name="function">A CalcFunction object representing the function to locate in the collection.</param>
            /// <returns>The zero-based index where the function is located within the collection; otherwise, negative one (-1).</returns>
            public int IndexOf(CalcFunction function) { return List.IndexOf(function); }
            /// <summary>
            /// Adds a CalcFunction into the list.
            /// </summary>
            /// <param name="function">A CalcFunction object to be added into the collection.</param>
            /// <returns>A CalcFunction object if the operation is succeeded, null, otherwise.</returns>
            public CalcFunction Add(CalcFunction function) {
                if (!List.Contains(function)) {
                    bool found = false;
                    foreach (CalcFunction f in List) {
                        if (string.Compare(f.Code.Trim(), function.Code.Trim(), true) == 0) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        int index = List.Add(function);
                        function.List = this;
                        return (CalcFunction)List[index];
                    }
                }
                return null;
            }
            /// <summary>
            /// Adds the elements of the specified collection to the end of the collection.
            /// </summary>
            /// <param name="functions">CalcFunctionCollection object to be added to the end of the collection.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
            public void AddRange(CalcFunctionCollection functions) {
                foreach (CalcFunction f in functions) this.Add(f);
            }
            /// <summary>
            /// Inserts an element into the collection at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which item should be inserted.</param>
            /// <param name="function">The CalcFunction object to insert.</param>
            public void Insert(int index, CalcFunction function) {
                if (!List.Contains(function)) {
                    bool found = false;
                    foreach (CalcFunction f in List) {
                        if (string.Compare(f.Code.Trim(), function.Code.Trim(), true) == 0) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) List.Insert(index, function);
                }
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the collection.
            /// </summary>
            /// <param name="function">The object to remove from the collection.</param>
            /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
            public bool Remove(CalcFunction function) {
                if (List.Contains(function)) {
                    List.Remove(function);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the collection.
            /// </summary>
            /// <param name="functionCode">The code of the function to remove from the collection.</param>
            /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
            public bool Remove(string functionCode) {
                CalcFunction fx = null;
                foreach (CalcFunction f in List) {
                    if (string.Compare(f.Code.Trim(), functionCode.Trim(), true) == 0) {
                        fx = f;
                        break;
                    }
                }
                if (fx != null) {
                    List.Remove(fx);
                    return true;
                }
                return false;
            }
            /// <summary>
            /// Determines whether an element is in the collection.
            /// </summary>
            /// <param name="function">The object to locate in the collection.</param>
            /// <returns>true if function is found in the collection; otherwise, false</returns>
            public bool Contains(CalcFunction function) { return List.Contains(function); }
            /// <summary>
            /// Determines whether a function with specified code-name is in the collection.
            /// </summary>
            /// <param name="functionCode">The code-name of the function to locate in the collection.</param>
            /// <returns>true if function is found in the collection; otherwise, false</returns>
            public bool Contains(string functionCode) {
                foreach (CalcFunction f in List) {
                    if (string.Compare(f.Code.Trim(), functionCode.Trim(), true) == 0) return true;
                }
                return false;
            }
            #endregion
            #region Overriden Methods
            protected override void OnValidate(object value) {
                if (!typeof(CalcFunction).IsAssignableFrom(value.GetType())) throw new ArgumentException("Value must Ai.Common.Calc.CalcFunction", "value");
            }
            #endregion
        }
        /// <summary>
        /// Encapsulates standard mathematics function.
        /// </summary>
        public sealed class CalcFunctions {
            // Standard evaluation for single value.
            private static bool evaluateSingle(Bracket b) {
                if (b == null) return false;
                if (!b.Valid) return false;
                if (b.contains(LIST_SEPARATOR.ToString())) return false;
                return true;
            }
            // User defined function collection.
            public static CalcFunctionCollection UserFunctions = new CalcFunctionCollection();
            // Abs function
            private class _Abs : CalcFunction {
                public _Abs() : base() {
                    Code = "Abs";
                    Name = "Absolute";
                    Description = "Calculates the absolute value of a specified number.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE.ToString().Trim())) return false;
                        return true;
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        double d = convertToNumber(b.Childs[0]);
                        return Math.Abs(d);
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the absolute value of a specified number.
            /// </summary>
            public static CalcFunction Abs { get { return new _Abs(); } }
            // ACos function
            private class _ACos : CalcFunction {
                public _ACos() : base() {
                    Code = "ACos";
                    Name = "ACos";
                    Description = "Calculates the angle whose cosine is the specified number.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE.ToString().Trim())) return false;
                        double d = convertToNumber(b.Childs[0]);
                        if (d < -1 || d > 1) return false;
                        return true;
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        double d = convertToNumber(b.Childs[0]);
                        return Math.Acos(d);
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the angle whose cosine is the specified number.
            /// </summary>
            public static CalcFunction ACos { get { return new _ACos(); } }
            // ASin function
            private class _ASin : CalcFunction {
                public _ASin() : base() {
                    Code = "ASin";
                    Name = "ASin";
                    Description = "Calculates the angle whose sine is the specified number.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE.ToString().Trim())) return false;
                        double d = convertToNumber(b.Childs[0]);
                        if (d < -1 || d > 1) return false;
                        return true;
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        double d = convertToNumber(b.Childs[0]);
                        return Math.Acos(d);
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the angle whose sine is the specified number.
            /// </summary>
            public static CalcFunction ASin { get { return new _ASin(); } }
            // ATan function
            private class _ATan : CalcFunction {
                public _ATan() : base() {
                    Code = "ATan";
                    Name = "ATan";
                    Description = "Calculates the angle whose tangent is the specified number.";
                }
                public override bool evaluate(Bracket b) {
                    return evaluateSingle(b);
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        if (b.Childs[0].ToString() == UNLIMITED_VALUE.ToString() || b.Childs[0].ToString() == "-" + UNLIMITED_VALUE) {
                            if (b.Childs[0].ToString() == UNLIMITED_VALUE.ToString()) return 0d;
                            else return Math.PI;
                        } else {
                            double d = convertToNumber(b.Childs[0]);
                            return Math.Atan(d);
                        }
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the angle whose tangent is the specified number.
            /// </summary>
            public static CalcFunction ATan { get { return new _ATan(); } }
            // Cos function
            private class _Cos : CalcFunction {
                public _Cos() : base() {
                    Code = "Cos";
                    Name = "Cos";
                    Description = "Returns the cosine of the specified angle.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE.ToString().Trim())) return false;
                        return true;
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        double d = convertToNumber(b.Childs[0]);
                        return Math.Cos(d);
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the cosine of the specified angle.
            /// </summary>
            public static CalcFunction Cos { get { return new _Cos(); } }
            // Sin function
            private class _Sin : CalcFunction { 
                public _Sin() : base() {
                    Code = "Sin";
                    Name = "Sin";
                    Description = "Returns the sine of the specified angle.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE.ToString().Trim())) return false;
                        return true;
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        double d = convertToNumber(b.Childs[0]);
                        return Math.Sin(d);
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the sine of the specified angle.
            /// </summary>
            public static CalcFunction Sin { get { return new _Sin(); } }
            // Log function
            private class _Log : CalcFunction {
                public _Log() : base() {
                    Code = "Log";
                    Name = "Log";
                    Description = "Returns the logarithm of a specified number and optionally in a specified base.";
                }
                public override bool evaluate(Bracket b) {
                    if (evaluateSingle(b)) {
                        if (b.contains(UNLIMITED_VALUE)) return false;
                        return true;
                    } else {
                        if (b.contains(LIST_SEPARATOR.ToString())) {
                            if (b.Childs.Length == 3) {
                                return b.contains(UNLIMITED_VALUE.ToString().Trim());
                            }
                            return false;
                        }
                    }
                    return false;
                }
                public override object calculate(Bracket b) {
                    if (evaluate(b)) {
                        if (b.contains(LIST_SEPARATOR.ToString().Trim())) {
                            double a = convertToNumber(b.Childs[0]);
                            double newBase = convertToNumber(b.Childs[2]);
                            return Math.Log(a, newBase);
                        } else {
                            double a = convertToNumber(b.Childs[0]); ;
                            return Math.Log(a);
                        }
                    }
                    return null;
                }
            }
            /// <summary>
            /// Gets a function to calculate the logarithm of a specified number and optionally in a specified base.
            /// </summary>
            public static CalcFunction Log { get { return new _Log(); } }
        }
    }
}