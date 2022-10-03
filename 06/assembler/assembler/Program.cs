instructionType InstructionType(string instruction)
{
    if (instruction[0] == '@') return instructionType.Ainstruction;
    else if (instruction[0] == '(') return instructionType.Linstruction;
    else return instructionType.Cinstruction;
}

var symbolTable = new SymbolTable();
var instructions = new List<string>();
var binInstructions = new List<string>();
var instructionTable = new InstructionTable();
var hasLSymbol = new bool[40000];
hasLSymbol[16384] = true;
hasLSymbol[24576] = true;
int addressnum = 16;
int staticnum = 16;

//读asm文件
using (var sr = new StreamReader(@"C:\Users\Lenovo\Desktop\nand2tetris\projects\06\pong\Pong.asm"))
{
    string line;

    while ((line = sr.ReadLine()) != null)
    {
        if (line == String.Empty) continue;
        string newLine = "";
        foreach (char i in line)
        {
            if (i == ' ') continue;
            if (i == '/') break;
            newLine += i;
        }
        if (newLine.Length > 0) instructions.Add(newLine);
    }
}
int pc = 0;
// First Pass
foreach (string instruction in instructions)
{
    instructionType type;
    type = InstructionType(instruction);
    if (type == instructionType.Ainstruction)
    {
        string symbol = instruction.Substring(1, instruction.Length - 1);
        foreach(char i in symbol)
        {
            if(i == '.')
            {
                if (!symbolTable.Contains(symbol))
                {
                    symbolTable.AddEntry(symbol, staticnum++);
                }
                break;
            }
        }
    }
    else
    if (type == instructionType.Linstruction)
    {
        string symbol = instruction.Substring(1, instruction.Length - 2);
        if (!symbolTable.Contains(symbol))
        {
            symbolTable.AddEntry(symbol, pc);
        }
        else symbolTable.symbolEntry[symbol] = pc;
        hasLSymbol[pc] = true;
        pc -= 1;
    }
    pc++;
    Console.WriteLine(instruction);
}

// Second Pass
foreach (string instruction in instructions)
{
    instructionType type;
    type = InstructionType(instruction);
    string bin = null;
    if (type == instructionType.Ainstruction)
    {
        string symbol = instruction.Substring(1, instruction.Length - 1);
        int temp = -1;
        if (symbol[0] <= '9' && symbol[0] >= '0')
        {
            temp = int.Parse(symbol);
        }
        else
        {
            if (!symbolTable.Contains(symbol))
            {
                while (hasLSymbol[addressnum]) addressnum++;
                symbolTable.AddEntry(symbol, addressnum);
            }
            temp = symbolTable.GetAddress(symbol);
        }
        bin = Convert.ToString(temp, 2);
        while (bin.Length < 16)
        {
            bin = "0" + bin;
        }
    }
    else if (type == instructionType.Linstruction)
    {
        continue;
    }
    else if (type == instructionType.Cinstruction)
    {
        string binDest, binComp, binJump;
        int compStart = 0, compEnd = instruction.Length - 1;
        if (instruction.IndexOf('=') == -1) binDest = "000";
        else
        {
            compStart = instruction.IndexOf('=') + 1;
            binDest = instructionTable.Dest(instruction.Substring(0, instruction.IndexOf('=')));
        }
        if (instruction.IndexOf(';') == -1) binJump = "000";
        else
        {
            compEnd = instruction.IndexOf(';') - 1;
            binJump = instructionTable.Jump(instruction.Substring(instruction.IndexOf(';') + 1));
        }
        binComp = instructionTable.Comp(instruction.Substring(compStart, compEnd - compStart + 1));
        bin = "111" + binComp + binDest + binJump;
    }
    Console.WriteLine(bin);
    binInstructions.Add(bin);
}

