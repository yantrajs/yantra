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
    /// Load String member
    /// </summary>
    LdSE,

    /// <summary>
    /// Load KeyString Member
    /// </summary>
    LdKE,

    /// <summary>
    /// Load Integer member
    /// </summary>
    LdIE,

    /// <summary>
    /// Load Value member a[m]
    /// </summary>
    LdVE,

    /// <summary>
    /// Load String member
    /// </summary>
    StSE,

    /// <summary>
    /// Load KeyString Member
    /// </summary>
    StKE,

    /// <summary>
    /// Load Integer member
    /// </summary>
    StIE,

    /// <summary>
    /// Load Value member a[m]
    /// </summary>
    StVE,

    Apd,
    ApdS,

    NAry,

    Inv0,
    Inv1,
    Inv2,
    Inv3,
    Inv4,


    InvN,
    InvS,

    MetV0,
    MetV1,
    MetV2,
    MetV3,
    MetV4,

    MetVN,
    MetVS,

    MetK0,
    MetK1,
    MetK2,
    MetK3,
    MetK4,

    MetKN,
    MetKS,

    Add,
    Mul,
    Div,
    Mod,
    BAnd,
    BOr,
    BXor,
    BNot,

    // logical and/or do not exist
    // use operator coalesce

    //And,
    //Or,
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

    Awit,
    Dup,

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
