﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace YantraJS.Generator
{
    public static class ILGeneratorExtensions
    {

        /// <summary>
        /// Emits instruction with _s if index is less or equal to 32767
        /// </summary>
        /// <param name="il"></param>
        /// <param name="opCode_S"></param>
        /// <param name="opCode"></param>
        /// <param name="index"></param>
        public static void Emit(this ILGenerator il, OpCode opCode_S, OpCode opCode, int index)
        {
            if (index <= 32767)
            {
                il.Emit(opCode_S, (short)index);
                return;
            }
            il.Emit(opCode, index);
        }

        public static void EmitConstant(this ILGenerator il, object value)
        {
            if(value == null)
            {
                il.Emit(OpCodes.Ldnull);
                return;
            }

            switch (value)
            {
                case string @string:
                    il.EmitConstant(@string);
                    return;
                case int @int:
                    il.EmitConstant(@int);
                    return;
                case bool b:
                    il.EmitConstant(b);
                    return;
                case float f:
                    il.EmitConstant(f);
                    return;
                case double d:
                    il.EmitConstant(d);
                    return;
                case decimal dm:
                    il.EmitConstant(dm);
                    return;
            }

            throw new NotSupportedException($"Constant of type  {value.GetType()} not supported, you must use a factory to create value of specified type");

        }

        public static void EmitConstant(this ILGenerator il, decimal value)
        {
            // var bits = new int[4];
            var bits = decimal.GetBits(value);
            Type type = typeof(decimal);

            int scale = (bits[3] & int.MaxValue) >> 16;
            if (scale == 0)
            {
                if (int.MinValue <= value)
                {
                    if (value <= int.MaxValue)
                    {
                        int intValue = decimal.ToInt32(value);
                        switch (intValue)
                        {
                            case -1:
                                il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MinusOne)));
                                return;
                            case 0:
                                il.Emit(OpCodes.Initobj, typeof(decimal));
                                return;
                            case 1:
                                il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.One)));
                                return;
                            default:
                                il.EmitConstant(intValue);
                                il.EmitNew(type.GetConstructor(typeof(int)));
                                return;
                        }
                    }

                    if (value <= uint.MaxValue)
                    {
                        il.EmitConstant(decimal.ToUInt32(value));
                        il.EmitNew(type.GetConstructor(typeof(uint)));
                        return;
                    }
                }

                if (long.MinValue <= value)
                {
                    if (value <= long.MaxValue)
                    {
                        il.EmitConstant(decimal.ToInt64(value));
                        il.EmitNew(type.GetConstructor(typeof(long)));
                        return;
                    }

                    if (value <= ulong.MaxValue)
                    {
                        il.EmitConstant(decimal.ToUInt64(value));
                        il.EmitNew(type.GetConstructor(typeof(ulong)));
                        return;
                    }

                    if (value == decimal.MaxValue)
                    {
                        il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MaxValue)));
                        return;
                    }
                }
                else if (value == decimal.MinValue)
                {
                    il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MinValue)));
                    return;
                }
            }

            il.EmitConstant(bits[0]);
            il.EmitConstant(bits[1]);
            il.EmitConstant(bits[2]);
            il.EmitConstant((bits[3] & 0x80000000) != 0);
            il.EmitConstant(unchecked((byte)scale));
            // il.EmitNew(Decimal_Ctor_Int32_Int32_Int32_Bool_Byte);
            il.EmitNew(type.GetConstructor(typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte)));
        }

        public static void EmitNew(this ILGenerator il, ConstructorInfo c)
        {
            il.Emit(OpCodes.Newobj, c);
        }

        public static void EmitConstant(this ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }

        public static void EmitConstant(this ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitConstant(this ILGenerator il, bool value)
        {
            il.Emit(value ? OpCodes.Ldc_I4_1: OpCodes.Ldc_I4_0);
        }

        public static void EmitConstant(this ILGenerator il, uint i)
        {
            il.EmitConstant(unchecked((int)i));
        }

        public static void EmitConstant(this ILGenerator il, ulong i)
        {
            il.EmitConstant(unchecked((long)i));
        }

        public static void EmitConstant(this ILGenerator il, long i)
        {
            il.Emit(OpCodes.Ldc_I8, i);
        }

        public static void EmitSaveLocal(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Stloc_3);
                    return;
            }
            il.Emit(OpCodes.Stloc_S, OpCodes.Stloc, index);
        }

        public static void EmitLoadArg(this ILGenerator il,int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    return;
            }
            il.Emit(OpCodes.Ldarg_S, OpCodes.Ldarg, index);

        }

        public static void EmitLoadArgAddress(this ILGenerator il, int index)
        {
            il.Emit(OpCodes.Ldarga_S, OpCodes.Ldarga, index);

        }


        public static void EmitLoadLocal(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldloc_3);
                    return;
            }
            il.Emit(OpCodes.Ldloc_S, OpCodes.Ldloc, index);
        }

        public static void  EmitCall(this ILGenerator il, MethodInfo method)
        {
            if (method.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, method);
                return;
            }
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitLoadLocalAddress(this ILGenerator il, int index)
        {
            il.Emit(OpCodes.Ldloca_S, OpCodes.Ldloca, index);
        }


        public static void EmitConstant(this ILGenerator il, string @string)
        {
            il.Emit(OpCodes.Ldstr, @string);
        }

        public static void EmitConstant(this ILGenerator il, int i)
        {
            switch (i)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            il.Emit(OpCodes.Ldc_I4_S, OpCodes.Ldc_I4, i);
            return;
        }
    }
}