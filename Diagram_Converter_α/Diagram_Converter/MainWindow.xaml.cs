using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace Diagram_Converter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly int containerHeadHeight = 30;
        private readonly int columnTextHeight = 30;
        private readonly int classContainerHeadHeight = 25;
        private readonly int classContainerWidth = 250;
        private readonly int classContainerPadding = 9;
        private readonly int classContainerFontHeight = 14;
        private readonly int classLineHeight = 8;

        class queryData
        {
            public string tableName;
            public List<string> columnList = new List<string>();
        }

        class classData
        {
            public string name;
            public List<variableData> fieldList = new List<variableData>();
            public List<methodData> methodList = new List<methodData>();
        }

        class variableData
        {
            public string name;
            public string value;
            public string type;
            public string access;
            public string property;
            public string modifier;

            public variableData() { }

            public variableData(string name, string value, string type, string access, string property, string modifier)
            {
                this.name = name;
                this.value = value;
                this.type = type;
                this.access = access;
                this.property = property;
                this.modifier = modifier;
            }
        }

        class methodData
        {
            public string name;
            public List<variableData> args = new List<variableData>();
            public string type;
            public string access;

            public methodData() { }

            public methodData(string name, List<variableData> args, string type, string access)
            {
                this.name = name;
                this.args = args;
                this.type = type;
                this.access = access;
            }
        }


        private void SelectStatementInput(object sender, TextChangedEventArgs e)
        {
            string sorce = SelectStatement.Text;
            string outputText;
            string defaultStringHead = "<mxGraphModel><root><mxCell id =\"0\"/><mxCell id =\"1\" parent =\"0\"/>";
            string defaultStringFoot = "</root></mxGraphModel>";
            queryData analysisDatas = ConvertQueryData(sorce);
            string container;
            string columnsText = "";
            int columnCount = analysisDatas.columnList.Count;
            int containerHeight;

            
            //コンテナを出力
            containerHeight = containerHeadHeight + (columnCount * columnTextHeight);
            container = Create_ERContainer(analysisDatas.tableName, containerHeight);

            //カラムを出力
            int id = 3;
            foreach(string column in analysisDatas.columnList.ToArray())
            {
                columnsText = columnsText + Create_ColumnText(column, id);
                id++;
            }
            


            outputText = defaultStringHead + container + columnsText + defaultStringFoot;
            drawIO_ERDiagramText.Text = outputText;
        }


        private void SorceCodeInput(object sender, TextChangedEventArgs e)
        {
            //ソースコード解釈メソッド変更
            if (SelectedCSharp.IsChecked == true)
            {
                SorceCodeConvert_CSharp();
            }
            else
            {
                SorceCodeConvert_JS();
            }
        }

        private void SorceCodeConvert_CSharp()
        {
            string defaultStringHead = "<mxGraphModel><root><mxCell id =\"0\"/><mxCell id =\"1\" parent =\"0\"/>";
            string classDiaglamsString = "";
            string defaultStringFoot = "</root></mxGraphModel>";

            string sorceCode = SorceCode.Text;
            Regex HierarchyRegex = new Regex(@"""[^\s\(\)\{\}]*[\s\(\)\{\}]+[^\s\(\)\{\}]*""");//文字列内の括弧　誤検知防止
            Regex CommentRegex = new Regex(@"//[^\n\r|\n|\r]*[\n\r|\n|\r]*");//コメント　誤検知防止

            sorceCode = HierarchyRegex.Replace(sorceCode, "_");
            sorceCode = sorceCode.Replace(Environment.NewLine, "");
            sorceCode = sorceCode.Replace("\t", "");

            string[] lines = Regex.Split(sorceCode, "(?<=;)|(?<={)|(?<=})");
            int MethodReadingHierarchy = -1;

            const int classModelInnerObjectCount = 4;
            int classDiaglamID = 2;
            int hierarchy = -1; //クラス内の階層を0基準にし、ソースコード本体の階層を-1と定義
            Hashtable classDataList = new Hashtable();

            foreach (string line in lines)
            {
                string[] sentences = Regex.Split(line, "(?<= )");

                int classSenetenceIndex = Array.IndexOf(sentences, "class ");
                int openIndex = Array.IndexOf(sentences, Array.Find(sentences, (s) => { return s.IndexOf("{") != -1; }));
                int closeIndex = Array.IndexOf(sentences, Array.Find(sentences, (s) => { return s.IndexOf("}") != -1; }));
                int semicolonIndex = Array.IndexOf(sentences, Array.Find(sentences, (s) => { return s.IndexOf(";") != -1; }));
                int GetterSetterHierarchy = -1;

                //開き括弧があれば階層を深くする
                if (openIndex >= 0)
                {
                    hierarchy++;
                    string parentHierarchy = (hierarchy - 1).ToString();

                    
                    if (MethodReadingHierarchy < 0)
                    {
                        for (int i = openIndex - 1; i >= 0; i--)
                        {
                            if (sentences[i] == " " || sentences[i] == "　")
                            {
                                continue;
                            }
                           
                            //空白を除いた１つ前の文字が　)　の場合、関数とみなす(既に関数が見つかってない場合)
                            if (Regex.IsMatch(sentences[i], @"[^)]\)\s*"))
                            {
                                classData parent = classDataList[parentHierarchy] as classData;
                                MethodReadingHierarchy = hierarchy;
                                if (classDataList[parentHierarchy] != null)
                                {
                                    string argLine = line;
                                    //コンストラクタをConvertToMethodDataに対応した文章フォーマットに書き換え
                                    Regex ConstructorRegex = new Regex(parent.name.Trim() + @"\s*\(");
                                    Console.WriteLine(parent.name.Trim() + @"\s*\(");
                                    argLine = ConstructorRegex.Replace(argLine, "void " + parent.name + "(");

                                    parent.methodList.Add(ConvertToMethodData(argLine));
                                }
                            }
                            break;
                        }
                    }

                }

                //class(クラス宣言)があれば出力クラスリストに追加し、階層を深くする
                if (classSenetenceIndex >= 0)
                {
                    classData addData = new classData();
                    addData.name = sentences[classSenetenceIndex + 1];
                    classDataList[hierarchy.ToString()] = addData;
                }

                //※Property句があればプロパティ読み込み状態になる


                //セミコロンがあればフィールドとする(関数内除く)
                if (semicolonIndex >= 0 && MethodReadingHierarchy < 0)
                {
                    if (classDataList[hierarchy.ToString()] != null)
                    {
                        classData parent = classDataList[hierarchy.ToString()] as classData;
                        parent.fieldList.Add(ConvertToVariableData(line));
                    }
                }

                //閉じ括弧があれば階層を浅くする
                if (closeIndex >= 0)
                {
                    //関数定義の閉じ括弧だった場合は関数読み込み状態をリセットする
                    if (MethodReadingHierarchy == hierarchy)
                    {
                        MethodReadingHierarchy = -1;
                    }

                    //クラス定義の閉じ括弧だった場合はクラス図を出力する
                    if(classDataList[hierarchy.ToString()] != null)
                    {
                        classDiaglamsString += CreateClassModel(classDataList[hierarchy.ToString()] as classData, classDiaglamID);
                        classDataList.Remove(hierarchy.ToString());//出力したクラスを削除
                        classDiaglamID += classModelInnerObjectCount;
                    }
                    hierarchy--;
                }

            }

            drawIO_ClassDiagramText.Text = defaultStringHead + classDiaglamsString + defaultStringFoot;
        }

        private variableData ConvertToVariableData(string line)
        {
            variableData output = new variableData();
            line = line.Trim();
            output.access = "";

            string[] sentences = line.Split(' ', ',');

            string analysisMode = "access";
            foreach (string sentence in sentences)
            {
                if (sentence == "")
                {
                    continue;
                }

                if (sentence == "private" || sentence == "protected" || sentence == "public" || sentence == "internal")
                {
                    output.access += sentence;//protected internalやprivate protectedに対応する
                    analysisMode = "type";
                    continue;
                }

                if (sentence == "const" || sentence == "readonly")
                {
                    output.modifier = sentence;
                    analysisMode = "type";
                    continue;
                }

                if (analysisMode == "type")
                {
                    output.type = sentence.Replace(";", " ");
                    analysisMode = "name";
                    continue;
                }

                if (analysisMode == "name")
                {
                    output.name = sentence.Replace(";", " ");
                    analysisMode = "isValue";
                    continue;
                }

                if (analysisMode == "isValue")
                {
                    if (sentence == "=")
                    {
                        analysisMode = "Value";
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (analysisMode == "Value")
                {
                    output.value += (sentence.Replace(";", " ") + " ");
                    continue;
                }

            }



            return output;
        }


        private methodData ConvertToMethodData(string line)
        {
            methodData output = new methodData();
            line = line.Trim();
            line = line.Replace("(", " ");
            line = line.Replace(")", " ");
            line = line.Replace("{", " ");
            output.access = "";

            string[] sentences = line.Split(' ', ',');

            string analysisMode = "access";
            variableData addData = null;
            foreach (string sentence in sentences)
            {
                if (sentence == "")
                {
                    continue;
                }

                if (sentence == "private" || sentence == "protected" || sentence == "public" || sentence == "internal")
                {
                    output.access += sentence;//protected internalやprivate protectedに対応する
                    analysisMode = "type";
                    continue;
                }

                if(analysisMode == "type")
                {
                    output.type = sentence;
                    analysisMode = "name";
                    continue;
                }

                if(analysisMode == "name")
                {
                    output.name = sentence;
                    analysisMode = "argType";
                    continue;
                }

                if (analysisMode == "isInit")
                {
                    if (sentence == "=")
                    {
                        analysisMode = "argValue";
                        continue;
                    }
                    else
                    {
                        output.args.Add(addData);
                        analysisMode = "argType";
                    }
                }

                if (analysisMode == "argType")
                {
                    addData = new variableData();
                    addData.type = sentence;
                    analysisMode = "argName";
                    continue;
                }

                if(analysisMode == "argName")
                {
                    addData.name = sentence;
                    analysisMode = "isInit";
                    continue;
                }

                if(analysisMode == "argValue")
                {
                    addData.value = sentence;
                    output.args.Add(addData);
                    analysisMode = "argType";
                    continue;
                }
                
            }
            return output;
        }


        private string CreateClassModel(classData analysisData, int rootID)
        {
            string output;
            
            int fieldHeight = (classContainerPadding * 2) + (classContainerFontHeight * analysisData.fieldList.Count);
            int methodHeight = (classContainerPadding * 2) + (classContainerFontHeight * analysisData.methodList.Count);
            int lineLocation_Y = classContainerHeadHeight + fieldHeight;

            string container;
            string field = CreateFields(analysisData, rootID, fieldHeight);
            string line = CreateClassDiagramLine(rootID, lineLocation_Y);
            string method = CreateMethods(analysisData, rootID, lineLocation_Y + classLineHeight, methodHeight);
            

            container = CreateClassContainer(analysisData, rootID, lineLocation_Y + methodHeight + classLineHeight);
            output = container + field + line + method;

            return output;
        }

        private string CreateClassContainer(classData analysisData, int rootID, int height)
        {
            string output;
            string mxCellHead = $"<mxCell id=\"{rootID}\" value=\"{analysisData.name}\" style=\"swimlane; fontStyle = 1; align = center; verticalAlign = top; childLayout = stackLayout; horizontal = 1; startSize = 26; horizontalStack = 0; resizeParent = 1; resizeParentMax = 0; resizeLast = 0; collapsible = 1; marginBottom = 0; \" vertex=\"1\" parent=\"1\">";
            string mxGeometry = $"<mxGeometry x=\"0\" y=\"0\" width=\"{classContainerWidth}\" height=\"{height}\" as=\"geometry\" />";
            string mxCellFoot = "</mxCell>";

            output = mxCellHead + mxGeometry + mxCellFoot;
            return output;
        }

        private string CreateClassDiagramLine(int rootID, int lineLocation_Y)
        {
            string output;
            string mxCellLineHead = $"<mxCell id=\"{rootID + 2}\" value=\"\" style=\"line;strokeWidth=1;fillColor = none; align = left; verticalAlign = middle; spacingTop = -1; spacingLeft = 3; spacingRight = 3; rotatable = 0; labelPosition = right; points =[]; portConstraint = eastwest; strokeWidth = 1;\" vertex=\"1\" parent=\"{rootID}\">";
            string mxGeometryLine = $"<mxGeometry y=\"{lineLocation_Y}\" width=\"{classContainerWidth}\" height=\"{classLineHeight}\" as=\"geometry\" />";
            string mxCellLineFoot = "</mxCell>";
            output = mxCellLineHead + mxGeometryLine + mxCellLineFoot;
            return output;
        }

        private string CreateMethods(classData analysisData, int rootID, int location_Y, int height)
        {
            string output;
            string methodsString = "";

            int count = 0;
            foreach (methodData method in analysisData.methodList)
            {
                if (count != 0)
                {
                    methodsString += "&#xa;";
                }
                methodsString += CreateMethod(method);
                count++;
            }

            string mxCellHead = $"<mxCell id=\"{rootID + 3}\" value=\"{methodsString}\" style=\"text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;\" vertex=\"1\" parent=\"{rootID}\">";
            string mxGeometry = $"<mxGeometry x=\"0\" y=\"{location_Y}\" width=\"{classContainerWidth}\" height=\"{height}\" as=\"geometry\" />";
            string mxCellFoot = "</mxCell>";

            output = mxCellHead + mxGeometry + mxCellFoot;
            return output;
        }

        private string CreateFields(classData analysisData, int rootID, int height)
        {
            string output;
            string fieldsString = "";

            int count = 0;
            foreach(variableData field in analysisData.fieldList)
            {
                if (count != 0)
                {
                    fieldsString += "&#xa;";
                }
                fieldsString += CreateField(field);
                count++;
            }

            string mxCellHead = $"<mxCell id=\"{rootID + 1}\" value=\"{fieldsString}\" style=\"text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;\" vertex=\"1\" parent=\"{rootID}\">";
            string mxGeometry = $"<mxGeometry x=\"0\" y=\"{classContainerHeadHeight}\" width=\"{classContainerWidth}\" height=\"{height}\" as=\"geometry\" />";
            string mxCellFoot = "</mxCell>";

            output = mxCellHead + mxGeometry + mxCellFoot;
            return output;
        }

        private string CreateMethod(methodData method)
        {
            string output;
            string access = ConvertAccessString(method.access);
            string argsString = "";
            string type = method.type != null ? method.type : "";

            type = type.Replace("<", "&lt;");
            type = type.Replace(">", "&gt;");

            //引数リストを生成
            foreach(variableData arg in method.args)
            {
                string FieldString = CreateField(arg);
                string argString = FieldString.Substring(3, FieldString.Length - 3);//アクセス修飾子を削除
                argsString += argString + ", ";
            }
            if (argsString.Length >= 2)
            {
                argsString = argsString.Substring(0, argsString.Length - 2);
                argsString = argsString.Replace("<", "&lt;");
                argsString = argsString.Replace(">", "&gt;");
            }

            output = $"{access}  {method.name}({argsString})";

            if (type != "void")
            {
                output += " : " + type;
            }            

            return output;
        }


        private string CreateField(variableData field)
        {
            string output;
            string access = ConvertAccessString(field.access);
            string type = field.type != null ? field.type : "";
            string property = "";

            type = type.Replace("<", "&lt;");
            type = type.Replace(">", "&gt;");

            //プロパティの場合
            if(field.property != null && field.property != "")
            {
                property = $"≪{field.property}≫";
            }

            output = $"{access} {property} {field.name} : {type}";


            //初期値がある場合
            if (field.value != null && field.value != "")
            {
                string value = field.value;
                value = value.Replace("<", "&lt;");
                value = value.Replace(">", "&gt;");
                output += " = " + value;
            }

            //その他修飾子がある場合
            if (field.modifier != null && field.modifier != "")
            {
                output += "{" + field.modifier + "}";
            }

            return output;
        }

        private string ConvertAccessString(string accessString)
        {
            string output;
            switch (accessString)
            {
                case "private":
                    output =  "-";
                    break;
                case "public":
                    output = "+";
                    break;
                case "protected":
                    output = "#";
                    break;
                case "package":
                    output = "~";
                    break;
                case "internal":
                    output = "~";
                    break;
                case "protectedinternal":
                    output = "#≪internal≫";
                    break;
                case "privateprotected":
                    output = "#≪private≫";
                    break;
                default:
                    output = "-";
                    break;
            }
            return output;
        }


        private void SorceCodeConvert_JS()
        {
            drawIO_ClassDiagramText.Text = "JS";
        }








        private queryData ConvertQueryData(string selectStatement)
        {
            queryData data = new queryData();
            string[] sentences;

            selectStatement = selectStatement.Replace("[", "");
            selectStatement = selectStatement.Replace("]", "");
            selectStatement = selectStatement.Replace(",", " ");
            selectStatement = selectStatement.Replace(Environment.NewLine, " ");

            sentences = selectStatement.Split(' ');

            string analysisMode = "none";
            foreach (string sentence in sentences)
            {
                //解析部
                if(sentence == "")
                {
                    continue;
                }
                if (sentence.ToUpper() == "SELECT")
                {
                    analysisMode = "select";
                    continue;
                }
                else if (sentence.ToUpper() == "FROM")
                {
                    analysisMode = "from";
                    continue;
                }
                else if (sentence.ToUpper() == "TOP")
                {
                    analysisMode = "top";
                    continue;
                }
                else
                {
                    //制御部
                    if (analysisMode == "top")
                    {
                        analysisMode = "select";
                        continue;
                    }
                    else if(analysisMode == "select")
                    {
                        data.columnList.Add(sentence);
                    }
                    else if(analysisMode == "from")
                    {
                        string[] tableLocations = sentence.Split('.');
                        data.tableName = tableLocations[tableLocations.Length - 1];
                        break;
                    }
                }
            }

            return data;
        }

        private string Create_ERContainer(string value, int height)
        {
            string outputText = "";
            string mxCellHead = $"<mxCell id =\"2\" value =\"{value}\" style =\"swimlane; fontStyle = 0; childLayout = stackLayout; horizontal = 1; startSize = 30; horizontalStack = 0; resizeParent = 1; resizeParentMax = 0; resizeLast = 0; collapsible = 1; marginBottom = 0; swimlaneFillColor = default; \" vertex =\"1\" parent =\"1\">";
            string mxGeometry = $"<mxGeometry x = \"0\" y =\"0\" width =\"310\" height =\"{height}\" as =\"geometry\"/>";
            string mxCellFoot = "</mxCell>";

            outputText = mxCellHead + mxGeometry + mxCellFoot;
            return outputText;
        }

        private string Create_ColumnText(string value, int id)
        {
            string outputText = "";
            string mxCellHead = $"<mxCell id =\"{id}\" value =\"{value}\" style =\"text; strokeColor = none; fillColor = none; align = left; verticalAlign = middle; spacingLeft = 4; spacingRight = 4; overflow = hidden; points =[[0, 0.5],[1,0.5]]; portConstraint = eastwest; rotatable = 0; \" vertex =\"1\" parent =\"2\">";
            string mxGeometry = $"<mxGeometry y =\"{(id - 3) * columnTextHeight + containerHeadHeight}\" width =\"310\" height =\"30\" as =\"geometry\"/>";
            string mxCellFoot = "</mxCell>";
            outputText = mxCellHead + mxGeometry + mxCellFoot;
            return outputText;
        }


        private void ERSentenceCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(drawIO_ERDiagramText.Text);
        }

        private void ClassSentenceCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(drawIO_ClassDiagramText.Text);
        }
    }
}