//写hack文件
using (StreamWriter sw = new StreamWriter(@"C:\Users\Lenovo\Desktop\nand2tetris\projects\06\pong\Pong.hack"))
{
    foreach (string instruction in binInstructions)
    {
        sw.WriteLine(instruction);
    }
}


enum instructionType
{
    Ainstruction,
    Cinstruction,
    Linstruction
}

// instruction 表
class InstructionTable
{
    public Dictionary<string, string> instructionDest = new Dictionary<string, string>();
    public Dictionary<string, string> instructionComp = new Dictionary<string, string>();
    public Dictionary<string, string> instructionJump = new Dictionary<string, string>();

    public InstructionTable()
    {
        InitializeDest();
        InitializeComp();
        InitializeJump();
    }

    private void InitializeDest()
    {
        instructionDest.Add("M", "001");
        instructionDest.Add("D", "010");
        instructionDest.Add("MD", "011");
        instructionDest.Add("A", "100");
        instructionDest.Add("AM", "101");
        instructionDest.Add("AD", "110");
        instructionDest.Add("ADM", "111");

    }

    private void InitializeComp()
    {
        instructionComp.Add("0", "0101010");
        instructionComp.Add("1", "0111111");
        instructionComp.Add("-1", "0111010");
        instructionComp.Add("D", "0001100");
        instructionComp.Add("A", "0110000");
        instructionComp.Add("!D", "0001101");
        instructionComp.Add("!A", "0110001");
        instructionComp.Add("-D", "0001111");
        instructionComp.Add("-A", "0110011");
        instructionComp.Add("D+1", "0011111");
        instructionComp.Add("A+1", "0110111");
        instructionComp.Add("D-1", "0001110");
        instructionComp.Add("A-1", "0110010");
        instructionComp.Add("D+A", "0000010");
        instructionComp.Add("D-A", "0010011");
        instructionComp.Add("A-D", "0000111");
        instructionComp.Add("D&A", "0000000");
        instructionComp.Add("D|A", "0010101");
        instructionComp.Add("M", "1110000");
        instructionComp.Add("!M", "1110001");
        instructionComp.Add("-M", "1110011");
        instructionComp.Add("M+1", "1110111");
        instructionComp.Add("M-1", "1110010");
        instructionComp.Add("D+M", "1000010");
        instructionComp.Add("D-M", "1010011");
        instructionComp.Add("M-D", "1000111");
        instructionComp.Add("D&M", "1000000");
        instructionComp.Add("D|M", "1010101");
    }

    private void InitializeJump()
    {
        instructionJump.Add("JGT", "001");
        instructionJump.Add("JEQ", "010");
        instructionJump.Add("JGE", "011");
        instructionJump.Add("JLT", "100");
        instructionJump.Add("JNE", "101");
        instructionJump.Add("JLE", "110");
        instructionJump.Add("JMP", "111");
    }

    public string Dest(string dest)
    {
        return instructionDest[dest];
    }

    public string Comp(string comp)
    {
        return instructionComp[comp];
    }

    public string Jump(string jump)
    {
        return instructionJump[jump];
    }
}

// symbol 表
class SymbolTable
{
    public Dictionary<string, int> symbolEntry = new Dictionary<string, int>();

    public SymbolTable()
    {
        for (int i = 0; i <= 15; i++)
        {
            symbolEntry.Add($"R{i}", i);
        }
        symbolEntry.Add("SP", 0);
        symbolEntry.Add("LCL", 1);
        symbolEntry.Add("ARG", 2);
        symbolEntry.Add("THIS", 3);
        symbolEntry.Add("THAT", 4);
        symbolEntry.Add("SCREEN", 16384);
        symbolEntry.Add("KBD", 24576);
    }
    public void AddEntry(string symbol, int address)
    {
        symbolEntry.Add(symbol, address);
    }

    public bool Contains(string symbol)
    {
        return symbolEntry.ContainsKey(symbol);
    }

    public int GetAddress(string symbol)
    {
        return (int)symbolEntry[symbol];
    }
}