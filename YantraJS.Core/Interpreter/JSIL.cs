using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Interpreter;

public enum JSIL
{

    /// <summary>
    /// None
    /// </summary>
    None,

    /// <summary>
    /// Load String named Variable to stack
    /// </summary>
    Load,

    /// <summary>
    /// Store Variable from stack
    /// </summary>
    Stor,

    /// <summary>
    /// Jump
    /// </summary>
    Jump,

    /// <summary>
    /// Jump if top of the stack is true
    /// </summary>
    JmpT,

    /// <summary>
    /// Jump if top of the stack is false
    /// </summary>
    JmpF,

    /// <summary>
    /// Coalesce
    /// </summary>
    Colc,

    /// <summary>
    /// String member
    /// </summary>
    StrM,

    /// <summary>
    /// KeyString Member
    /// </summary>
    KeyM,

    /// <summary>
    /// Integer member
    /// </summary>
    IntM,

    /// <summary>
    /// Value member a[m]
    /// </summary>
    ValM,

    Inv0,
    Inv1,
    Inv2,
    Inv3,
    Inv4,


    InvN,

    Met0,
    Met1,
    Met2,
    Met3,
    Met4,

    MetN,

    Add,
    Mul,
    Div,
    Mod,
    And,
    Or,
    Not,

    /// <summary>
    /// Execute Given CLR Method on JSValue instance or given specified static method 
    /// </summary>
    Clr0,


    /// <summary>
    /// Return undefined
    /// </summary>
    RetU,

    /// <summary>
    /// Return from top of the stack
    /// </summary>
    RetV,

    Yild,

    Thro,

    /// <summary>
    /// Begin Try
    /// </summary>
    BegT,
    EndT,


    /// <summary>
    /// Catch
    /// </summary>
    BegC,
    EndC,

    /// <summary>
    /// Finally
    /// </summary>
    BegF,
    EndF,


}
