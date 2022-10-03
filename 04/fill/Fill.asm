// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Pseudocode 
// 根据原理图只要Address从0-8192都是1111111111111111即-1便可全黑
// 都是0即可以全白
// Start:
//  R0 = 0 , R1 = 8192 设置Address的起点和终点
//  Screen , address , KBD 设置IN/OUT
//  if( KBD > 0 ) GOTO Blackens
// Whiten:
//  R2 = 0
//  GOTO Painting
// Blacken:
//  R2 = -1 
// Painting:
//  if( R0 > R1 ) GOTO Start
//  RAM[Address] = R2  ( R2 = 0 / -1)
//  Address = Address + 1
//  R0 = R0 + 1
//  GOTO Painting
// END:
//   GOTO END

// Put your code here.

(START)
    // 设置 R0 = 0 
    @0
    D = A 
    @R0
    M = D

    // 设置 R1 = 8192
    @8192
    D = A 
    @R1 
    M = D

    //设置Screen和Screen的address
    @SCREEN 
    D = A 
    @address 
    M = D
    
    //设置键盘输入
    @KBD
    D = M

    //如果键盘输入大于0，则跳去BLACK
    //否则正常向下运行，即WHITE
    @BLACK 
    D;JGT

(WHITE)
    //设置 R2 为 0
    @0
    D = A 
    @R2
    M = D
    //进入绘图
    @PAINT 
    0;JMP

(BLACK)
    //设置 R2 为 -1
    D = -1
    @R2 
    M = D

(PAINT)
    //判断 R0 是否大于 R1
    //如果是则跳回开头
    @R0
    D = M 
    @R1 
    D = D - M 
    @START 
    D;JGE

    //赋值R2到Address中
    @R2 
    D = M 
    @address
    A = M 
    M = D

    //完成一次循环，R0和地址进行迭代
    @R0
    M = M + 1
    @address 
    M = M + 1
    @PAINT
    0;JMP

(END)
    @END
    0;JMP

