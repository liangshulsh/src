// Ai Software Library.

using System;
using System.Collections.Generic;
using System.Collections;

namespace Ai.Common {
    /// <summary>
    /// Provide an encryption using huffman algorithm.
    /// </summary>
    public sealed class Huffman {
        private class TreeNode {
            string _code = "";
            int _parent = -1;
            int _leftNode = -1;
            int _rightNode = -1;
            uint _count = 0;
            public TreeNode() { }
            public TreeNode(string code, uint count) {
                _code = code;
                _count = count;
            }
            public string Code {
                get { return _code; }
                set { _code = value; }
            }
            public int Parent {
                get { return _parent; }
                set { _parent = value; }
            }
            public int LeftNode {
                get { return _leftNode; }
                set { _leftNode = value; }
            }
            public int RightNode {
                get { return _rightNode; }
                set { _rightNode = value; }
            }
            public uint Count {
                get { return _count; }
                set { _count = value; }
            }
        }
        private class TreeComparer : IComparer<TreeNode> {
            #region IComparer<TreeNode> Members
            public int Compare(TreeNode x, TreeNode y) {
                if (x != null) {
                    if (y != null) {
                        if (x.Count > y.Count) {
                            return 1;
                        } else {
                            if (x.Count < y.Count) return -1;
                            else return string.Compare(x.Code, y.Code, false);
                        }
                    }
                }
                return 0;
            }
            #endregion
        }
        List<TreeNode> tree = new List<TreeNode>();
        public Huffman() { 
            tree.Add(new TreeNode(Convert.ToChar(32).ToString(), 354));
            tree.Add(new TreeNode(Convert.ToChar(33).ToString(), 1079));
            tree.Add(new TreeNode(Convert.ToChar(34).ToString(), 680));
            tree.Add(new TreeNode(Convert.ToChar(35).ToString(), 46));
            tree.Add(new TreeNode(Convert.ToChar(36).ToString(), 50));
            tree.Add(new TreeNode(Convert.ToChar(37).ToString(), 22));
            tree.Add(new TreeNode(Convert.ToChar(38).ToString(), 151));
            tree.Add(new TreeNode(Convert.ToChar(39).ToString(), 973));
            tree.Add(new TreeNode(Convert.ToChar(40).ToString(), 368));
            tree.Add(new TreeNode(Convert.ToChar(41).ToString(), 399));
            tree.Add(new TreeNode(Convert.ToChar(42).ToString(), 28));
            tree.Add(new TreeNode(Convert.ToChar(43).ToString(), 31));
            tree.Add(new TreeNode(Convert.ToChar(44).ToString(), 5194));
            tree.Add(new TreeNode(Convert.ToChar(45).ToString(), 6465));
            tree.Add(new TreeNode(Convert.ToChar(46).ToString(), 13148));
            tree.Add(new TreeNode(Convert.ToChar(47).ToString(), 11579));
            tree.Add(new TreeNode(Convert.ToChar(48).ToString(), 2125));
            tree.Add(new TreeNode(Convert.ToChar(49).ToString(), 1895));
            tree.Add(new TreeNode(Convert.ToChar(50).ToString(), 1597));
            tree.Add(new TreeNode(Convert.ToChar(51).ToString(), 1285));
            tree.Add(new TreeNode(Convert.ToChar(52).ToString(), 1119));
            tree.Add(new TreeNode(Convert.ToChar(53).ToString(), 1513));
            tree.Add(new TreeNode(Convert.ToChar(54).ToString(), 1025));
            tree.Add(new TreeNode(Convert.ToChar(55).ToString(), 983));
            tree.Add(new TreeNode(Convert.ToChar(56).ToString(), 777));
            tree.Add(new TreeNode(Convert.ToChar(57).ToString(), 928));
            tree.Add(new TreeNode(Convert.ToChar(58).ToString(), 3509));
            tree.Add(new TreeNode(Convert.ToChar(59).ToString(), 182));
            tree.Add(new TreeNode(Convert.ToChar(60).ToString(), 48));
            tree.Add(new TreeNode(Convert.ToChar(61).ToString(), 2154));
            tree.Add(new TreeNode(Convert.ToChar(62).ToString(), 474));
            tree.Add(new TreeNode(Convert.ToChar(63).ToString(), 2492));
            tree.Add(new TreeNode(Convert.ToChar(64).ToString(), 65));
            tree.Add(new TreeNode(Convert.ToChar(65).ToString(), 1609));
            tree.Add(new TreeNode(Convert.ToChar(66).ToString(), 724));
            tree.Add(new TreeNode(Convert.ToChar(67).ToString(), 963));
            tree.Add(new TreeNode(Convert.ToChar(68).ToString(), 582));
            tree.Add(new TreeNode(Convert.ToChar(69).ToString(), 604));
            tree.Add(new TreeNode(Convert.ToChar(70).ToString(), 673));
            tree.Add(new TreeNode(Convert.ToChar(71).ToString(), 599));
            tree.Add(new TreeNode(Convert.ToChar(72).ToString(), 648));
            tree.Add(new TreeNode(Convert.ToChar(73).ToString(), 2722));
            tree.Add(new TreeNode(Convert.ToChar(74).ToString(), 226));
            tree.Add(new TreeNode(Convert.ToChar(75).ToString(), 203));
            tree.Add(new TreeNode(Convert.ToChar(76).ToString(), 2593));
            tree.Add(new TreeNode(Convert.ToChar(77).ToString(), 764));
            tree.Add(new TreeNode(Convert.ToChar(78).ToString(), 731));
            tree.Add(new TreeNode(Convert.ToChar(79).ToString(), 506));
            tree.Add(new TreeNode(Convert.ToChar(80).ToString(), 808));
            tree.Add(new TreeNode(Convert.ToChar(81).ToString(), 34));
            tree.Add(new TreeNode(Convert.ToChar(82).ToString(), 1108));
            tree.Add(new TreeNode(Convert.ToChar(83).ToString(), 1158));
            tree.Add(new TreeNode(Convert.ToChar(84).ToString(), 1636));
            tree.Add(new TreeNode(Convert.ToChar(85).ToString(), 865));
            tree.Add(new TreeNode(Convert.ToChar(86).ToString(), 238));
            tree.Add(new TreeNode(Convert.ToChar(87).ToString(), 969));
            tree.Add(new TreeNode(Convert.ToChar(88).ToString(), 64));
            tree.Add(new TreeNode(Convert.ToChar(89).ToString(), 158));
            tree.Add(new TreeNode(Convert.ToChar(90).ToString(), 42));
            tree.Add(new TreeNode(Convert.ToChar(91).ToString(), 6526));
            tree.Add(new TreeNode(Convert.ToChar(92).ToString(), 15));
            tree.Add(new TreeNode(Convert.ToChar(93).ToString(), 6096));
            tree.Add(new TreeNode(Convert.ToChar(94).ToString(), 33));
            tree.Add(new TreeNode(Convert.ToChar(95).ToString(), 187));
            tree.Add(new TreeNode(Convert.ToChar(96).ToString(), 2));
            tree.Add(new TreeNode(Convert.ToChar(97).ToString(), 47625));
            tree.Add(new TreeNode(Convert.ToChar(98).ToString(), 9034));
            tree.Add(new TreeNode(Convert.ToChar(99).ToString(), 18282));
            tree.Add(new TreeNode(Convert.ToChar(100).ToString(), 21677));
            tree.Add(new TreeNode(Convert.ToChar(101).ToString(), 63199));
            tree.Add(new TreeNode(Convert.ToChar(102).ToString(), 12824));
            tree.Add(new TreeNode(Convert.ToChar(103).ToString(), 14122));
            tree.Add(new TreeNode(Convert.ToChar(104).ToString(), 26718));
            tree.Add(new TreeNode(Convert.ToChar(105).ToString(), 38286));
            tree.Add(new TreeNode(Convert.ToChar(106).ToString(), 1344));
            tree.Add(new TreeNode(Convert.ToChar(107).ToString(), 5312));
            tree.Add(new TreeNode(Convert.ToChar(108).ToString(), 30037));
            tree.Add(new TreeNode(Convert.ToChar(109).ToString(), 15942));
            tree.Add(new TreeNode(Convert.ToChar(110).ToString(), 37221));
            tree.Add(new TreeNode(Convert.ToChar(111).ToString(), 45603));
            tree.Add(new TreeNode(Convert.ToChar(112).ToString(), 15214));
            tree.Add(new TreeNode(Convert.ToChar(113).ToString(), 549));
            tree.Add(new TreeNode(Convert.ToChar(114).ToString(), 36812));
            tree.Add(new TreeNode(Convert.ToChar(115).ToString(), 31819));
            tree.Add(new TreeNode(Convert.ToChar(116).ToString(), 52829));
            tree.Add(new TreeNode(Convert.ToChar(117).ToString(), 19832));
            tree.Add(new TreeNode(Convert.ToChar(118).ToString(), 6290));
            tree.Add(new TreeNode(Convert.ToChar(119).ToString(), 13407));
            tree.Add(new TreeNode(Convert.ToChar(120).ToString(), 2276));
            tree.Add(new TreeNode(Convert.ToChar(121).ToString(), 10449));
            tree.Add(new TreeNode(Convert.ToChar(122).ToString(), 1134));
            tree.Add(new TreeNode(Convert.ToChar(123).ToString(), 1416));
            tree.Add(new TreeNode(Convert.ToChar(124).ToString(), 706));
            tree.Add(new TreeNode(Convert.ToChar(125).ToString(), 1416));
            tree.Add(new TreeNode(Convert.ToChar(126).ToString(), 8));
            createTree();
        }
        private void createTree() {
            TreeNode pNode;
            int lIndex, rIndex;
            uint minCount;
            bool quit = false;
            lIndex = 0;
            rIndex = 0;
            while (!quit) {
                minCount = 0;
                foreach (TreeNode tn in tree) {
                    if (tree.IndexOf(tn) != lIndex) {
                        if (tn.Parent == -1) {
                            if (minCount == 0) {
                                rIndex = tree.IndexOf(tn);
                                minCount = tn.Count;
                            } else {
                                if (tn.Count < minCount) {
                                    rIndex = tree.IndexOf(tn);
                                    minCount = tn.Count;
                                }
                            }
                        }
                    }
                }
                if (minCount == 0) {
                    quit = true;
                } else {
                    pNode = new TreeNode("", tree[lIndex].Count + tree[rIndex].Count);
                    pNode.LeftNode = lIndex;
                    pNode.RightNode = rIndex;
                    tree[lIndex].Parent = tree.Count;
                    tree[rIndex].Parent = tree.Count;
                    tree.Add(pNode);
                    minCount = 0;
                    foreach (TreeNode tn in tree) {
                        if (tn.Parent == -1) {
                            if (minCount == 0) {
                                lIndex = tree.IndexOf(tn);
                                minCount = tn.Count;
                            } else {
                                if (tn.Count < minCount) {
                                    lIndex = tree.IndexOf(tn);
                                    minCount = tn.Count;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Encrypt a given string using huffman algorithm.
        /// </summary>
        /// <param name="s">A string value to be encrypted.</param>
        /// <returns>A string object represent the encrypted string.</returns>
        public string encrypt(string s) {
            string result = "";
            string binary = "";
            int i = 0;
            string strHex = "";
            while (i < s.Length) {
                binary = binary + encryptChar(s.Substring(i, 1));
                i++;
            }
            if (binary.Length % 8 > 0) {
                string addChar;
                if (binary.Substring(binary.Length - 1, 1) == "1") addChar = "0";
                else addChar = "1";
                while (binary.Length % 8 > 0) binary = binary + addChar;
                result = "1";
            } else {
                result = "0";
            }
            i = 0;
            while (i < binary.Length) {
                strHex = Convert.ToString(Convert.ToInt32(binary.Substring(i, 8), 2), 16);
                if (strHex.Length == 1) strHex = "0" + strHex;
                result = result + strHex;
                i += 8;
            }
            return result;
        }
        /// <summary>
        /// Decrypt a string that has been encrypted using huffman algorithm.
        /// </summary>
        /// <param name="s">An encrypted string.</param>
        /// <returns>A decrypted string from an encrypted string.</returns>
        public string decrypt(string s) {
            string result = "";
            int hexVal = 0;
            string hexBin = "";
            string binary = "";
            int i = 1;
            while (i < s.Length) { 
                hexVal = Convert.ToInt32(s.Substring(i, 2), 16);
                hexBin = Common.binary(hexVal);
                binary = binary + hexBin;
                i = i + 2;
            }
            if (s.Substring(0, 1) == "1") {
                int addCount = 1;
                string addChar = binary.Substring(binary.Length - 1, 1);
                while (binary.Substring(binary.Length - addCount, 1) == addChar) addCount++;
                addCount -= 1;
                binary = binary.Remove(binary.Length - addCount);
            }
            i = 0;
            while (i < binary.Length) result = result + getCode(binary, ref i);
            return result;
        }
        /// <summary>
        /// Sets the probability value of a character.
        /// </summary>
        /// <param name="c">The character that have the value.</param>
        /// <param name="value">The probability value of the character.</param>
        public void setCharValue(char c, uint value) {
            bool found = false;
            TreeNode newTn = null;
            foreach (TreeNode tn in tree) {
                if (tn.Code == c.ToString()) {
                    newTn = tn;
                    found = true;
                    break;
                }
            }
            if (!found) {
                newTn = new TreeNode(c.ToString(), value);
                tree.Add(newTn);
            }
            newTn.Count = value;
        }
        /// <summary>
        /// Gets the currently used probability value of a character.
        /// </summary>
        /// <param name="c">Character to be search.</param>
        /// <returns>An unsigned integer value represent the probability value of the character.</returns>
        public uint getCharValue(char c) {
            foreach (TreeNode tn in tree) {
                if (tn.Code == c.ToString()) return tn.Count;
            }
            return 0;
        }
        /// <summary>
        /// Clears all used character and its probabilities.
        /// </summary>
        public void clearProbabilities() {
            tree.Clear();
        }
        /// <summary>
        /// Recreate the tree to be used in encryption and decription.
        /// </summary>
        public void recreateTree() {
            createTree();
        }
        private string encryptChar(string c) {
            string binary = "";
            int nodeIndex = -1;
            TreeNode tn;
            foreach (TreeNode t in tree) {
                if (t.Code == c) {
                    nodeIndex = tree.IndexOf(t);
                    break;
                }
            }
            if (nodeIndex > -1) {
                tn = tree[nodeIndex];
                while (tn.Parent > -1) {
                    if (nodeIndex == tree[tn.Parent].LeftNode) binary = "1" + binary;
                    else binary = "0" + binary;
                    tn = tree[tn.Parent];
                    nodeIndex = tree.IndexOf(tn);
                }
            }
            return binary;
        }
        private string getCode(string binary, ref int startIndex) {
            string result = "";
            TreeNode cn = tree[tree.Count - 1];
            bool quit = startIndex >= binary.Length;
            while (!quit) {
                if (binary.Substring(startIndex, 1) == "1") {
                    if (cn.LeftNode > -1) {
                        cn = tree[cn.LeftNode];
                        startIndex += 1;
                        if (startIndex >= binary.Length) {
                            result = cn.Code;
                            quit = true;
                        }
                    } else {
                        result = cn.Code;
                        quit = true;
                    }
                } else {
                    if (cn.RightNode > -1) {
                        cn = tree[cn.RightNode];
                        startIndex += 1;
                        if (startIndex >= binary.Length) {
                            result = cn.Code;
                            quit = true;
                        }
                    } else {
                        result = cn.Code;
                        quit = true;
                    }
                }
            }
            return result;
        }
    }
}