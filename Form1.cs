using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        public string text = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            text = richTextBox1.Text.Trim();
            var scanner = new Scanner(text);
        
            var tokens = scanner.Scan();
            string memorytext = "";
            Parser.error = "";
            Error.haderror = false;
            var p = new Parser(tokens);
            var stmts = p.Program();
            richTextBox2.Text = "";
            foreach (var item in tokens)
            {
                richTextBox2.Text += item.type.ToString();
                richTextBox2.Text += '\n';
            }
            if (scanner.unkown)
                scanner.error += "error: Unknown Symbol";
            if (scanner.error != "")
                richTextBox3.Text = scanner.error;

            else if(Parser.error == "")
            {

                var symbols = new Dictionary<string, TokenType>();
                foreach (var stmt in stmts)
                {
                    if(stmt!=null)
                    stmt.Check(symbols);
                }
                if (!Error.haderror)
                {
                    var memory = new Dictionary<string, dynamic>();
                    foreach (var stmt in stmts)
                    {
                        if (stmt != null)
                            stmt.Interpret(memory);
                    }
                    foreach (var variable in memory.Keys)
                    {
                        memorytext += $"{variable} = {memory[variable]}";
                        memorytext += "\n";
                    }
                }

                
            }
            if (Parser.error == "")
                richTextBox3.Text = "✓ No Issues Found";
            else richTextBox3.Text = Parser.error;
            richTextBox4.Text = memorytext;
        }
    }
}